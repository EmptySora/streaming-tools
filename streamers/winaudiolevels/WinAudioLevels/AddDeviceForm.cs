using NAudio.CoreAudioApi;
using NAudio.Wave;
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
    public partial class AddDeviceForm : Form {
        private const string TOOLTIP_FLOW_CAPTURE = "Includes devices like microphones or other audio inputs.";
        private const string TOOLTIP_FLOW_RENDER = "Includes devices like headphones or speakers.";
        private const string TOOLTIP_STATE_ACTIVE = "Includes devices that are plugged-in, enabled, and present.";
        private const string TOOLTIP_STATE_DISABLED = "Includes devices that are disabled through the system audio settings panel.";
        private const string TOOLTIP_STATE_NOT_PRESENT = "Includes devices that are disabled through Device Manager or that are phsyically disconnected.";
        private const string TOOLTIP_STATE_UNPLUGGED = "Includes devices that are enabled and physically connected, but don't have an audio device connected to them. (eg: a headphone jack when headphones aren't plugged in)";
        private static readonly string[] FORCE_ENABLED_COLUMNS = new string[] {
            "deviceIconColumn", "deviceNameColumn", "deviceTypeColumn", "deviceStatusColumn"
        };
        private ApplicationSettings Settings => ApplicationSettings.CurrentSettings;
        private delegate DataGridViewCell ColumnValueDelegate(MMDevice device, AudioDeviceProperties properties);
        private static readonly Dictionary<string, ColumnValueDelegate> COLUMN_DELEGATES = new Dictionary<string, ColumnValueDelegate>() {
            //REQUIRED COLUMNS
            { "deviceIconColumn", ColumnDelegate_Icon },
            { "deviceNameColumn", ColumnDelegate_Name },
            { "deviceTypeColumn", ColumnDelegate_Type },
            { "deviceStatusColumn", ColumnDelegate_Status },
            { "deviceIdColumn", ColumnDelegate_ID },
            { "deviceInstanceIdColumn", ColumnDelegate_InstanceID },

            { "deviceDeviceFriendlyNameColumn", ColumnDelegate_DeviceFriendlyName },
            { "deviceFriendlyNameColumn", ColumnDelegate_FriendlyName },
            { "deviceAudioSessionCountColumn", ColumnDelegate_AudioSessionCount },
            { "deviceAudioMeterMasterPeakColumn", ColumnDelegate_AudioMeterMasterPeak },
            { "deviceAudioVolumeMasterVolumeColumn", ColumnDelegate_AudioVolumeMasterVolume },
            { "deviceAudioVolumeMasterVolumeScalarColumn", ColumnDelegate_AudioVolumeMasterVolumeScalar },
            { "deviceAudioVolumeMuteColumn", ColumnDelegate_AudioVolumeMute },
            { "deviceAudioVolumeRangeMaxdBColumn", ColumnDelegate_AudioVolumeRangeMaxdB },
            { "deviceAudioVolumeRangeMindBColumn", ColumnDelegate_AudioVolumeRangeMindB },
            { "deviceAudioVolumeRangeIncrementdBColumn", ColumnDelegate_AudioVolumeRangeIncrementdB },
            { "deviceAudioVolumeStepColumn", ColumnDelegate_AudioVolumeStep },
            { "deviceAudioVolumeStepCountColumn", ColumnDelegate_AudioVolumeStepCount },
            { "deviceAudioClientMixFormatAvgBytesPerSecondColumn", ColumnDelegate_AudioClientMixFormatAvgBytesPerSecond },
            { "deviceAudioClientMixFormatBitsPerSampleColumn", ColumnDelegate_AudioClientMixFormatBitsPerSample },
            { "deviceAudioClientMixFormatBlockAlignColumn", ColumnDelegate_AudioClientMixFormatBlockAlign },
            { "deviceAudioClientMixFormatChannelCountColumn", ColumnDelegate_AudioClientMixFormatChannelCount },
            { "deviceAudioClientMixFormatEncodingColumn", ColumnDelegate_AudioClientMixFormatEncoding },
            { "deviceAudioClientMixFormatExtraSizeColumn", ColumnDelegate_AudioClientMixFormatExtraSize },
            { "deviceAudioClientMixFormatSampleRateColumn", ColumnDelegate_AudioClientMixFormatSampleRate },
            
            //OPTIONAL COLUMNS
        };
        //ID, InstanceID, DeviceFriendlyName, FriendlyName, AudioSessionCount, AudioMeterMasterPeak, AudioVolumeMasterVolume, AudioVolumeMasterVolumeScalar
        //AudioVolumeMute, AudioVolumeRangeMaxdB, AudioVolumeRangeMindB, AudioVolumeRangeIncrementdB, AudioVolumeStep, AudioVolumeStepCount,
        //AudioClientMixFormatAvgBytesPerSecond, AudioClientMixFormatBitsPerSample, AudioClientMixFormatBlockAlign, AudioClientMixFormatChannelCount,
        //AudioClientMixFormatEncoding, AudioClientMixFormatExtraSize, AudioClientMixFormatSampleRate
        
        #region Column Value Delegates
        //device*Column: Icon Name Type Status
        private static DataGridViewCell ColumnDelegate_AudioClientMixFormatEncoding(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioClientMixFormatEncoding.HasValue ? Enum.GetName(typeof(WaveFormatEncoding), properties.AudioClientMixFormatEncoding.Value) : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioClientMixFormatSampleRate(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioClientMixFormatSampleRate.HasValue ? properties.AudioClientMixFormatSampleRate.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioClientMixFormatExtraSize(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioClientMixFormatExtraSize.HasValue ? properties.AudioClientMixFormatExtraSize.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioClientMixFormatChannelCount(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioClientMixFormatChannelCount.HasValue ? properties.AudioClientMixFormatChannelCount.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioClientMixFormatBlockAlign(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioClientMixFormatBlockAlign.HasValue ? properties.AudioClientMixFormatBlockAlign.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioClientMixFormatBitsPerSample(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioClientMixFormatBitsPerSample.HasValue ? properties.AudioClientMixFormatBitsPerSample.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioClientMixFormatAvgBytesPerSecond(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioClientMixFormatAvgBytesPerSecond.HasValue ? properties.AudioClientMixFormatAvgBytesPerSecond.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeStepCount(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeStepCount.HasValue ? properties.AudioVolumeStepCount.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeStep(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeStep.HasValue ? properties.AudioVolumeStep.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeRangeIncrementdB(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeRangeIncrementdB.HasValue ? properties.AudioVolumeRangeIncrementdB.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeRangeMindB(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeRangeMindB.HasValue ? properties.AudioVolumeRangeMindB.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeRangeMaxdB(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeRangeMaxdB.HasValue ? properties.AudioVolumeRangeMaxdB.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeMute(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeMute.HasValue ? properties.AudioVolumeMute.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeMasterVolumeScalar(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeMasterVolumeScalar.HasValue ? properties.AudioVolumeMasterVolumeScalar.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioVolumeMasterVolume(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioVolumeMasterVolume.HasValue ? properties.AudioVolumeMasterVolume.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_FriendlyName(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.FriendlyName);
        }
        private static DataGridViewCell ColumnDelegate_AudioSessionCount(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioSessionCount.HasValue ? properties.AudioSessionCount.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_AudioMeterMasterPeak(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.AudioMeterMasterPeak.HasValue ? properties.AudioMeterMasterPeak.Value + "" : "");
        }
        private static DataGridViewCell ColumnDelegate_DeviceFriendlyName(MMDevice _, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.DeviceFriendlyName);
        }

        private static DataGridViewCell ColumnDelegate_ID(MMDevice device, AudioDeviceProperties properties) {
            return DataGridTextBox(device.ID);
        }
        private static DataGridViewCell ColumnDelegate_InstanceID(MMDevice device, AudioDeviceProperties properties) {
            return DataGridTextBox(device.InstanceId);
        }
        private static DataGridViewCell ColumnDelegate_Icon(MMDevice device, AudioDeviceProperties properties) {
            //Image.
            return DataGridImage(properties.IconPath); //this is a resource path. we need the code to load resources from files.
            //We should likely return a Bitmap, not a string.
        }
        private static DataGridViewCell ColumnDelegate_Name(MMDevice device, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.FriendlyName ?? properties.DeviceFriendlyName ?? device.ID);
        }
        private static DataGridViewCell ColumnDelegate_Type(MMDevice device, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.DataFlow.HasValue
                    ? Enum.GetName(typeof(DataFlow), properties.DataFlow.Value)
                    : "[#U/N]");
        }
        private static DataGridViewCell ColumnDelegate_Status(MMDevice device, AudioDeviceProperties properties) {
            return DataGridTextBox(properties.State.HasValue
                    ? Enum.GetName(typeof(DeviceState), properties.State.Value)
                    : "[#U/N]");
        }
        private static DataGridViewCell DataGridTextBox(string text) {
            return new DataGridViewTextBoxCell() {
                Value = text
            };
        }
        private static DataGridViewCell DataGridImage(string resource) {
            try {
                return new DataGridViewImageCell() {
                    Value = IconTools.IconUtil.IconFromResourcePath(resource),
                    ValueIsIcon = true
                };
            } catch (FileNotFoundException) {
                Console.WriteLine("Could not find file: {0}!", resource);
                return new DataGridViewImageCell() {
                    Value = null //file not found.
                };
            } catch (FileFormatException) {
                //not an icon.
                return new DataGridViewImageCell() {
                    Value = Image.FromFile(resource),
                    ValueIsIcon = false
                };
            }
            /*
            DOES FILE.EXIST CARE ABOUT RESOURCE PATHS?
                WinAudioLevels.exe... EXISTS
                WinAudioLevels.exe,-145... DOES NOT EXIST
            */
        }
        #endregion
        private readonly Dispatcher _dispather = Dispatcher.CurrentDispatcher;
        private Thread _loading_thread = null;
        private event EventHandler<string[]> NativeDeviceSearchFinished;
        private event EventHandler<Exception> NativeDeviceSearchErrored;
        private event EventHandler<Exception> NativeDeviceSearchFailed;
        private const int MAX_FAILURES_BEFORE_GIVE_UP = 10;
        private bool _search_requested = false;
        private readonly string[] _native_enabled_columns = new string[0];
        private bool _native_disabled = false;
        private bool _obs_disabled = false;

        /// <summary>
        /// Creates a new <see cref="AddDeviceForm"/>. If <paramref name="native"/> is <c>true</c>, the "Native" tab will be displayed.
        /// Otherwise, the "OBS" tab will be displayed.
        /// </summary>
        /// <param name="native">Whether or not the "Native" (vs "OBS") tab should be first opened.</param>
        public AddDeviceForm(bool native = true) {
            this.InitializeComponent();
            this.tabControl1.SelectedTab = native ? this.nativeTab : this.obsTab;

            this.deviceSearchToolTip.SetToolTip(this.deviceFlowCaptureCheckbox, TOOLTIP_FLOW_CAPTURE);
            this.deviceSearchToolTip.SetToolTip(this.deviceFlowRenderCheckbox, TOOLTIP_FLOW_RENDER);
            this.deviceSearchToolTip.SetToolTip(this.deviceStateActiveCheckbox, TOOLTIP_STATE_ACTIVE);
            this.deviceSearchToolTip.SetToolTip(this.deviceStateDisabledCheckbox, TOOLTIP_STATE_DISABLED);
            this.deviceSearchToolTip.SetToolTip(this.deviceStateNotPresentCheckbox, TOOLTIP_STATE_NOT_PRESENT);
            this.deviceSearchToolTip.SetToolTip(this.deviceStateUnpluggedCheckbox, TOOLTIP_STATE_UNPLUGGED);
            this.NativeDeviceSearchFinished += this.AddDeviceForm_NativeDeviceSearchFinished;
            this.NativeDeviceSearchErrored += this.AddDeviceForm_NativeDeviceSearchErrored;
            this.NativeDeviceSearchFailed += this.AddDeviceForm_NativeDeviceSearchFailed;
            this._loading_thread = new Thread(this.LoadingMain) { Name = "Device Loader Thread" };
            this._loading_thread.Start();
            this._native_enabled_columns = (string[])this.Settings.Settings.AddDeviceForm_Native_EnabledColumns.Clone();
            this.RefreshNativeDataGridViewColumns();
        }


        private void RefreshNativeDataGridViewColumns() {
            IEnumerable<string> enabledColumns = this._native_enabled_columns.Union(FORCE_ENABLED_COLUMNS);
            IEnumerable<string> disabledColumns = this.dataGridView1.Columns.Cast<DataGridViewColumn>().Select(a => a.Name).Except(enabledColumns);
            foreach (string column in enabledColumns) {
                this.dataGridView1.Columns[column].Visible = true;
            }
            foreach (string column in disabledColumns) {
                this.dataGridView1.Columns[column].Visible = false;
            }
        }

        private bool NativeSearchFormDisabled {
            set {
                this.deviceSearchButton.Enabled =
                  this.deviceFlowCaptureCheckbox.Enabled =
                  this.deviceFlowRenderCheckbox.Enabled =
                  this.deviceStateActiveCheckbox.Enabled =
                  this.deviceStateDisabledCheckbox.Enabled =
                  this.deviceStateNotPresentCheckbox.Enabled =
                  this.deviceStateUnpluggedCheckbox.Enabled =
                  this.deviceSearchSelectColumnsButton.Enabled = !value;
                this.deviceSearchButton.Text = value
                    ? "Searching..."
                    : "Search";
                this._native_disabled = value;
                if (value) {
                    this.okButton.Enabled = !value;
                }
            }
            get => this._native_disabled;
        }
        private bool OBSSearchFormDisabled {
            set { }
            get => this._obs_disabled;
        }

        private void AddDeviceForm_NativeDeviceSearchFailed(object sender, Exception e) {
            if (Thread.CurrentThread != this._dispather.Thread) {
                this._dispather.InvokeAsync(() => {
                    this.AddDeviceForm_NativeDeviceSearchFailed(sender, e);
                });
                return;
            }
            if(MessageBox.Show(
                string.Format("An error occurred while searching for native audio devices. Would you like to try again? ({0})", e),
                "Error",
                MessageBoxButtons.RetryCancel,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1) == DialogResult.Retry) {
                this._search_requested = true;
            } else {
                this.NativeSearchFormDisabled = false;
            }

        }

        private void AddDeviceForm_NativeDeviceSearchErrored(object sender, Exception e) {
            Console.WriteLine("Error while searching for native devices:\n{0}", e);
            //do nothing.
        }

        private void AddDeviceForm_NativeDeviceSearchFinished(object sender, string[] e) {
            if (Thread.CurrentThread != this._dispather.Thread) {
                this._dispather.InvokeAsync(() => {
                    this.AddDeviceForm_NativeDeviceSearchFinished(sender, e);
                });
                return;
            }
            this.NativeSearchFormDisabled = false;
            if(e.Length == 0) {
                MessageBox.Show(
                    "The search returned zero results! Select a broader criteria or check your audio settings/device manager to see if you may have disabled the device in question.",
                    "No Results!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (string deviceId in e) {
                MMDevice device = GetDeviceFromId(deviceId);
                AudioDeviceProperties properties = device;
                DataGridViewCell[] values = this.GetRowValues(device, properties);
                DataGridViewRow row = new DataGridViewRow {
                    Tag = properties //keep this object there so the context menu works.
                };
                row.Cells.AddRange(values);
                rows.Add(row);
            }
            this.dataGridView1.Rows.AddRange(rows.ToArray());
        }
        private DataGridViewCell[] GetRowValues(MMDevice device, AudioDeviceProperties properties) {
            DataGridViewColumnCollection columns = this.dataGridView1.Columns;
            List<DataGridViewCell> values = new List<DataGridViewCell>();
            foreach(DataGridViewColumn column in columns) {
                if (COLUMN_DELEGATES.ContainsKey(column.Name)) {
                    values.Add(COLUMN_DELEGATES[column.Name].Invoke(device, properties));
                } else {
                    throw new Exception("Missing column delegate for column: " + column.Name + "!");
                }
            }
            return values.ToArray();
        }
        private void TabControl1_TabIndexChanged(object sender, EventArgs e) {
            this.okButton.Enabled = this.ShouldTheOKButtonBeEnabled;
            this._loading_thread.Abort();
            this._search_requested = false;
            this._loading_thread = new Thread(this.LoadingMain) { Name = "Device Loader Thread" };
            this._loading_thread.Start();
            if(this.tabControl1.SelectedTab == this.nativeTab) {
                this.OBSSearchFormDisabled = false;
            } else {
                this.NativeSearchFormDisabled = false;
            }
            //restart loader thread to cancel ongoing transactions.
        }
        private static MMDevice GetDeviceFromId(string id) {
            using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator()) {
                return enumerator.GetDevice(id);
            }
        }
        private void LoadingMain() {
            int failures = 0;
            while (true) {
                try {
                    Thread.Sleep(1000);
                    if (!this._search_requested) {
                        failures = 0;
                        continue; // no search is requested, soooooooo... sleep.
                    }
                    using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator()) {
                        MMDevice[] collection = enumerator.EnumerateAudioEndPoints(this.UserSelectedDataFlow, this.UserSelectedDeviceState).ToArray();
                        this.NativeDeviceSearchFinished?.Invoke(this, collection.Select(a=>a.ID).ToArray());
                        foreach(MMDevice device in collection) {
                            device.Dispose(); //dispose of everything.
                        }
                        //we cannot return MMDevice objects because COM is not thread-safe. Accessing the objects would cause COMExceptions to be raised.
                        //send the device ID instead.
                    }
                    failures = 0;
                    this._search_requested = false;
                } catch (ThreadAbortException) {
                    throw; //Re throw ThreadAbort or else aborting this thread will not work at all.
                } catch (Exception e) {
                    this.NativeDeviceSearchErrored?.Invoke(this, e);
                    failures++;
                    if(failures > MAX_FAILURES_BEFORE_GIVE_UP) {
                        this._search_requested = false;
                        failures = 0;
                        this.NativeDeviceSearchFailed?.Invoke(this, e);
                    }
                }
            }
        }
        private void PropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
            //open a AudioDeviceProperties dialog with the selected device.
            if(this.dataGridView1.SelectedRows.Count == 0) {
                return;
            }
            using (NativeAudioDevicePropertiesDialog dialog = new NativeAudioDevicePropertiesDialog() {
                DeviceId = ((MMDevice)this.dataGridView1.SelectedRows[0].Tag).ID
            }) {
                dialog.ShowDialog();
            }

        }
        //options:
        //    Type: All, Capture, Render
        //    State: All, Disabled, Unplugged, Not Present, Active
        //columns:
        //    Device Icon, Name, Type, Status, ...
        private bool ShouldTheOKButtonBeEnabled {
            get {
                if (this.tabControl1.SelectedTab == this.nativeTab) {
                    //native logic
                } else {
                    //OBS logic
                }
                return false;
            }
        }

        private DeviceState UserSelectedDeviceState => this._dispather.Invoke(()=>(DeviceState)(
                    (this.deviceStateActiveCheckbox.Checked ? 1 : 0) +
                    (this.deviceStateDisabledCheckbox.Checked ? 2 : 0) +
                    (this.deviceStateNotPresentCheckbox.Checked ? 4 : 0) +
                    (this.deviceStateUnpluggedCheckbox.Checked ? 8 : 0)));
        private DataFlow UserSelectedDataFlow => this._dispather.Invoke(() => (DataFlow) 
                    (this.deviceFlowCaptureCheckbox.Checked && this.deviceFlowRenderCheckbox.Checked
                        ? 2
                        : (this.deviceFlowCaptureCheckbox.Checked ? 1 : 0)));

        private void DeviceSearchButton_Click(object sender, EventArgs e) {
            if (this._search_requested) {
                MessageBox.Show("A search has already been initiated!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.ClearNativeGridRows();
            this.okButton.Enabled = this.ShouldTheOKButtonBeEnabled;
            this.deviceSearchButton.Enabled = false;
        }

        private void DeviceSearchSelectColumnsButton_Click(object sender, EventArgs e) {
            //pops up a window to select which columns are enabled.
            //names headers help state
            IEnumerable<DataGridViewColumn> columns = this.dataGridView1.Columns.Cast<DataGridViewColumn>();
            IEnumerable<string> ids = columns.Select(a => a.Name);
            using (ColumnSelectDialog dialog = new ColumnSelectDialog(
                ids.ToArray(),
                columns.Select(a => a.HeaderText).ToArray(),
                columns.Select(a => a.ToolTipText).ToArray(),
                columns.Select(a => FORCE_ENABLED_COLUMNS.Contains(a.Name) ? 1 : a.Visible ? 0 : -1).ToArray()) {
                 AllowReorder = false //reordering can be done through the UI as is. Disallow this.
            }) {
                if(dialog.ShowDialog() == DialogResult.OK) {
                    string[] active = dialog.ActiveColumnIds;
                    columns.ToList().ForEach(a => {
                        a.Visible = active.Contains(a.Name);
                    });
                    this.Settings.Settings.AddDeviceForm_Native_EnabledColumns = active;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    this.Settings.SaveAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }
        //ToolTipText, HeaderText, Name

        private void AddDeviceForm_FormClosed(object sender, FormClosedEventArgs e) {
            this._loading_thread.Abort();
            if (this.DialogResult  == DialogResult.Cancel) {
                this.ClearNativeGridRows();
            }
        }
        private void ClearNativeGridRows() {
            IEnumerable<DataGridViewRow> rows = this.dataGridView1.Rows.OfType<DataGridViewRow>();
            foreach (DataGridViewRow row in rows) {
                ((MMDevice)row.Tag).Dispose();
            }
            this.dataGridView1.Rows.Clear();
        }

        private void OkButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
            this.SelectedDevice = this.CreatedDevice;
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e) {
            this.okButton.Enabled = this.NativeSearchFormDisabled
                ? false 
                : this.dataGridView1.SelectedRows.Count > 0;
        }
        private ApplicationSettings.SettingsV0.AudioDeviceSettings CreatedDevice {
            get {
                bool native = this.tabControl1.SelectedTab == this.nativeTab;
                AudioDeviceProperties properties = (AudioDeviceProperties)this.dataGridView1.SelectedRows[0].Tag;
                return new ApplicationSettings.SettingsV0.AudioDeviceSettings() {
                    Capture = true,
                    Notes = "",
                    DisplayName = native ? properties.ID : "", //same as ObsName
                    Type = this.GetDeviceType(),
                    DeviceId = native ? properties.ID : null,
                    ObsName = native ? null : ""
                };
#warning properly retrieve the OBS name via textbox.

            }
        }

        public ApplicationSettings.SettingsV0.AudioDeviceSettings SelectedDevice { get; private set; }
        private ApplicationSettings.SettingsV0.AudioDeviceSettings.DeviceType GetDeviceType() {
            bool native = this.tabControl1.SelectedTab == this.nativeTab;
            ApplicationSettings.SettingsV0.AudioDeviceSettings.DeviceType type = ApplicationSettings.SettingsV0.AudioDeviceSettings.DeviceType.GenericInput;
            if (native) {
                MMDevice device = (MMDevice)this.dataGridView1.SelectedRows[0].Tag;
                if (device.DataFlow == DataFlow.Render) {
                    type |= ApplicationSettings.SettingsV0.AudioDeviceSettings.DeviceType.GenericOutput;
                }
            } else {
                type |= ApplicationSettings.SettingsV0.AudioDeviceSettings.DeviceType.ScreenCaptureInput;
#warning TODO: If there is a checkbox to specify in the OBS tab if it's input or output make sure to do that here.
            }
            return type;
        }

        private void NativeDeviceContextStrip_Opening(object sender, CancelEventArgs e) {
            e.Cancel = this.dataGridView1.SelectedRows.Count <= 0;
            this.propertiesToolStripMenuItem.Enabled = this.dataGridView1.SelectedRows.Count > 0;
        }
    }

}
//Sequence:
//    0. Specify MMDeviceEnumerator and general search criteria.
//    1. MMDeviceEnumerator to get a list of devices.
//    2. Display matching devices (how would we list them...? maybe a list of line items with columns...? add an icon?)
//    3. User selects a device (and that enables the OK button.)
//    4. User presses OK.
//    5. device ID is returned through a public property.
//REFRESH BUTTON.
//Context menu for the loading devices bit to open up property windows.
#warning DO NOT REMOVE THESE WARNINGS
#warning 1. REMEMBER TO DO ALL OF THE COM INTEROP ON A SEPARATE THREAD.
#warning 2. DO NOT USE TASK AS THAT CANNOT GUARANTEE THAT THE SAME THREAD IS USED.
#warning 3. COM MUST BE SAME THREAD. ACCESSING FROM OTHER THREAD WILL NOT WORK
#warning 4. THE ID OF THE DEVICE IS USED TO IDENTIFY IT.