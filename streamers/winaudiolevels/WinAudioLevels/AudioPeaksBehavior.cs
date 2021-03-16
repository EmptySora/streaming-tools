using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WinAudioLevels {
    class AudioPeaksBehavior : WebSocketBehavior {
        private const float FPS = 60;
        private const float SLEEP_TIME = 1000 / FPS;
        private static readonly List<AudioPeaksBehavior> BEHAVIORS = new List<AudioPeaksBehavior>();
        internal static ApplicationSettings _settings = ApplicationSettings.GetDefaultSettings();
        private static bool _started = false;
        static AudioPeaksBehavior() {
            _settings.Settings.Devices = new ApplicationSettings.SettingsV0.AudioDeviceSettings[] {
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsId = "T1A2c7yAjELCRqciAMIfqA", //Headphones Output
                     ObsName = "Headphones Output",
                },
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsId = "WW2S6OP8F0/rpBP2bGp2jA", //Speaker Audio Output
                     ObsName = "Speaker Audio Output",
                },
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsId = "Jrk68psf0AM9uuGgh7Vg3Q", //Audio Cap
                     ObsName = "Audio Cap",
                },
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsId = "jbRrAIQ/VUEePKlSv4Ou/A", //Microphone Input
                     ObsName = "Microphone Input"
                }
            };
            Thread t = new Thread(PingMain) { Name = "WebSocketServer Pinger" };
            t.Start();
        }
        public static void StartPing() {
            if (_started) {
                return;
            }
            _started = true;

        }
        private static void PingMain() {
            JsonSerializerSettings settings = new JsonSerializerSettings() {
                Formatting = Formatting.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            };
            AudioCapture capture = AudioCapture.GetOutputPlusSettingsCapture(_settings.Settings);
            capture.Start(); //START THE CAPTURE, MORON.
            while (true) {
                Thread.Sleep(TimeSpan.FromMilliseconds(SLEEP_TIME)); //SLEEP_TIME (25/1.5= ~17 milliseconds)
                AudioPeakMessage message = AudioPeakMessage.NewPeaks(capture.LastAmplitudePercents.ToArray());
                string json = JsonConvert.SerializeObject(message, settings);
                for (int i = 0; i < BEHAVIORS.Count; i++) {
                    AudioPeaksBehavior behavior = BEHAVIORS[i];
                    if (behavior.State != WebSocketState.Open) {
                        BEHAVIORS.RemoveAt(i);
                        i--;
                        continue;
                    }
                    behavior.Send(json);
                }
            }
        }

        public AudioPeaksBehavior()
            : base() {
            Console.WriteLine("=====================================================");
            Console.WriteLine("CREATED NEW TESTBEHAVIOR OBJECT!");
        }
        protected override void OnOpen() {
            base.OnOpen();
            Console.WriteLine("=====================================================");
            Console.WriteLine("CLIENT CONNECTED TO WEBSOCKET SERVER!");
            Console.WriteLine("Remote Endpoint: {0}", this.Context.UserEndPoint);
        }
        protected override void OnClose(CloseEventArgs e) {
            Console.WriteLine("=====================================================");
            Console.WriteLine("CLIENT DISCONNECTED FROM WEBSOCKET SERVER!");
            Console.WriteLine(
                "Details:\n" +
                "    Code: {0}\n" +
                "    Reason: {1}\n" +
                "    Was Clean?: {2}", e.Code, e.Reason, e.WasClean);
            base.OnClose(e);
        }
        protected override void OnError(ErrorEventArgs e) {
            base.OnError(e);
            Console.WriteLine("=====================================================");
            Console.WriteLine("WEBSOCKET SERVER ERROR!");
            Console.WriteLine("Remote Endpoint: {0}", this.Context.UserEndPoint);
            Console.WriteLine(
                "Exception:\n" +
                "\n" +
                "{0}", e.Exception);
        }
        protected override void OnMessage(MessageEventArgs e) {
            base.OnMessage(e);
            Console.WriteLine("=====================================================");
            Console.WriteLine("CLIENT DISCONNECTED FROM WEBSOCKET SERVER!");
            Console.WriteLine("Remote Endpoint: {0}", this.Context.UserEndPoint);
            Console.WriteLine(
                "Details:\n" +
                "    Data: {0}\n" +
                "    Is Binary?: {1}\n" +
                "    Is Ping?: {2}\n" +
                "    Is Text?: {3}", e.Data, e.IsBinary, e.IsPing, e.IsText);
        }

        public static void Initializer(AudioPeaksBehavior behavior) {
            if (!BEHAVIORS.Contains(behavior)) {
                BEHAVIORS.Add(behavior);
            }
            Console.WriteLine("=====================================================");
            Console.WriteLine("Created new TestBehavior object!");
        }
    }
}
