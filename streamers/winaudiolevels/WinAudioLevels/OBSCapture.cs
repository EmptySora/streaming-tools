using IronOcr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

namespace WinAudioLevels {
    public static class OBSCapture {

        #region Imports
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(
            IntPtr hWndParent,
            [MarshalAs(UnmanagedType.FunctionPtr)]WNDENUMPROC lpEnumFunc, //callback
            IntPtr lParam); //set lParam to IntPtr.zero
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextW(
            IntPtr hWnd,
            IntPtr lpString, //pass in a buffer.
            int nMaxCount);
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLengthW(
            IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool WNDENUMPROC(
            [In]IntPtr hwnd,
            [In]IntPtr lParam); //ignore.
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PrintWindow(
            IntPtr hWnd,
            IntPtr hdcBlt, //Would the graphics pointer work...?
            uint nFlags); //Set to PW_CLIENTONLY
        private const uint PW_CLIENTONLY = 0x1;
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(
            IntPtr hWnd);
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(
            IntPtr hWnd,
            [MarshalAs(UnmanagedType.LPStruct)]out RECT lpRect);
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion
        //PrintWindow
        // 0: Found "Audio Mixer" window.
        //-1: Could not find process
        //-2: Could not find "Audio Mixer" window.
        private static IntPtr _h_audio_mixer = IntPtr.Zero;

        //use these to hook into the error and display a warning to the user
        //BE SURE TO ONLY SHOW A MESSAGE ONCE UNTIL ERRORFIXED is HIT
        public static event EventHandler OBSNotFoundError;
        public static event EventHandler AudioMixerWindowNotFoundError;
        public static event EventHandler OBSErrorFixed;
        internal static event EventHandler<Image> AudioMixerWindowCaptured;
        private static bool _errored = false;
        private static int _listener_count = 0;
        private static Size _window_size;
        private static Graphics _graphics;
        private static Bitmap _bitmap;
        public const int FPS = 30;
        private static readonly Thread THREAD = new Thread(CaptureLoop);
        static OBSCapture() {
            THREAD.Name = "OBS Capture Thread";
            THREAD.Start();
        }
        private static readonly IronTesseract TESSERACT = new IronTesseract();
        //key is the theme's name. value is array of colors: ggyyrr (first is light, second is dark)
        private static readonly Dictionary<string, Color[]> METER_COLORS = new Dictionary<string, Color[]>() {
            {
                "Dark", new Color[] {
                    Color.FromArgb(76,255,76), //GREEN: ACTIVE
                    Color.FromArgb(38,127,38), //GREEN: INACTIVE
                    Color.FromArgb(255,255,76), //YELLOW: ACTIVE
                    Color.FromArgb(127,127,38), //YELLOW: INACTIVE
                    Color.FromArgb(255,76,76), //RED: ACTIVE
                    Color.FromArgb(127,38,38), //RED: INACTIVE
                }
            },
            {
                "Acri", new Color[] {
                    Color.FromArgb(132,216,43), //GREEN: ACTIVE
                    Color.FromArgb(66,116,12), //GREEN: INACTIVE
                    Color.FromArgb(228,215,23), //YELLOW: ACTIVE
                    Color.FromArgb(152,143,15), //YELLOW: INACTIVE
                    Color.FromArgb(215,65,22), //RED: ACTIVE
                    Color.FromArgb(128,32,4), //RED: INACTIVE
                }
            },
            {
                "Rachni", new Color[] {
                    Color.FromArgb(119,255,143), //GREEN: ACTIVE
                    Color.FromArgb(0,128,79), //GREEN: INACTIVE
                    Color.FromArgb(255,157,76), //YELLOW: ACTIVE
                    Color.FromArgb(128,57,0), //YELLOW: INACTIVE
                    Color.FromArgb(255,89,76), //RED: ACTIVE
                    Color.FromArgb(128,9,0), //RED: INACTIVE
                }
            },
            {
                "System", new Color[] {
                    Color.FromArgb(50,200,50), //GREEN: ACTIVE
                    Color.FromArgb(15,100,15), //GREEN: INACTIVE
                    Color.FromArgb(255,200,50), //YELLOW: ACTIVE
                    Color.FromArgb(100,100,15), //YELLOW: INACTIVE
                    Color.FromArgb(200,50,50), //RED: ACTIVE
                    Color.FromArgb(100,15,15), //RED: INACTIVE
                }
            }
        };
        private static readonly Dictionary<string, Color> METER_TICK_COLORS = new Dictionary<string, Color>() {
            { "Dark", Color.FromArgb(225,224,225) },
            { "Acri", Color.FromArgb(239,240,241) },
            { "Rachni", Color.FromArgb(239,240,241) },
            { "System", Color.FromArgb(0,0,0) }
        };
        private static readonly Color[] ALL_METER_COLORS = METER_COLORS["Dark"]
            .Union(METER_COLORS["Acri"])
            .Union(METER_COLORS["Rachni"])
            .Union(METER_COLORS["System"])
            .Distinct()
            .ToArray();
        private static readonly List<ObsAudioMixerMeter> METERS = new List<ObsAudioMixerMeter>();
        private static readonly object LOCK = new object();
        private static readonly Dictionary<string, double> METER_LEVELS = new Dictionary<string, double>();
        private static readonly List<double> METER_LEVELS_SEQ = new List<double>();
        private static int GetAudioMixer(out IntPtr hAudioMixer) {
            Process obs = null;
            hAudioMixer = IntPtr.Zero;
            foreach (Process proc in Process.GetProcesses()) {
                string path = proc.MainModule.FileName;
                string[] dirs = Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar);
                string file = Path.GetFileNameWithoutExtension(path).ToLower();
                IEnumerable<Process> children;
                if (file == "obs32" || file == "obs64") {
                    children = proc.GetChildProcesses();
                    if (children.Any(child => {
                        return Path
                        .GetFileNameWithoutExtension(child.MainModule.FileName)
                        .ToLower() == "obs-browser-page";
                    })) {
                        //DEFINITELY the right one.
                        obs = proc;
                        break;
                    } else {
                        continue;
                        //we continue here because they *should* have at least one browser control
                        //How do we know that? Because this app is used in conjunction with my background
                        //animation (which plays in a browser control).
                        //sooo... it's a safe bet that the user has a browser control and, thus, OBS has
                        //at least one "obs-browser-page.exe" subprocess.
                    }
                }
                //obs-browser-page.exe
                //obs32.exe / obs64.exe
            }
            if (obs == null) {
                return -1; //could not find obs... :(
            }
            IntPtr pHandle = obs.Handle;
            IntPtr mwHandle = obs.MainWindowHandle;
            IntPtr handle = IntPtr.Zero;
            bool EnumCallback(IntPtr hWnd, IntPtr lParam) {
                IntPtr nameBuffer = IntPtr.Zero;
                const string expectedTitle = "Audio Mixer";
                try {
                    nameBuffer = Marshal.AllocHGlobal(512 * 2);
                    int chars = GetWindowTextW(hWnd, nameBuffer, 512); //length less null term
                    if (chars == 0) {
                        throw new Win32Exception();
                    }
                    if (chars == expectedTitle.Length) {
                        string name = Marshal.PtrToStringUni(nameBuffer, chars);
                        if (expectedTitle == name) {
                            handle = hWnd;
                            return false;
                            //we have what we came for.
                            //let's get gone.
                        }
                    }
                    //2b null term

                } finally {
                    if (IntPtr.Zero != nameBuffer) {
                        Marshal.FreeHGlobal(nameBuffer);
                    }
                }
                return true;
            }
            //LOOK FOR Qt5QWindowIcon[Text: "Audio Mixer"]
            //
            EnumChildWindows(mwHandle, EnumCallback, IntPtr.Zero); //return value unused.
            hAudioMixer = handle;

            if (hAudioMixer == IntPtr.Zero) {
                return -2; //Audio Mixer window wasn't found... :(
            }

            return 0;
            //C:\Program Files (x86)\obs-studio\bin\64bit\obs64.exe
        }
        private static Size GetWindowSize(IntPtr hwnd) {
            if (!GetWindowRect(hwnd, out RECT rect)) {
                throw new Win32Exception();
            }
            //inc exc
            return new Size(rect.right - rect.left, rect.bottom - rect.top);
        }
        private static void CaptureLoop() {
            TimeSpan errorWait = TimeSpan.FromSeconds(30);
            TimeSpan idleWait = TimeSpan.FromSeconds(2);
            TimeSpan activeWait = TimeSpan.FromMilliseconds(1000F / FPS);
            while (true) {
                try {
                    if (_listener_count > 0) {
                        CaptureMain();
                        Thread.Sleep(activeWait); //wait to capture next frame.
                    } else {
                        Thread.Sleep(idleWait);
                    }
                } catch (ThreadAbortException) {
                    throw; //rethrow to allow abort to work.
                } catch (FileNotFoundException) {
                    Console.WriteLine("Could not find obs64.exe/obs32.exe! Waiting {0} seconds...", errorWait.TotalSeconds);
                    OBSNotFoundError?.Invoke(null, new EventArgs());
                    _errored = true;
                } catch (EntryPointNotFoundException) {
                    Console.WriteLine("Could not find Audio Mixer window! Waiting {0} seconds...", errorWait.TotalSeconds);
                    AudioMixerWindowNotFoundError?.Invoke(null, new EventArgs());
                    _errored = true;
                } catch (Exception e) {
                    Console.WriteLine("ERROR IN OBS CAPTURE LOOP:\n{0}", e);
                    Thread.Sleep(errorWait);
                }
            }
        }
        private static void CaptureMain() {
            //check if window was closed.
            if (!(_h_audio_mixer == IntPtr.Zero || IsWindow(_h_audio_mixer))) {
                _h_audio_mixer = IntPtr.Zero;
                _window_size = default;
                METERS.Clear();
                //hAudioMixer was closed...
                //this should work on everything but 16-bit windows.
                //windows uses the upper 16-bits of hwnd to handle duplicate window handles.
                Console.WriteLine("Audio Mixer windows was closed...?");
            }
            //obtain window if we don't have it.
            if (_h_audio_mixer == IntPtr.Zero) {
                switch (GetAudioMixer(out _h_audio_mixer)) {
                case -1:
                    throw new FileNotFoundException("Could not find the obs process.");
                case -2:
                    throw new EntryPointNotFoundException("Could not find the Audio Mixer window.");
                }
                if (_errored) {
                    _errored = false;
                    OBSErrorFixed?.Invoke(null, new EventArgs());
                }
            }
            //check window size.
            Size currentSize = GetWindowSize(_h_audio_mixer);
            //if window size changed, we need to get new drawing buffers.
            if (currentSize != _window_size) {
                if (!(_graphics is null)) {
                    _graphics.Dispose();
                }
                if (!(_bitmap is null)) {
                    _bitmap.Dispose();
                }
                METERS.Clear();
                _bitmap = new Bitmap(currentSize.Width, currentSize.Height);
                _graphics = Graphics.FromImage(_bitmap);
            }
            _window_size = currentSize;
            //capture OBS window. (PW_CLIENTONLY specifies not to capture the window border.)
            if (!PrintWindow(_h_audio_mixer, _graphics.GetHdc(), PW_CLIENTONLY)) {
                throw new Win32Exception();
            }
            //send this to the debuggin context.
            AudioMixerWindowCaptured?.Invoke(null, (Image)_bitmap.Clone());
            //for debugging
            //this will allow me to create a form to "see" the OBS Audio Mixer and make sure this is working.

            //the bitmap should now have the full Audio Mixer window (excluding the borders and top bar)
            //to detect the meters, we 

            if (GetMeterCount() != METERS.Count()) {
                METERS.Clear();
                lock (LOCK) {
                    METER_LEVELS.Clear();
                    METER_LEVELS_SEQ.Clear();
                }
                UpdateMeters();
                lock (LOCK) {
                    METERS.ForEach(a => {
                        METER_LEVELS[a.MeterName] = -60;
                        METER_LEVELS_SEQ.Add(-60);
                    });
                }
            }
            int index = 0;
            METERS.ForEach(a => {
                double level = GetMeterLevel(a);
                lock (LOCK) {
                    METER_LEVELS[a.MeterName] = level;
                    METER_LEVELS_SEQ[index++] = level;
                    //update the meter levels.
                }
            });
#warning We are currently not checking to see if the Audio Mixer is Vertical or Horizontal...
            //OH MY GOD, THERE'S A VERTICAL LAYOUT... FUCK ME
        }
        private static double GetMeterLevel(ObsAudioMixerMeter meter) {
            double levelA = double.MinValue;
            double levelB = double.MinValue;
            bool bTemp = false;
            Color[] colors = METER_COLORS[meter.Theme].Where(a => bTemp = !bTemp).ToArray();
            if (meter.HasDualMeter) {
                double maxX = meter.MeterChannelBottom.X;
                for (int x = meter.MeterChannelBottom.X; x < meter.MeterChannelBottom.Right; x++) {
                    if (colors.Contains(_bitmap.GetPixel(x, meter.MeterChannelBottom.Y))) {
                        maxX = x;
                    }
                }
                levelB = ((maxX - meter.MeterChannelBottom.X) / meter.MeterChannelBottom.Width * 60) - 60;
            }
            {
                double maxX = meter.MeterChannelTop.X;
                for (int x = meter.MeterChannelTop.X; x < meter.MeterChannelTop.Right; x++) {
                    if (colors.Contains(_bitmap.GetPixel(x, meter.MeterChannelTop.Y))) {
                        maxX = x;
                    }
                }
                levelA = ((maxX - meter.MeterChannelTop.X) / meter.MeterChannelTop.Width * 60) - 60;
            }
            return Math.Max(levelA, levelB);
        }
        private static int GetMeterCount() {
            int scanLineA = (_bitmap.Width / 2) + 10; //search 20 apart
            int scanLineB = (_bitmap.Width / 2) - 10; //we do this because OBS displays two peaks. The second interrupts the usual color of the meter
            int height = _bitmap.Height;
            if (scanLineA < 0) {
                return 0; //window is too thin... :(
            }
            int count = 0;
            bool anyMeter = false;
            bool meter = false;
            int pAfter = 0;
            for (int y = 0; y < height; y++) {
                Color a = _bitmap.GetPixel(scanLineA, y);
                Color b = _bitmap.GetPixel(scanLineB, y);
                if (ALL_METER_COLORS.Contains(a) || ALL_METER_COLORS.Contains(b)) {
                    if (!meter && (pAfter > ObsAudioMixerMeter.MAX_PIXELS_BETWEEN_CHANNELS || !anyMeter)) {
                        count++;
                    }
                    anyMeter = meter = true;
                    pAfter = 0;
                } else if (anyMeter) {
                    pAfter++;
                }
            }
            return count;
        }
        private static void UpdateMeters() {
            //ObsAudioMixerMeter
            int scanLineA = (_bitmap.Width / 2) + 10; //search 20 apart
            int scanLineB = (_bitmap.Width / 2) - 10; //we do this because OBS displays two peaks. The second interrupts the usual color of the meter
            int height = _bitmap.Height;
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
            string theme = null;
            Color[] match_colors = ALL_METER_COLORS;
            Color tick_color = default;
            List<Tuple<Point, Point>> coords = new List<Tuple<Point, Point>>();
            //a:start,end; b:start,end
            for (int y = 0; y < height; y++) {
                Color a = _bitmap.GetPixel(scanLineA, y);
                Color b = _bitmap.GetPixel(scanLineB, y);
                if (match_colors.Contains(a) || match_colors.Contains(b)) {
                    theme = METER_COLORS.First(c => c.Value.Contains(a) || c.Value.Contains(b)).Key;
                    if (ReferenceEquals(match_colors, ALL_METER_COLORS)) {
                        //ReferenceEquals because it's quicker and we don't want to check sequences.
                        //we just care if we're checking all meter colors or not.
                        match_colors = METER_COLORS[theme];
                        tick_color = METER_TICK_COLORS[theme];
                    }
                    if (!meter && (pAfter > ObsAudioMixerMeter.MAX_PIXELS_BETWEEN_CHANNELS || !anyMeter)) {
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
                    } else if (!meter && (pAfter < ObsAudioMixerMeter.MAX_PIXELS_BETWEEN_CHANNELS)) {
                        //new channel
                        yStartB = y;
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
                for (int x = _bitmap.Width - 1; x >= 0; x--) {
                    if (_bitmap.GetPixel(x, y) == tick_color) {
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
                METERS.Clear();
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
                    Rectangle nameRect = default;
                    try {
                        nameRect = new Rectangle(
                            0,
                            oMeter.MeterChannelTop.Y - 26,
                            oMeter.MeterChannelTop.Width + oMeter.MeterChannelTop.X - 70,
                            25);
                    } catch {
                        return; //window too small :(
                    }
                    using (OcrInput input = new OcrInput(_bitmap, nameRect)) {
                        oMeter.MeterName = TESSERACT.Read(input).Text;
                        //OcrInput and IronOcr are like: "It's finally my time to shine..."
                    }
                    METERS.Add(oMeter);
                }
                //55 pixels off rectangle end.
                //2 pixels above meter
                //25 pixels high
            }

            //scan pixel below bottom meter

        }

        public static double? GetAudioMeterLevel(string meterName) {
            return !METER_LEVELS.ContainsKey(meterName)
                ? null
                : (double?)METER_LEVELS[meterName];
        }
        public static double? GetAudioMeterLevel(int index) {
            return METER_LEVELS_SEQ.Count > index
                ? (double?)METER_LEVELS_SEQ[index]
                : null;
        }

        internal static void RegisterCapture() {
            _listener_count++;
        }
        internal static void UnregisterCapture() {
            _listener_count--;
        }

        private static readonly Dictionary<object, ObsAudioMeterStatus> STATUSES = new Dictionary<object, ObsAudioMeterStatus>();
        private static readonly object STATUSES_LOCK = new object();

        public enum ObsAudioMeterStatus {
            Success,
            WindowTooThin,
            NoMetersFound,
            TicksNotFound,
            CouldNotFindObsProcess,
            CouldNotFindAudioMixerWindow,
            FailedToCaptureAudioMixerWindow
        }
        public static ObsAudioMeterStatus? GetStatus(object token) {
            if (!STATUSES.ContainsKey(token)) {
                return null;
            }
            ObsAudioMeterStatus ret = STATUSES[token];
            lock (STATUSES_LOCK) {
                STATUSES.Remove(ret);
            }
            return ret;
        }
        public static IEnumerable<ObsAudioMixerMeter> GetObsAudioMeters(object token, EventHandler<Image> cbCapturedImage = null) {
            Bitmap _bitmap;
            Graphics _graphics;
            #region CaptureMain
            {
                //obtain window if we don't have it.
                switch (GetAudioMixer(out IntPtr _h_audio_mixer)) {
                case -1:
                    lock (STATUSES_LOCK) {
                        STATUSES.Add(token, ObsAudioMeterStatus.CouldNotFindObsProcess);
                    }
                    yield break;
                case -2:
                    lock (STATUSES_LOCK) {
                        STATUSES.Add(token, ObsAudioMeterStatus.CouldNotFindAudioMixerWindow);
                    }
                    yield break;
                }
                //check window size.
                Size _window_size = GetWindowSize(_h_audio_mixer);
                _bitmap = new Bitmap(_window_size.Width, _window_size.Height);
                _graphics = Graphics.FromImage(_bitmap);

                //capture OBS window. (PW_CLIENTONLY specifies not to capture the window border.)
                if (!PrintWindow(_h_audio_mixer, _graphics.GetHdc(), PW_CLIENTONLY)) {
                    lock (STATUSES_LOCK) {
                        STATUSES.Add(token, ObsAudioMeterStatus.FailedToCaptureAudioMixerWindow);
                    }
                    yield break;
                }
                //send this to the debuggin context.
                cbCapturedImage?.Invoke(null, (Image)_bitmap.Clone());
#warning We are currently not checking to see if the Audio Mixer is Vertical or Horizontal...
                //OH MY GOD, THERE'S A VERTICAL LAYOUT... FUCK ME
            }
            #endregion
            #region UpdateMeters
            {
                //ObsAudioMixerMeter
                int scanLineA = (_bitmap.Width / 2) + 10; //search 20 apart
                int scanLineB = (_bitmap.Width / 2) - 10; //we do this because OBS displays two peaks. The second interrupts the usual color of the meter
                int height = _bitmap.Height;
                if (scanLineA < 0) {
                    lock (STATUSES_LOCK) {
                        STATUSES.Add(token, ObsAudioMeterStatus.WindowTooThin);
                    }
                    yield break; //window is too thin... :(
                }
                int count = 0, pAfter = 0, yStartA = 0, yEndA = 0, yStartB = 0, yEndB = 0;
                bool anyMeter = false, meter = false, startedB = false;

                string theme = null;
                Color[] match_colors = ALL_METER_COLORS;
                Color tick_color = default;
                List<Tuple<Point, Point>> coords = new List<Tuple<Point, Point>>();
                //a:start,end; b:start,end
                for (int y = 0; y < height; y++) {
                    Color a = _bitmap.GetPixel(scanLineA, y);
                    Color b = _bitmap.GetPixel(scanLineB, y);
                    if (match_colors.Contains(a) || match_colors.Contains(b)) {
                        theme = METER_COLORS.First(c => c.Value.Contains(a) || c.Value.Contains(b)).Key;
                        if (ReferenceEquals(match_colors, ALL_METER_COLORS)) {
                            //ReferenceEquals because it's quicker and we don't want to check sequences.
                            //we just care if we're checking all meter colors or not.
                            match_colors = METER_COLORS[theme];
                            tick_color = METER_TICK_COLORS[theme];
                        }
                        if (!meter && (pAfter > ObsAudioMixerMeter.MAX_PIXELS_BETWEEN_CHANNELS || !anyMeter)) {
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
                        } else if (!meter && (pAfter < ObsAudioMixerMeter.MAX_PIXELS_BETWEEN_CHANNELS)) {
                            //new channel
                            yStartB = y;
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
                        pAfter++;

                    }
                }
                if (anyMeter && yStartA > 0) {
                    coords.Add(new Tuple<Point, Point>(
                        new Point(yStartA, yEndA),
                        new Point(yStartB, yEndB)));
                }

                if (coords.Count == 0) {
                    lock (STATUSES_LOCK) {
                        STATUSES.Add(token, ObsAudioMeterStatus.NoMetersFound);
                    }
                    yield break; //found no meters... :(
                }
                {
                    Tuple<Point, Point> coord = coords[0];
                    int y = coord.Item2.X == 0
                        ? coord.Item1.Y + 1
                        : coord.Item2.Y + 1;
                    int lastX = -1;
                    int firstX = -1;
                    for (int x = _bitmap.Width - 1; x >= 0; x--) {
                        if (_bitmap.GetPixel(x, y) == tick_color) {
                            lastX = x;
                            if (firstX == -1) {
                                firstX = x;
                            }
                        }
                    }
                    if (lastX == -1) {
                        lock (STATUSES_LOCK) {
                            STATUSES.Add(token, ObsAudioMeterStatus.TicksNotFound);
                        }
                        yield break;
                        //could not find ticks somehow...? :(
                    }
                    int xSize = firstX - lastX;
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
                        Rectangle nameRect = default;
                        try {
                            nameRect = new Rectangle(
                                0,
                                oMeter.MeterChannelTop.Y - 26,
                                oMeter.MeterChannelTop.Width + oMeter.MeterChannelTop.X - 70,
                                25);
                        } catch {
                            lock (STATUSES_LOCK) {
                                STATUSES.Add(token, ObsAudioMeterStatus.WindowTooThin);
                            }
                            yield break; //window too small :(
                        }
                        using (OcrInput input = new OcrInput(_bitmap, nameRect)) {
                            oMeter.MeterName = TESSERACT.Read(input).Text;
                            //OcrInput and IronOcr are like: "It's finally my time to shine..."
                        }
                        yield return oMeter;
                    }
                    //55 pixels off rectangle end.
                    //2 pixels above meter
                    //25 pixels high
                }

                //scan pixel below bottom meter
            }
            #endregion
            lock (STATUSES_LOCK) {
                STATUSES.Add(token, ObsAudioMeterStatus.Success);
            }
            yield break;
        }
    }

    public struct ObsAudioMixerMeter {
        public const int MAX_PIXELS_BETWEEN_CHANNELS = 10;
        public string Theme { get; set; }
        public string MeterName { get; set; }
        public Rectangle MeterChannelTop { get; set; }
        public Rectangle MeterChannelBottom { get; set; }
        public bool HasDualMeter => this.MeterChannelBottom != default;
    }
}
#warning Need to hook into the events from this class in MainForm so that we can tell the user when they goof the OBS requirements.
//we should add an ability to detach those event handlers temporarily so that the AddDeviceForm can hook into and detect when the goof occurs.