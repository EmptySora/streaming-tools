//#define SECURE_WEBSOCKET
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace WinAudioLevels {
    class WebServer : IDisposable {
        private WebSocketServer _server;
        private readonly ApplicationSettings.SettingsV0.WebSocketServerSettings _settings;
        private Thread _thread;
        private bool _started;
        public WebServer(ApplicationSettings.SettingsV0.WebSocketServerSettings settings) {
            this._settings = settings;
        }
        private void ServerMain() {
            this._server?.Stop();
            this._server = this._settings.Uri != null
                ? new WebSocketServer(this._settings.FullUri)
                : new WebSocketServer(this._settings.Port, this._settings.Secure);

            if (this._settings.Secure) {
                this._server.SslConfiguration.EnabledSslProtocols = this._settings.SslEnabledProtocols;
                this._server.SslConfiguration.CheckCertificateRevocation = this._settings.SslCheckCertificateRevocation;
                this._server.SslConfiguration.ClientCertificateRequired = this._settings.SslRequireClientCertificate;
                Console.WriteLine("Loading SSL certificate.");
                X509Certificate certificate = X509Certificate.CreateFromCertFile(this._settings.SslServerCertificatePath);
                this._server.SslConfiguration.ServerCertificate = new X509Certificate2(certificate);
            }
            Console.WriteLine("Created WebSocketServer");
            this._server.WebSocketServices.AddService<AudioPeaksBehavior>("/AudioPeaks", AudioPeaksBehavior.Initializer);
            Console.WriteLine("Added /AudioPeaks/ behavior");
            this._server.Start();
            Console.WriteLine("Started server");
        }

        public void Start() {
            if (this._started) {
                throw new InvalidOperationException("The server is already running!");
            }
            if (this._server != null) {
                this._server.Start();
                this._started = true;
            } else {
                this._thread = new Thread(this.ServerMain) { Name = "WebSocket Server thread." };
                Console.WriteLine("Creating Thread: {0} ({1})", this._thread.Name, this._thread.ManagedThreadId);
                this._thread.Start();
            }
        }
        public void Stop() {
            if (!this._started) {
                throw new InvalidOperationException("The server is not running!");
            }
            this._server?.Stop();
        }


        public const int SERVER_PORT = 8069; //8080 & 6969 mixed
        /*
        private static void ServerMain() {
            
#if SECURE_WEBSOCKET
            WebSocketServer server = new WebSocketServer(string.Format("wss://localhost:{0}",SERVER_PORT));
            server.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
            server.SslConfiguration.ClientCertificateRequired = false;
            Console.WriteLine("Loading SSL certificate.");
            X509Certificate certificate = X509Certificate.CreateFromCertFile("domain.crt");
            server.SslConfiguration.ServerCertificate = new X509Certificate2(certificate);
#else
            WebSocketServer server = new WebSocketServer(string.Format("ws://localhost:{0}",SERVER_PORT));
#endif
            //server.SslConfiguration.CheckCertificateRevocation
            Console.WriteLine("Created WebSocketServer");
            //server.AddWebSocketService("/AudioPeaks", () => new TestBehavior());
            
            server.WebSocketServices.AddService<AudioPeaksBehavior>("/AudioPeaks", AudioPeaksBehavior.Initializer);
            Console.WriteLine("Added /AudioPeaks/ behavior");
            server.Start();
            Console.WriteLine("Started server");
        }
        */

        #region IDisposable Support
        private bool _disposed_value = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!this._disposed_value) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                    this._server?.Stop();
                    this._thread.Abort(); //if we're somehow not already out of that thread...
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                this._disposed_value = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WebServer()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
    enum AudioPeakMessageType {
        Ping,
        Peaks
    }
    enum AudioPeakMessageStatus {
        Success,
        Info,
        Error
    }
    class AudioPeakMessage {
        [JsonIgnore()]
        public AudioPeakMessageStatus Status = AudioPeakMessageStatus.Success;
        [JsonProperty(PropertyName = "status")]
        public string StatusTextual => Enum.GetName(typeof(AudioPeakMessageStatus), this.Status);
        [JsonProperty(PropertyName = "type")]
        [JsonIgnore]
        public string TypeTextual => Enum.GetName(typeof(AudioPeakMessageType), this.Type);
        [JsonIgnore()]
        public AudioPeakMessageType Type = AudioPeakMessageType.Peaks;
        [JsonProperty(PropertyName = "serverTime")]
        [JsonIgnore]
        public DateTime ServerTime = DateTime.UtcNow;
        [JsonProperty(PropertyName = "data")]
        public object Data; //Exception if Status

        public class AudioPeaks {
            [JsonProperty(PropertyName = "peaks")]
            public double[] Peaks = new double[0];
            [JsonProperty(PropertyName = "max")]
            public double Maximum => this.Peaks.Length == 0 ? 0 : this.Peaks.Max();
            [JsonProperty(PropertyName = "min")]
            public double Minimum => this.Peaks.Length == 0 ? 0 : this.Peaks.Min();
            [JsonProperty(PropertyName = "avg")]
            public double Average => this.Peaks.Length == 0 ? 0 : this.Peaks.Average();
        }

        public static AudioPeakMessage NewPing() {
            return new AudioPeakMessage() {
                Type = AudioPeakMessageType.Ping
            };
        }
        public static AudioPeakMessage NewPeaks(params double[] peaks) {
            return new AudioPeakMessage() {
                Data = new AudioPeaks() {
                    Peaks = peaks
                }
            };
        }
    }
    class AudioPeaksBehavior : WebSocketBehavior {
        private const float FPS = 60;
        private const float SLEEP_TIME = 1000 / FPS;
        private static readonly List<AudioPeaksBehavior> BEHAVIORS = new List<AudioPeaksBehavior>();
        internal static ApplicationSettings _settings = ApplicationSettings.GetDefaultSettings();
        private static bool _started = false;
        static AudioPeaksBehavior() {
            _settings.Settings.Devices = new ApplicationSettings.SettingsV0.AudioDeviceSettings[] {
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsName = "T1A2c7yAjELCRqciAMIfqA" //Headphones Output
                },
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsName = "WW2S6OP8F0/rpBP2bGp2jA" //Speaker Audio Output
                },
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsName = "Jrk68psf0AM9uuGgh7Vg3Q" //Audio Cap
                },
                new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                     ObsName = "jbRrAIQ/VUEePKlSv4Ou/A" //Microphone Input
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
                string json = JsonConvert.SerializeObject(message,settings);
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
