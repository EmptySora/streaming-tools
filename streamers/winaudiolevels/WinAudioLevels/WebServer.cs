//#define SECURE_WEBSOCKET
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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
}
