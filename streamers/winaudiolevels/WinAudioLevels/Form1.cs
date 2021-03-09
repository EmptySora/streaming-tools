using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;

namespace WinAudioLevels {
    public partial class Form1 : Form {
        private SoundAudioCapture[] _clients = null;
        public Form1() {
            this.InitializeComponent();
            this.Load += this.Form1_Load;
            this.FormClosed += this.Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            this.QuitToolStripMenuItem_Click(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.LoadAudioDevices();
            this.TestingCode();
        }

        private void LoadAudioDevices() {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            MMDeviceCollection devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All);
            TreeView view = this.treeView1;
            view.BeginUpdate();
            view.Nodes.Clear();
            TreeNode root = new TreeNode("Audio Devices Root") {
                Tag = null
            };
            view.Nodes.Add(root);
            int devNum = 0;
            root.Nodes.AddRange(devices.Select(device => {
                AudioDeviceProperties properties = device;
                TreeNode node =  new TreeNode(string.Format("Device {0}: \"{1}\"",++devNum, properties.FriendlyName ?? properties.DeviceFriendlyName ?? device.ID)) {
                    Tag = properties
                };
                TreeNode sessionRoot = new TreeNode("Audio Sessions") {
                    Tag = null
                };
                //add in additional subnodes for enumerable properties like the DeviceTopology crap.
                try {
                    SessionCollection collection = device.AudioSessionManager.Sessions;
                    for (int i = 0; i < collection.Count; i++) {
                        AudioControlProperties props = collection[i];
                        sessionRoot.Nodes.Add(new TreeNode(string.Format("Session {0}: \"{1}\"", i + 1, props.AudioControlDisplayName ?? "<Unknown>")) {
                            Tag = props
                        });
                    }
                    node.Nodes.Add(sessionRoot);
                } catch { }
                return node;
            }).ToArray());
            view.Nodes[0].Expand();
            //view.ExpandAll();
            view.EndUpdate();
            //WasapiLoopbackCapture loopback = new WasapiLoopbackCapture();
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            this.propertyGrid1.SelectedObject = ((TreeView)sender).SelectedNode.Tag;
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e) {
            this.LoadAudioDevices();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void NewBrowserToolStripMenuItem_Click(object sender, EventArgs e) {
            new CefBrowser().Show();
        }

        private void TestCaptureToolStripMenuItem_Click(object sender, EventArgs e) {
            if(this._clients != null) {
                foreach (SoundAudioCapture captureClient in this._clients) {
                    captureClient.Stop();
                }
            }
            SoundAudioCapture[] captureClients = SoundAudioCapture.CaptureAllAudio();
            foreach(SoundAudioCapture captureClient in captureClients) {
                captureClient.Start();
            }
            this._clients = captureClients;
        }


        public void TestingCode() {
            ApplicationSettings.SettingsV0.WebSocketServerSettings serverSettings = ApplicationSettings.GetDefaultSettings().Settings.Servers[0];
            WebServer server = new WebServer(serverSettings);
            server.Start();
        }

        private void TestOBSToolStripMenuItem_Click(object sender, EventArgs e) {
            new ObsTest().Show();
        }
    }

}
