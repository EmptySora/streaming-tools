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
/*
 * TODO:
 *   - Add in a AudioDeviceSelectDialog form (to select an audio device)
 *     - Make sure to interactively specify which type (OBS vs WASAPI) (hopefully without two to three forms)
 *     - Return should be ApplicationSettings.SettingsV0.AudioDeviceSettings
 *   - Add in a audio meter list showing all the audio meters being tested against. (maybe see if the NAudio.VolumeMeter control will work with me...)
 *     - Context menu that bitch up and allow modifying properties.
 *       - The item will open a form to edit and view the device properties.
 *     - Add in a checkbox to control if the source is captured to the servers.
 *     - Device settings window should clone the AudioDeviceSettings object and only replace it if the settings change.
 *       - Closing the window will save it (assuming DialogResult != Cancel (specify DefaultButton))
 *   - Settings form that allows configuring WebSocket servers and to start and stop servers.
 *     - Like above, clone settings and only save if DialogResult != Cancel
 *   - Actually finish the OBS capture
 *     - Add code to detect if the Audio Mixer is popped out. (if it is, our life is easy.
 *       - If not popped out display an error message.
 *     - Use Iron OCR to detect the names of the audio sources.
 *   - Add in the Form1 as an audio device explorer. (remove a bunch of the older features.)
 *   - Help and About maybe implement with cefsharp? (or is this too heavy-handed...?)
 *   - Add in a way to specify the reason why the old settings file was removed (in the ApplicationSettings bs)
 *   - Need to look into IMMNotificationClient::OnDeviceStateChanged to handle hotplugging properly so the app doesn't crash.
 *   
 *   
 *   Controls:
 *     - A horizontally scrolling container containing individual controls
 *       - Each control takes an AUDIODEVICE as an argument and using NAudio.VolumeMeter to display the peaks.
 *       - If no audio devices are added, there's an alternate control explaining to "Click here to add an audio device"
 *       - Each device should individually have a context menu. 
 *       - The properties window for audio devices has two tabs: properties, options
 * */