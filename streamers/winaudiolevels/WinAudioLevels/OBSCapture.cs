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

        //use these to hook into the error and display a warning to the user
        //BE SURE TO ONLY SHOW A MESSAGE ONCE UNTIL ERRORFIXED is HIT
        public static event EventHandler OBSNotFoundError;
        public static event EventHandler AudioMixerWindowNotFoundError;
        public static event EventHandler OBSErrorFixed;
        internal static event EventHandler<Image> AudioMixerWindowCaptured;
        private static readonly TimeSpan ERROR_WAIT = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan IDLE_WAIT = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan ACTIVE_WAIT = TimeSpan.FromMilliseconds(1000F / FPS);
        internal static Bitmap Bitmap { get; private set; }
        private static readonly Thread THREAD = new Thread(CaptureLoop) {
            Name = "OBS Capture Thread"
        };
        public const int FPS = 30;

        private static bool _errored = false;
        private static int _listener_count = 0;
        private static Size _window_size;
        private static Graphics _graphics;
        private static IntPtr _h_audio_mixer = IntPtr.Zero;
        static OBSCapture() {
            THREAD.Start();
        }

        private static string GetControlText(IntPtr hWnd) {
            int size = SendMessage(hWnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();
            if(size == 0) {
                return null;
            }
            StringBuilder builder = new StringBuilder(size + 1);
            SendMessage(hWnd, WM_GETTEXT, builder.Capacity, builder);
            return builder.ToString();
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
                    string file = Path.GetFileNameWithoutExtension(proc.MainModule.FileName).ToLower();
                    if (file == "obs32" || file == "obs64") {
                        if (proc.GetChildProcesses().Any(child => Path.GetFileNameWithoutExtension(child.MainModule.FileName).ToLower() == "obs-browser-page")) {
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
                }catch (Exception e) {
                    Console.WriteLine("Error during proc lookup: {0}", e.Message);
                }
            }
            if (obs is null) {
                return -1; //could not find obs... :(
            }
            //for some reason, using the mainwindowhandle value for this doesn't work...
            //since we still need the OBS process to do this right, we still need the above process logic.
            EnumChildWindows(IntPtr.Zero, EnumCallback, IntPtr.Zero); //return value unused.

            if ((hAudioMixer = handle) == IntPtr.Zero) {
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
            while (true) {
               TimeSpan wait = ERROR_WAIT;
                try {
                    if (_listener_count > 0) {
                        CaptureMain();
                    }
                    wait = _listener_count > 0 
                        ? ACTIVE_WAIT 
                        : IDLE_WAIT;
                } catch (ThreadAbortException) {
                    throw; //rethrow to allow abort to work.
                } catch (FileNotFoundException) {
                    Console.WriteLine("Could not find obs64.exe/obs32.exe! Waiting {0} seconds...", wait.TotalSeconds);
                    OBSNotFoundError?.Invoke(null, new EventArgs());
                    _errored = true;
                } catch (EntryPointNotFoundException) {
                    Console.WriteLine("Could not find Audio Mixer window! Waiting {0} seconds...", wait.TotalSeconds);
                    AudioMixerWindowNotFoundError?.Invoke(null, new EventArgs());
                    _errored = true;
                } catch (Exception e) {
                    Console.WriteLine("ERROR IN OBS CAPTURE LOOP:\n{0}", e);
                } finally {
                    Thread.Sleep(wait);
                }
            }
        }
        private static void CaptureMain() {
            //check if window was closed.
            if (!(_h_audio_mixer == IntPtr.Zero || IsWindow(_h_audio_mixer))) {
                _h_audio_mixer = IntPtr.Zero;
                _window_size = default;
                ObsAudioMixerMeter.ResetMeters();
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
            //if window size changed, we need to get new drawing buffers.
            if (_window_size != (_window_size = GetWindowSize(_h_audio_mixer))) {
                _graphics?.Dispose();
                Bitmap?.Dispose();
                ObsAudioMixerMeter.ResetMeters();
                Bitmap = new Bitmap(_window_size.Width, _window_size.Height);
                _graphics = Graphics.FromImage(Bitmap);
            }

            CaptureWindow(_graphics);
            if(AudioMixerWindowCaptured != null) {
                Bitmap img = new Bitmap(_window_size.Width, _window_size.Height);
                using(Graphics gfx = Graphics.FromImage(img)) {
                    CaptureWindow(gfx);
                    //separate print window call to avoid locking up the bitmap
                }
                AudioMixerWindowCaptured.Invoke(null, img);
                //do not dispose of img. clients may still be using it...
            }

            ObsAudioMixerMeter.RefreshMeterLists();

            Console.Title = string.Format(
                "METER LEVELS: {0}",
                string.Join(", ", ObsAudioMixerMeter.GetAllAudioMeterLevel()));
#warning We are currently not checking to see if the Audio Mixer is Vertical or Horizontal...
            //OH MY GOD, THERE'S A VERTICAL LAYOUT... FUCK ME
        }
        
        private static void CaptureWindow(Graphics g) {
            try {
                //capture OBS window. (PW_CLIENTONLY specifies not to capture the window border.)
                if (!PrintWindow(_h_audio_mixer, g.GetHdc(), PW_CLIENTONLY)) {
                    throw new Win32Exception();
                }
            } catch {
                throw;
            } finally {
                g.ReleaseHdc();
            }
        }
                
        internal static void RegisterCapture() {
            _listener_count++;
        }
        internal static void UnregisterCapture() {
            _listener_count--;
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