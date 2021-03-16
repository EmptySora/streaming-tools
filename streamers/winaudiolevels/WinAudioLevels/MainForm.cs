using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinAudioLevels {
    public partial class MainForm : Form {

        public ApplicationSettings Settings { get; } = null;
        public LoadingForm LoadingForm { get; } = null;
        public MainForm() {
            this.InitializeComponent();
            this.Settings = ApplicationSettings.GetDefaultSettings();
            this.LoadingForm = new LoadingForm(false);
            this.FormClosed += this.MainForm_FormClosed;
            this.HandleResizeEvent(this, new EventArgs());
            ApplicationSettings.RegisterMainForm(this);
        }
        public MainForm(ApplicationSettings settings, LoadingForm loader) {
            this.InitializeComponent();
            this.Settings = settings;
            this.LoadingForm = loader;
            loader.FormBorderStyle = FormBorderStyle.FixedToolWindow; //change it to a tool window since we'll be using it for that.
            this.FormClosed += this.MainForm_FormClosed;
            //we can reuse the loading form, potentially. (eg: for other operations.
            if (this.Settings.OldFileWasLost) {
                if(MessageBox.Show(
                    string.Format(
                        "The old settings.json file was found to be corrupted. It has been moved to {0}. Would you like to open the containing directory?",
                        this.Settings.OldFileLocation),
                    "Information",
                     MessageBoxButtons.YesNo,
                      MessageBoxIcon.Information,
                       MessageBoxDefaultButton.Button1
                    ) == DialogResult.Yes) {
                    ProcessStartInfo pInfo = new ProcessStartInfo() {
                        FileName = "%windir%\\explorer.exe",
                        Arguments = string.Format(
                             "/select,\"{0}\"",
                             this.Settings.OldFileLocation)
                    };
                    Process.Start(pInfo);
                }
            }
            this.HandleResizeEvent(this,new EventArgs());
            ApplicationSettings.RegisterMainForm(this);
        }
        private void HandleResizeEvent(object sender, EventArgs e) {
            Label label = this.label1;
            Panel panel = this.contentPanel;
            label.Left = (int)((panel.Width - label.Width) / 2F);
            label.Top = (int)((panel.Height - label.Height) / 2F);
            //panel: 800, 404
            //label: 268, 100
            //266 152
        }

        private async void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            await this.Settings.SaveAsync();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e) {
            if(MessageBox.Show("Are you sure you want to exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                Application.Exit();
            }
        }

        private void DevicesToolStripMenuItem_Click(object sender, EventArgs e) {
            new Form1().ShowDialog();

        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void ArrangeDevicesToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void HelpToolStripMenuItem1_Click(object sender, EventArgs e) {

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void NativeDeviceToolStripMenuItem_Click(object sender, EventArgs e) {
            //add native audio device
        }

        private void OBSCaptureToolStripMenuItem_Click(object sender, EventArgs e) {
            //add OBS mixer audio capture.
        }
    }
}