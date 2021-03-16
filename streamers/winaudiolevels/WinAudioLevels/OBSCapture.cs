//using IronOcr;
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
using System.Collections;
using System.Security.Cryptography;

namespace WinAudioLevels {
    public static class OBSCapture {

        #region Imports
        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(
            IntPtr hWndParent,
            [MarshalAs(UnmanagedType.FunctionPtr)]WNDENUMPROC lpEnumFunc, //callback
            IntPtr lParam); //set lParam to IntPtr.zero
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
            out RECT lpRect);


        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam);
        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;
        [DllImport("user32.dll", EntryPoint = "SendMessage", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SendMessage(
            IntPtr hWnd, 
            uint Msg, 
            int wParam,
            StringBuilder lParam);
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetWindowThreadProcessId(
            IntPtr hWnd,
            out uint lpdwProcessId); //return value is the thread id, FYI
        /*
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextW(
            IntPtr hWnd,
            IntPtr lpString, //pass in a buffer.
            int nMaxCount);
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLengthW(
            IntPtr hWnd);
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassNameW(
            IntPtr hWnd,
            StringBuilder lpClassName,
            int nMaxCount);
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowLongW(
            IntPtr hWnd,
            int nIndex);
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowLongPtrW(
            IntPtr hWnd,
            int nIndex);
        const int GWL_STYLE = -16;
        const int GWL_HINSTANCE = -6;
        const int SS_ICON = 0x00000003;
        [DllImport("user32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowModuleFileNameW(
            IntPtr hWnd,
            StringBuilder pszFileName,
            int cchFileNameMax);
        [DllImport("Oleacc.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetProcessHandleFromHwnd(
            IntPtr hWnd);
        [DllImport("kernel32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(
            IntPtr hObject);
        */


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
        public static event EventHandler AudioMixerOcrStarting;
        internal static event EventHandler<Image> AudioMixerWindowCaptured;
        internal static event EventHandler<Tuple<Rectangle,int>> AudioMixerOcrAttempt; //read region, meter
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
        private static readonly MD5CryptoServiceProvider HASHER = new MD5CryptoServiceProvider();
        //key is the theme's name. value is array of colors: ggyyrr (first is light, second is dark)
        private static readonly List<ObsAudioMixerMeter> METERS = new List<ObsAudioMixerMeter>();
        private static readonly object LOCK = new object();
        private static readonly Dictionary<string, double> METER_LEVELS = new Dictionary<string, double>();
        private static readonly List<double> METER_LEVELS_SEQ = new List<double>();
        private static string GetControlText(IntPtr hWnd) {
            int size = SendMessage(hWnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();
            if(size == 0) {
                return null;
            }
            StringBuilder builder = new StringBuilder(size + 1);
            SendMessage(hWnd, WM_GETTEXT, builder.Capacity, builder);
            return builder.ToString();
        }
        public static ObsTheme? GetCurrentObsTheme() {
            if (METERS.Count > 0) {
                return METERS[0].Theme;
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
            }
            return null;
        }
        public static string GetCurrentObsThemeName() {
            if (METERS.Count > 0) {
                return METERS[0].ThemeName;
            }
            return null;
        }
        private static int GetAudioMixer(out IntPtr hAudioMixer) {
            IntPtr handle = IntPtr.Zero;
            Process obs = null;
            hAudioMixer = IntPtr.Zero;
            bool EnumCallback(IntPtr hWnd, IntPtr lParam) {
                try {
                    if ("Audio Mixer" == GetControlText(hWnd)) {
                        GetWindowThreadProcessId(hWnd, out uint procID);
                        if (procID != obs.Id) {
                            return true; //WRONG "Audio Mixer" window, dammit!
                        }
                        handle = hWnd;
                        return false;
                        //we have what we came for.
                        //let's get gone.
                    }
                } finally { }
                return true;
            }
            foreach (Process proc in Process.GetProcesses()) {
                try {
                    string path = proc.MainModule.FileName;
                    string[] dirs = Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar);
                    string file = Path.GetFileNameWithoutExtension(path).ToLower();
                    IEnumerable<Process> children;
                    if (file == "obs32" || file == "obs64") {
                        children = proc.GetChildProcesses();
                        if (children.Any(child => Path
                            .GetFileNameWithoutExtension(child.MainModule.FileName)
                            .ToLower() == "obs-browser-page")) {
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
                }catch(Exception e) {
                    //Console.WriteLine("Error during proc lookup: {0}", e.Message);
                }
            }
            if (obs == null) {
                return -1; //could not find obs... :(
            }
            //for some reason, using the mainwindowhandle value for this doesn't work...
            //since we still need the OBS process to do this right, we still need the above process logic.
            EnumChildWindows(IntPtr.Zero, EnumCallback, IntPtr.Zero); //return value unused.
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
                    Thread.Sleep(errorWait);
                } catch (EntryPointNotFoundException) {
                    Console.WriteLine("Could not find Audio Mixer window! Waiting {0} seconds...", errorWait.TotalSeconds);
                    AudioMixerWindowNotFoundError?.Invoke(null, new EventArgs());
                    _errored = true;
                    Thread.Sleep(errorWait);
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
            //FYI, this is scuffed. I think this might include the window border which is explicitly excluded from being captured...
            //At least, I think so based on the behavior of resizing the ObsTest window...
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
            try {
                if (!PrintWindow(_h_audio_mixer, _graphics.GetHdc(), PW_CLIENTONLY)) {
                    throw new Win32Exception();
                }
            } catch {
                throw;
            } finally {
                _graphics.ReleaseHdc();
            }
            if(AudioMixerWindowCaptured != null) {
                Bitmap img = new Bitmap(currentSize.Width, currentSize.Height);
                Graphics gfx = Graphics.FromImage(img);
                try {
                    if (!PrintWindow(_h_audio_mixer, gfx.GetHdc(), PW_CLIENTONLY)) {
                        throw new Win32Exception();
                    }
                } catch {
                    throw;
                } finally {
                    gfx.ReleaseHdc();
                }
                //separate print window call to avoid locking up the bitmap
                AudioMixerWindowCaptured.Invoke(null, img);
            }
            //send this to the debuggin context.
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
                    if (METERS.Count > 0) {
                        METERS.ForEach(a => {
                            METER_LEVELS[a.MeterId] = -60;
                            METER_LEVELS_SEQ.Add(-60);
                        });
                    }
                }
            }
            int index = 0;
            if (METERS.Count > 0) {
                METERS.ForEach(a => {
                    double level = GetMeterLevel(a);
                    lock (LOCK) {
                        METER_LEVELS[a.MeterId] = level;
                        METER_LEVELS_SEQ[index++] = level;
                        //update the meter levels.
                    }
                });
            }
            UpdateConsoleShowing();
#warning We are currently not checking to see if the Audio Mixer is Vertical or Horizontal...
            //OH MY GOD, THERE'S A VERTICAL LAYOUT... FUCK ME
        }
        private static void UpdateConsoleShowing() {
            Console.Title = string.Format(
                "METER LEVELS: {0}",
                string.Join(", ", METER_LEVELS_SEQ));
        }
        private static double GetMeterLevel(ObsAudioMixerMeter meter) {
            double levelA = double.MinValue;
            double levelB = double.MinValue;
            bool bTemp = false;
            Color[] colors = meter.Theme.meterColors.Where(a => bTemp = !bTemp).ToArray();
            if (meter.HasDualMeter) {
                double maxX = meter.MeterChannelBottom.X;
                for (int x = meter.MeterChannelBottom.X; x < meter.MeterChannelBottom.Right; x++) {
                    if (colors.Contains(_bitmap.GetPixel(x, meter.MeterChannelBottom.Y))) {
                        maxX = x;
                    } else if (maxX != meter.MeterChannelBottom.X) {
                        break; //avoid catching the dot thingies
                    } else {
                        break;
                    }
                }
                levelB = ((maxX - meter.MeterChannelBottom.X) / meter.MeterChannelBottom.Width * 60) - 60;
            }
            {
                double maxX = meter.MeterChannelTop.X;
                for (int x = meter.MeterChannelTop.X; x < meter.MeterChannelTop.Right; x++) {
                    if (colors.Contains(_bitmap.GetPixel(x, meter.MeterChannelTop.Y))) {
                        maxX = x;
                    } else if (maxX != meter.MeterChannelBottom.X) {
                        break; //avoid catching the dot thingies
                    } else {
                        break;
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
                if (ObsTheme.ALL_METER_COLORS.Contains(a) || ObsTheme.ALL_METER_COLORS.Contains(b)) {
                    if (!meter && (pAfter > ObsAudioMixerMeter.MAX_PIXELS_BETWEEN_CHANNELS || !anyMeter)) {
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
            ObsTheme theme = default;
            Color[] match_colors = ObsTheme.ALL_METER_COLORS;
            Color tick_color = default;
            List<Tuple<Point, Point>> coords = new List<Tuple<Point, Point>>();
            //a:start,end; b:start,end
            for (int y = 0; y < height; y++) {
                Color a = _bitmap.GetPixel(scanLineA, y);
                Color b = _bitmap.GetPixel(scanLineB, y);
                if (match_colors.Contains(a) || match_colors.Contains(b)) {
                    theme = ObsTheme.GetThemeByMeterColor(a, b);
                    if (ReferenceEquals(match_colors, ObsTheme.ALL_METER_COLORS)) {
                        //ReferenceEquals because it's quicker and we don't want to check sequences.
                        //we just care if we're checking all meter colors or not.
                        match_colors = theme.meterColors;
                        tick_color = theme.meterTickColor;
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
                int mIndex = 0;
                AudioMixerOcrStarting?.Invoke(null, new EventArgs());
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
                        Math.Min(oMeter.MeterChannelTop.Width + oMeter.MeterChannelTop.X - 70, _bitmap.Width),
                        Math.Min(25 + Math.Min(oMeter.MeterChannelTop.Y - 26, 0), _bitmap.Height));
                    //ocr rect: {0, max(0, top.Y - 26)} width = min(bmp.width, top.Width + top.X - 70), height = min(bmp.height, 25 + min(0, top.Y - 26))
                    AudioMixerOcrAttempt?.Invoke(null, new Tuple<Rectangle, int>(nameRect, mIndex));
                    oMeter.MeterId = GetMeterId(oMeter, _bitmap, nameRect);
                    Console.WriteLine("Found Meter (id): {0}", oMeter.MeterId);
                    METERS.Add(oMeter);
                    mIndex++;
                }
                //55 pixels off rectangle end.
                //2 pixels above meter
                //25 pixels high
            }

            //scan pixel below bottom meter

        }
        private static string GetMeterId(ObsAudioMixerMeter meterBase, Bitmap bmp, Rectangle rect) {
            List<List<bool>> bits = new List<List<bool>>();
            Color[] bgColors = meterBase.Theme.meterBackgroundColors;
            //step 1: convert the OCR region to bool[][] where false=bg, true=!bg
            for (int x = rect.X,i=0; x < Math.Min(rect.Right,bmp.Width); x++,i++) {
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
                if(bits[bits.Count - 1].All(a => !a)) {
                    bits.RemoveAt(bits.Count - 1);
                    check = true;
                    continue;
                }
                if(bits.All(a => !a[0])) {
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
        internal static string GetMeterId(ObsTheme theme, string name) {
            List<List<bool>> bits = new List<List<bool>>();
            Color[] bgColors = theme.meterBackgroundColors;
            Bitmap bmp = theme.RenderText(name);
            Rectangle rect = new Rectangle(default, bmp.Size);
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
        /*
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
        */
    }
}
//Need to hook into the events from this class in MainForm so that we can tell the user when they goof the OBS requirements.
//we should add an ability to detach those event handlers temporarily so that the AddDeviceForm can hook into and detect when the goof occurs.