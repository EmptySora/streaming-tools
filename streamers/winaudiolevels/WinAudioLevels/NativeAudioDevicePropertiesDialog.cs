using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinAudioLevels {
    public partial class NativeAudioDevicePropertiesDialog : Form {
        private string _device;
        public string DeviceId {
            get => this._device;
            set {
                using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator()) {
                    try {
                        this.devicePropertyGrid.SelectedObject = (AudioDeviceProperties)enumerator.GetDevice(value);
                    } catch {
                        throw new Exception("Could not find device with ID: " + value);
                    }
                }
                this._device = value;
            }
        }
        public NativeAudioDevicePropertiesDialog() {
            this.InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
