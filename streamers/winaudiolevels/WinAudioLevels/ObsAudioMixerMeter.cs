using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WinAudioLevels {
    public class ObsAudioMixerMeter {
        public static event EventHandler AudioMixerOcrStarting;
        internal static event EventHandler<Tuple<Rectangle, int>> AudioMixerOcrAttempt; //read region, meter
        internal static readonly MD5CryptoServiceProvider HASHER = new MD5CryptoServiceProvider();

        private static readonly object METERS_LOCK = new object();
        private static readonly object METER_LEVELS_LOCK = new object();
        private static readonly object METER_LEVELS_SEQ_LOCK = new object();
        private static readonly object STATUSES_LOCK = new object();
        private static readonly List<ObsAudioMixerMeter> METERS = new List<ObsAudioMixerMeter>();
        private static readonly Dictionary<string, double> METER_LEVELS = new Dictionary<string, double>();
        private static readonly List<double> METER_LEVELS_SEQ = new List<double>();
        private static readonly Dictionary<object, ObsAudioMeterStatus> STATUSES = new Dictionary<object, ObsAudioMeterStatus>();
        
        public const int MAX_PIXELS_BETWEEN_CHANNELS = 10;
        public string ThemeName => this.Theme.name;
        public ObsTheme Theme { get; set; }
        public string MeterName { get; set; }
        public string MeterId { get; set; }
        public Rectangle MeterChannelTop { get; set; }
        public Rectangle MeterChannelBottom { get; set; }
        public bool HasDualMeter => this.MeterChannelBottom != default;

        public string GetMeterId(Bitmap bmp, Rectangle rect) {
            
            List<List<bool>> bits = new List<List<bool>>();
            Color[] bgColors = this.Theme.meterBackgroundColors;
            //step 1: convert the OCR region to bool[][] where false=bg, true=!bg
            for (int x = rect.X, i = 0; x < Math.Min(rect.Right, bmp.Width); x++, i++) {
                bits.Add(new List<bool>());
                for (int y = rect.Y; y < Math.Min(rect.Bottom, bmp.Height); y++) {
                    bits[i].Add(!bgColors.Contains(bmp.GetPixel(x, y)));
                }
            }
            //step 2: trim the top all false rows and the right all false columns.
            bool check = true;
            while (check) {
                check = false;
                //x,y
                if (bits[bits.Count - 1].All(a => !a)) {
                    bits.RemoveAt(bits.Count - 1);
                    check = true;
                    continue;
                }
                if (bits.All(a => !a[0])) {
                    bits.ForEach(a => a.RemoveAt(0));
                    check = true;
                    continue;
                }
            }
            //step 3: compute hash.
            List<bool> tmp = new List<bool>();
            bits.ForEach(a => tmp.AddRange(a));
            BitArray bitArray = new BitArray(tmp.ToArray());
            byte[] bytes = new byte[(bitArray.Length - 1) / 8 + 1];
            bitArray.CopyTo(bytes, 0);
            string hashString = string.Format(
                "{1}.{2}.{0}",
                Convert.ToBase64String(bytes),
                bits.Count,
                bits.Count == 0 ? 0 : bits[0].Count);
            //Console.WriteLine("Hash String: \"{0}\"", hashString);
            byte[] hash = HASHER.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            //128-bit, so 16-bytes
            //hex would be 2ch/byte = 32 characters.
            //base64 would be 4ch/3bytes = 24 characters.
            //ehhhhh good enough
            string ret = Convert.ToBase64String(hash).Replace("=", "");
            return ret;
        }

        public double MeterLevel {
            get {
                double levelA = double.MinValue;
                double levelB = double.MinValue;
                bool bTemp = false;
                Color[] colors = this.Theme.meterColors.Where(a => bTemp = !bTemp).ToArray();
                if (this.HasDualMeter) {
                    double maxX = this.MeterChannelBottom.X;
                    for (int x = this.MeterChannelBottom.X; x < this.MeterChannelBottom.Right; x++) {
                        if (colors.Contains(OBSCapture._bitmap.GetPixel(x, this.MeterChannelBottom.Y))) {
                            maxX = x;
                        } else if (maxX != this.MeterChannelBottom.X) {
                            break; //avoid catching the dot thingies
                        } else {
                            break;
                        }
                    }
                    levelB = ((maxX - this.MeterChannelBottom.X) / this.MeterChannelBottom.Width * 60) - 60;
                }
                {
                    double maxX = this.MeterChannelTop.X;
                    for (int x = this.MeterChannelTop.X; x < this.MeterChannelTop.Right; x++) {
                        if (colors.Contains(OBSCapture._bitmap.GetPixel(x, this.MeterChannelTop.Y))) {
                            maxX = x;
                        } else if (maxX != this.MeterChannelBottom.X) {
                            break; //avoid catching the dot thingies
                        } else {
                            break;
                        }
                    }
                    levelA = ((maxX - this.MeterChannelTop.X) / this.MeterChannelTop.Width * 60) - 60;
                }
                return Math.Max(levelA, levelB);
            }
        }

        public static int MeterCount {
            get {
                int scanLineA = (OBSCapture._bitmap.Width / 2) + 10; //search 20 apart
                int scanLineB = (OBSCapture._bitmap.Width / 2) - 10; //we do this because OBS displays two peaks. The second interrupts the usual color of the meter
                int height = OBSCapture._bitmap.Height;
                if (scanLineA < 0) {
                    return 0; //window is too thin... :(
                }
                int count = 0;
                bool anyMeter = false;
                bool meter = false;
                int pAfter = 0;
                for (int y = 0; y < height; y++) {
                    Color a = OBSCapture._bitmap.GetPixel(scanLineA, y);
                    Color b = OBSCapture._bitmap.GetPixel(scanLineB, y);
                    if (ObsTheme.ALL_METER_COLORS.Contains(a) || ObsTheme.ALL_METER_COLORS.Contains(b)) {
                        if (!meter && (pAfter > MAX_PIXELS_BETWEEN_CHANNELS || !anyMeter)) {
                            count++;
                        }
                        anyMeter = meter = true;
                        pAfter = 0;
                    } else if (anyMeter) {
                        pAfter++;
                        meter = false;
                    }
                }
                return count;
            }
        }

        internal static void UpdateMeters() {
            //ObsAudioMixerMeter
            int scanLineA = (OBSCapture._bitmap.Width / 2) + 10; //search 20 apart
            int scanLineB = (OBSCapture._bitmap.Width / 2) - 10; //we do this because OBS displays two peaks. The second interrupts the usual color of the meter
            int height = OBSCapture._bitmap.Height;
            if (scanLineA < 0) {
                return; //window is too thin... :(
            }
            int count = 0;
            bool anyMeter = false;
            bool meter = false;
            int pAfter = 0;
            bool startedB = false;

            int yStartA = 0;
            int yEndA = 0;
            int yStartB = 0;
            int yEndB = 0;
            ObsTheme theme = default;
            Color[] match_colors = ObsTheme.ALL_METER_COLORS;
            Color tick_color = default;
            List<Tuple<Point, Point>> coords = new List<Tuple<Point, Point>>();
            //a:start,end; b:start,end
            for (int y = 0; y < height; y++) {
                Color a = OBSCapture._bitmap.GetPixel(scanLineA, y);
                Color b = OBSCapture._bitmap.GetPixel(scanLineB, y);
                if (match_colors.Contains(a) || match_colors.Contains(b)) {
                    theme = ObsTheme.GetThemeByMeterColor(a, b);
                    if (ReferenceEquals(match_colors, ObsTheme.ALL_METER_COLORS)) {
                        //ReferenceEquals because it's quicker and we don't want to check sequences.
                        //we just care if we're checking all meter colors or not.
                        match_colors = theme.meterColors;
                        tick_color = theme.meterTickColor;
                    }
                    if (!meter && (pAfter > MAX_PIXELS_BETWEEN_CHANNELS || !anyMeter)) {
                        if (anyMeter) {
                            coords.Add(new Tuple<Point, Point>(
                                new Point(yStartA, yEndA),
                                new Point(yStartB, yEndB)));
                            yStartA =
                                yStartB =
                                yEndA =
                                yEndB = 0;
                        }
                        count++; //new meter
                        yStartA = y;
                        startedB = false;
                    } else if (!meter && (pAfter < MAX_PIXELS_BETWEEN_CHANNELS)) {
                        //new channel
                        yStartB = y;
                        startedB = true;
                    }
                    anyMeter = meter = true;
                    pAfter = 0;
                } else if (anyMeter) {
                    if (pAfter == 0) {
                        if (startedB) {
                            yEndB = y;
                        } else {
                            yEndA = y;
                        }
                    }
                    meter = false;
                    pAfter++;
                }
            }
            if (anyMeter && yStartA > 0) {
                coords.Add(new Tuple<Point, Point>(
                    new Point(yStartA, yEndA),
                    new Point(yStartB, yEndB)));
            }

            if (coords.Count == 0) {
                return; //found no meters... :(
            }
            {
                Tuple<Point, Point> coord = coords[0];
                int y = coord.Item2.X == 0
                    ? coord.Item1.Y + 1
                    : coord.Item2.Y + 1;
                int lastX = -1;
                int firstX = -1;
                for (int x = OBSCapture._bitmap.Width - 1; x >= 0; x--) {
                    if (OBSCapture._bitmap.GetPixel(x, y) == tick_color) {
                        lastX = x;
                        if (firstX == -1) {
                            firstX = x;
                        }
                    }
                }
                if (lastX == -1) {
                    return;
                    //could not find ticks somehow...? :(
                }
                int xSize = firstX - lastX;
                lock (METERS_LOCK) {
                    METERS.Clear();
                }
                int mIndex = 0;
                AudioMixerOcrStarting?.Invoke(null, new EventArgs()); //can't call event handler outside of declaring class :(
                foreach (Tuple<Point, Point> coordinate in coords) {
                    ObsAudioMixerMeter oMeter = new ObsAudioMixerMeter() {
                        MeterChannelTop = new Rectangle(
                             new Point(lastX, coordinate.Item1.X),
                             new Size(xSize, coordinate.Item1.Y - coordinate.Item1.X)),
                        MeterChannelBottom = coord.Item2.X == 0
                            ? default
                            : new Rectangle(
                                 new Point(lastX, coordinate.Item2.X),
                                 new Size(xSize, coordinate.Item2.Y - coordinate.Item2.X)),
                        Theme = theme
                    };
                    Rectangle nameRect = new Rectangle(
                        0,
                        Math.Max(oMeter.MeterChannelTop.Y - 26, 0),
                        Math.Min(oMeter.MeterChannelTop.Width + oMeter.MeterChannelTop.X - 70, OBSCapture._bitmap.Width),
                        Math.Min(25 + Math.Min(oMeter.MeterChannelTop.Y - 26, 0), OBSCapture._bitmap.Height));
                    //ocr rect: {0, max(0, top.Y - 26)} width = min(bmp.width, top.Width + top.X - 70), height = min(bmp.height, 25 + min(0, top.Y - 26))
                    AudioMixerOcrAttempt?.Invoke(null, new Tuple<Rectangle, int>(nameRect, mIndex)); //cannot call event handler outside of declaring class :(
                    oMeter.MeterId = oMeter.GetMeterId(OBSCapture._bitmap, nameRect);
                    Console.WriteLine("Found Meter (id): {0}", oMeter.MeterId);
                    lock (METERS_LOCK) {
                        METERS.Add(oMeter);
                    }
                    mIndex++;
                }
                //55 pixels off rectangle end.
                //2 pixels above meter
                //25 pixels high
            }

            //scan pixel below bottom meter

        }
        internal static void ResetMeters(bool b = false) {
            lock (METERS_LOCK) {
                METERS.Clear();
            }
            if (b) {
                lock (METER_LEVELS_LOCK) {
                    METER_LEVELS.Clear();
                }
                lock (METER_LEVELS_SEQ_LOCK) {
                    METER_LEVELS_SEQ.Clear();
                }
                UpdateMeters();
                lock (METERS_LOCK) {
                    if (METERS.Count > 0) {
                        METERS.ForEach(a => {
                            lock (METER_LEVELS_LOCK) {
                                METER_LEVELS[a.MeterId] = -60;
                            }
                            lock (METER_LEVELS_SEQ_LOCK) {
                                METER_LEVELS_SEQ.Add(-60);
                            }
                        });
                    }
                }
            }
        }
        internal static void RefreshMeterLists() {
            int mCount = 0;
            lock (METERS_LOCK) {
                mCount = METERS.Count;
            }
            if (MeterCount != mCount) {
                ResetMeters(true);
            }
            int index = 0;
            lock (METERS_LOCK) {
                if (METERS.Count > 0) {
                    METERS.ForEach(a => {
                        double level = a.MeterLevel;
                        lock (METER_LEVELS_LOCK) {
                            METER_LEVELS[a.MeterId] = level;
                        }
                        lock (METER_LEVELS_SEQ_LOCK) {
                            METER_LEVELS_SEQ[index++] = level;
                        }
                        //update the meter levels.
                    });
                }
            }
        }

        public static double? GetAudioMeterLevel(string meterName) {
            lock (METER_LEVELS_LOCK) {
                return !METER_LEVELS.ContainsKey(meterName)
                    ? null
                    : (double?)METER_LEVELS[meterName];
            }
        }
        public static double? GetAudioMeterLevel(int index) {
            lock (METER_LEVELS_SEQ_LOCK) {
                return METER_LEVELS_SEQ.Count > index
                    ? (double?)METER_LEVELS_SEQ[index]
                    : null;
            }
        }
        public static double[] GetAllAudioMeterLevel() {
            lock (METER_LEVELS_SEQ_LOCK) {
                return METER_LEVELS_SEQ.ToArray();
            }
        }
        public static ObsAudioMeterStatus? GetStatus(object token) {
            lock (STATUSES_LOCK) {
                if (!STATUSES.ContainsKey(token)) {
                    return null;
                }
                ObsAudioMeterStatus ret = STATUSES[token];
                STATUSES.Remove(ret);
                return ret;
            }
        }


        public static ObsTheme? CurrentObsTheme {
            get {
                lock (METERS_LOCK) {
                    return METERS.Count > 0
                        ? (ObsTheme?)METERS[0].Theme
                        : null;
                }
            }
        }
        /*
        switch (METERS[0].ThemeName) {
        case "Acri":
            return ObsTheme.ACRI;
        case "System":
            return ObsTheme.SYSTEM;
        case "Dark":
            return ObsTheme.DARK;
        case "Rachni":
            return ObsTheme.RACHNI;
        }
        */
        public static string CurrentObsThemeName {
            get {
                lock (METERS_LOCK) {
                    return METERS.Count > 0
                        ? METERS[0].ThemeName
                        : null;
                }
            }
        }
    }
}
