using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace WinAudioLevels {
    public partial class CefBrowser : Form {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        public string URL { get; private set; }
        public static void BrowserMain() {
            Application.Run(new CefBrowser());
        }
        public CefBrowser() {
            this.InitializeComponent();
            this.chromiumWebBrowser1.LoadError += this.ChromiumWebBrowser1_LoadError;
            this.chromiumWebBrowser1.FrameLoadStart += this.ChromiumWebBrowser1_FrameLoadStart;
            this.chromiumWebBrowser1.FrameLoadEnd += this.ChromiumWebBrowser1_FrameLoadEnd;
        }

        private void ChromiumWebBrowser1_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e) {
            this._dispatcher.Invoke(() => {
                this.Text = string.Format("CefBrowser[{0}] - Fully Loaded", e.Url);
            });
        }

        private void ChromiumWebBrowser1_FrameLoadStart(object sender, CefSharp.FrameLoadStartEventArgs e) {
            this._dispatcher.Invoke(() => {
                this.Text = string.Format("CefBrowser[{0}] - Loading", e.Url);
            });
        }

        private void ChromiumWebBrowser1_LoadError(object sender, CefSharp.LoadErrorEventArgs e) {
            this._dispatcher.Invoke(() => {
                this.Text = string.Format("CefBrowser[{2}] - Error: {0} ({1})", e.ErrorText, e.ErrorCode, e.FailedUrl);
            });
        }

        public void LoadURL(string URL) {
            this._dispatcher.Invoke(() => {
                this.chromiumWebBrowser1.Load(URL);
                this.URL = URL;
            });
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
            using (OpenFileDialog dialog = new OpenFileDialog() {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "HTML Files (*.htm, *.html, *.xhtml)|*.xhtml;*.html;*.htm|All Files (*.*)|*.*",
                Multiselect = false,
                FilterIndex = 0,
                Title = "Open File",
                InitialDirectory = Environment.CurrentDirectory,
                ShowHelp = false,
                ShowReadOnly = false,
                SupportMultiDottedExtensions = true,
                DereferenceLinks = false,

            }) {
                if (dialog.ShowDialog() == DialogResult.OK) {
                    string url = string.Format("file:///{0}", dialog.FileName.Replace('\\', '/'));
                    this.LoadURL(url);
                }
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e) {
            new CefBrowser().Show();
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e) {
            this.chromiumWebBrowser1.Load(this.URL);
        }

        private void OpenURLToolStripMenuItem_Click(object sender, EventArgs e) {
            while (true) {
                string result = Prompt.ShowPrompt(
                    "Enter the URL to load below.",
                    "Open URL...",
                    "https://example.com",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    out DialogResult dialogResult);
                if (dialogResult == DialogResult.OK) {
                    try {
                        new Uri(result);
                        this.LoadURL(result);
                        return;
                    } catch {
                        if (MessageBox.Show(
                            "The provided URL is invalid.",
                            "Error",
                             MessageBoxButtons.RetryCancel,
                             MessageBoxIcon.Error) != DialogResult.Retry) {
                            return;
                        }
                    }
                } else {
                    return;
                }
            }

        }
    }
}
