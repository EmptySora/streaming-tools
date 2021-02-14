using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace WinAudioLevels {
    public partial class LoadingForm : Form {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private event EventHandler DoneInitializing;
        public LoadingForm(bool appStart = true) {
            this.InitializeComponent();
            if (appStart) {
                this.ProgressMax = TOTAL_INITIALIZATION_STEPS;
                new Thread(this.Initialization) { Name = "Initialization Thread" }.Start();
                this.DoneInitializing += this.LoadingForm_DoneInitializing;
            }
        }

        private void LoadingForm_DoneInitializing(object sender, EventArgs e) {
            if(Thread.CurrentThread != this._dispatcher.Thread) {
                this._dispatcher.InvokeAsync(() => this.LoadingForm_DoneInitializing(sender, e));
                return;
            }
            this.Hide();
            new MainForm(this._settings, this).Show();
        }

        private string LoadingText {
            set => this._dispatcher.Invoke(() => this.loadingLabel.Text = value);
        }
        private int Progress {
            set => this._dispatcher.Invoke(() => this.progressBar1.Value = value);
        }
        private int ProgressMax {
            set => this._dispatcher.Invoke(() => this.progressBar1.Maximum = value);
        }
        //min=0, max=100
        private ApplicationSettings _settings = null;

        private const int TOTAL_INITIALIZATION_STEPS = 2;
        //Step 1: Load/Create Settings.
        //Step 2: Start WebSocket servers.
        private async void Initialization() {
            int step = this.Progress = 0;
            #region Step 1: Load settings from settings.json
            this.LoadingText = string.Format(
                "[{0} of {1}] Loading <settings.json>...",
                step,
                TOTAL_INITIALIZATION_STEPS);
            ApplicationSettings settings = await ApplicationSettings.LoadOrDefaultAsync();
            this._settings = settings;
            this.Progress = ++step;
            #endregion
            #region Step 2: Start WebSocket Servers.
            this.LoadingText = string.Format(
                "[{0} of {1}] Starting WebSocket servers...",
                step,
                TOTAL_INITIALIZATION_STEPS);
            WebServer[] servers = settings.Settings.Servers.Select(server => {
                WebServer s = new WebServer(server);
                if (server.Enabled) {
                    s.Start();
                }
                return s;
            }).ToArray();
            settings.LoadingObject = servers;
            this.Progress = ++step;
            #endregion
            //use settings.LoadingObject to transfer other data.



            DoneInitializing?.Invoke(this, new EventArgs());
        }
    }
}
