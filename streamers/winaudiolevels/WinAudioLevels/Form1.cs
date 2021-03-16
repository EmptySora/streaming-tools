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

        private void TestOBSFontToolStripMenuItem_Click(object sender, EventArgs e) {
            new ObsTestTheme().Show();
        }

        private void TestOBSNameGenToolStripMenuItem_Click(object sender, EventArgs e) {
            string expectedID = "Jrk68psf0AM9uuGgh7Vg3Q";
            string name = "Audio Cap";
            Console.WriteLine("Checking to see if the name detection actually works...");
            Console.WriteLine("In the Acri theme, a meter named \"{0}\" should have an id of \"{1}\"", name, expectedID);
            Console.WriteLine("We have to go this way because OCR was a bust. (using IDs, I mean)");
            Console.WriteLine("However, if we know and recreate the exact method in which OBS draws the meter labels...");
            Console.WriteLine("We can just render the label the same way to get the ID.");
            string trueID = ObsTheme.ACRI.GetMeterId(name);
            Console.WriteLine("Alright... we just detected the ID... but does it match...?");
            Thread.Sleep(1000);
            Console.WriteLine("Insert dramatic pause...");
            Thread.Sleep(3000);
            Console.WriteLine("Andddddddd...");
            Thread.Sleep(3000);
            if(trueID == expectedID) {
                Console.WriteLine("THE IDS ACTUALLY MATCH! FUCK YES!");
            } else {
                Console.WriteLine("And... seems I still have more work to do... so much for acing it the first try...");
            }
            Console.WriteLine("EXPECTED: {0}\nTRUE:     {1}", expectedID, trueID);

            Console.WriteLine();
            Console.WriteLine("Testing others...");
            Console.WriteLine();
            this.TestID("Headphones Output", "T1A2c7yAjELCRqciAMIfqA");
            Console.WriteLine();
            this.TestID("Speaker Audio Output", "WW2S6OP8F0/rpBP2bGp2jA");
            Console.WriteLine();
            this.TestID("Audio Cap", "Jrk68psf0AM9uuGgh7Vg3Q");
            Console.WriteLine();
            this.TestID("Microphone Input", "jbRrAIQ/VUEePKlSv4Ou/A");
        }
        private void TestID(string name,string expectedID) {
            string trueID = ObsTheme.ACRI.GetMeterId(name);
            if (trueID == expectedID) {
                Console.WriteLine("IDs Match");
            } else {
                Console.WriteLine("IDs Don't match");
            }
            Console.WriteLine("EXPECTED, TRUE: \"{0}\" \"{1}\" ({2})", expectedID, trueID, name);
        }
    }

}
