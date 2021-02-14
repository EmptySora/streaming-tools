namespace WinAudioLevels {
    partial class AddDeviceForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddDeviceForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.nativeTab = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.deviceSearchSelectColumnsButton = new System.Windows.Forms.Button();
            this.deviceSearchButton = new System.Windows.Forms.Button();
            this.deviceFlowGroupBox = new System.Windows.Forms.GroupBox();
            this.deviceFlowRenderCheckbox = new System.Windows.Forms.CheckBox();
            this.deviceFlowCaptureCheckbox = new System.Windows.Forms.CheckBox();
            this.deviceStatusGroupBox = new System.Windows.Forms.GroupBox();
            this.deviceStateUnpluggedCheckbox = new System.Windows.Forms.CheckBox();
            this.deviceStateActiveCheckbox = new System.Windows.Forms.CheckBox();
            this.deviceStateNotPresentCheckbox = new System.Windows.Forms.CheckBox();
            this.deviceStateDisabledCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.deviceIconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.deviceNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceStatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceInstanceIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceDeviceFriendlyNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceFriendlyNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioSessionCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioMeterMasterPeakColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeMasterVolumeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeMasterVolumeScalarColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeMuteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeRangeMaxdBColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeRangeMindBColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeRangeIncrementdBColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeStepColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioVolumeStepCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioClientMixFormatAvgBytesPerSecondColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioClientMixFormatBitsPerSampleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioClientMixFormatBlockAlignColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioClientMixFormatChannelCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioClientMixFormatEncodingColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioClientMixFormatExtraSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceAudioClientMixFormatSampleRateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nativeDeviceContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obsTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonGroupPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.deviceSearchToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.nativeTab.SuspendLayout();
            this.deviceFlowGroupBox.SuspendLayout();
            this.deviceStatusGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.nativeDeviceContextStrip.SuspendLayout();
            this.obsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.buttonGroupPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.nativeTab);
            this.tabControl1.Controls.Add(this.obsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(487, 388);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.TabIndexChanged += new System.EventHandler(this.TabControl1_TabIndexChanged);
            // 
            // nativeTab
            // 
            this.nativeTab.Controls.Add(this.label2);
            this.nativeTab.Controls.Add(this.deviceSearchSelectColumnsButton);
            this.nativeTab.Controls.Add(this.deviceSearchButton);
            this.nativeTab.Controls.Add(this.deviceFlowGroupBox);
            this.nativeTab.Controls.Add(this.deviceStatusGroupBox);
            this.nativeTab.Controls.Add(this.label1);
            this.nativeTab.Controls.Add(this.dataGridView1);
            this.nativeTab.Location = new System.Drawing.Point(4, 22);
            this.nativeTab.Name = "nativeTab";
            this.nativeTab.Padding = new System.Windows.Forms.Padding(3);
            this.nativeTab.Size = new System.Drawing.Size(479, 362);
            this.nativeTab.TabIndex = 0;
            this.nativeTab.Text = "Native Device";
            this.nativeTab.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 346);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(307, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Tip: Right-click on a found device to copy or view its properties.\r\n";
            // 
            // deviceSearchSelectColumnsButton
            // 
            this.deviceSearchSelectColumnsButton.Location = new System.Drawing.Point(291, 72);
            this.deviceSearchSelectColumnsButton.Name = "deviceSearchSelectColumnsButton";
            this.deviceSearchSelectColumnsButton.Size = new System.Drawing.Size(177, 23);
            this.deviceSearchSelectColumnsButton.TabIndex = 7;
            this.deviceSearchSelectColumnsButton.Text = "Select Columns...";
            this.deviceSearchSelectColumnsButton.UseVisualStyleBackColor = true;
            this.deviceSearchSelectColumnsButton.Click += new System.EventHandler(this.DeviceSearchSelectColumnsButton_Click);
            // 
            // deviceSearchButton
            // 
            this.deviceSearchButton.Location = new System.Drawing.Point(291, 43);
            this.deviceSearchButton.Name = "deviceSearchButton";
            this.deviceSearchButton.Size = new System.Drawing.Size(177, 23);
            this.deviceSearchButton.TabIndex = 6;
            this.deviceSearchButton.Text = "Search";
            this.deviceSearchButton.UseVisualStyleBackColor = true;
            this.deviceSearchButton.Click += new System.EventHandler(this.DeviceSearchButton_Click);
            // 
            // deviceFlowGroupBox
            // 
            this.deviceFlowGroupBox.Controls.Add(this.deviceFlowRenderCheckbox);
            this.deviceFlowGroupBox.Controls.Add(this.deviceFlowCaptureCheckbox);
            this.deviceFlowGroupBox.Location = new System.Drawing.Point(189, 36);
            this.deviceFlowGroupBox.Name = "deviceFlowGroupBox";
            this.deviceFlowGroupBox.Size = new System.Drawing.Size(95, 61);
            this.deviceFlowGroupBox.TabIndex = 5;
            this.deviceFlowGroupBox.TabStop = false;
            this.deviceFlowGroupBox.Text = "Device Type";
            // 
            // deviceFlowRenderCheckbox
            // 
            this.deviceFlowRenderCheckbox.AutoSize = true;
            this.deviceFlowRenderCheckbox.Checked = true;
            this.deviceFlowRenderCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.deviceFlowRenderCheckbox.Location = new System.Drawing.Point(6, 36);
            this.deviceFlowRenderCheckbox.Name = "deviceFlowRenderCheckbox";
            this.deviceFlowRenderCheckbox.Size = new System.Drawing.Size(61, 17);
            this.deviceFlowRenderCheckbox.TabIndex = 11;
            this.deviceFlowRenderCheckbox.Text = "Render";
            this.deviceFlowRenderCheckbox.UseVisualStyleBackColor = true;
            // 
            // deviceFlowCaptureCheckbox
            // 
            this.deviceFlowCaptureCheckbox.AutoSize = true;
            this.deviceFlowCaptureCheckbox.Checked = true;
            this.deviceFlowCaptureCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.deviceFlowCaptureCheckbox.Location = new System.Drawing.Point(6, 19);
            this.deviceFlowCaptureCheckbox.Name = "deviceFlowCaptureCheckbox";
            this.deviceFlowCaptureCheckbox.Size = new System.Drawing.Size(63, 17);
            this.deviceFlowCaptureCheckbox.TabIndex = 10;
            this.deviceFlowCaptureCheckbox.Text = "Capture";
            this.deviceFlowCaptureCheckbox.UseVisualStyleBackColor = true;
            // 
            // deviceStatusGroupBox
            // 
            this.deviceStatusGroupBox.Controls.Add(this.deviceStateUnpluggedCheckbox);
            this.deviceStatusGroupBox.Controls.Add(this.deviceStateActiveCheckbox);
            this.deviceStatusGroupBox.Controls.Add(this.deviceStateNotPresentCheckbox);
            this.deviceStatusGroupBox.Controls.Add(this.deviceStateDisabledCheckbox);
            this.deviceStatusGroupBox.Location = new System.Drawing.Point(12, 36);
            this.deviceStatusGroupBox.Name = "deviceStatusGroupBox";
            this.deviceStatusGroupBox.Size = new System.Drawing.Size(171, 61);
            this.deviceStatusGroupBox.TabIndex = 3;
            this.deviceStatusGroupBox.TabStop = false;
            this.deviceStatusGroupBox.Text = "Device Status";
            // 
            // deviceStateUnpluggedCheckbox
            // 
            this.deviceStateUnpluggedCheckbox.AutoSize = true;
            this.deviceStateUnpluggedCheckbox.Location = new System.Drawing.Point(79, 19);
            this.deviceStateUnpluggedCheckbox.Name = "deviceStateUnpluggedCheckbox";
            this.deviceStateUnpluggedCheckbox.Size = new System.Drawing.Size(78, 17);
            this.deviceStateUnpluggedCheckbox.TabIndex = 9;
            this.deviceStateUnpluggedCheckbox.Text = "Unplugged";
            this.deviceStateUnpluggedCheckbox.UseVisualStyleBackColor = true;
            // 
            // deviceStateActiveCheckbox
            // 
            this.deviceStateActiveCheckbox.AutoSize = true;
            this.deviceStateActiveCheckbox.Checked = true;
            this.deviceStateActiveCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.deviceStateActiveCheckbox.Location = new System.Drawing.Point(6, 19);
            this.deviceStateActiveCheckbox.Name = "deviceStateActiveCheckbox";
            this.deviceStateActiveCheckbox.Size = new System.Drawing.Size(56, 17);
            this.deviceStateActiveCheckbox.TabIndex = 6;
            this.deviceStateActiveCheckbox.Text = "Active";
            this.deviceStateActiveCheckbox.UseVisualStyleBackColor = true;
            // 
            // deviceStateNotPresentCheckbox
            // 
            this.deviceStateNotPresentCheckbox.AutoSize = true;
            this.deviceStateNotPresentCheckbox.Location = new System.Drawing.Point(79, 36);
            this.deviceStateNotPresentCheckbox.Name = "deviceStateNotPresentCheckbox";
            this.deviceStateNotPresentCheckbox.Size = new System.Drawing.Size(82, 17);
            this.deviceStateNotPresentCheckbox.TabIndex = 7;
            this.deviceStateNotPresentCheckbox.Text = "Not Present";
            this.deviceStateNotPresentCheckbox.UseVisualStyleBackColor = true;
            // 
            // deviceStateDisabledCheckbox
            // 
            this.deviceStateDisabledCheckbox.AutoSize = true;
            this.deviceStateDisabledCheckbox.Location = new System.Drawing.Point(6, 36);
            this.deviceStateDisabledCheckbox.Name = "deviceStateDisabledCheckbox";
            this.deviceStateDisabledCheckbox.Size = new System.Drawing.Size(67, 17);
            this.deviceStateDisabledCheckbox.TabIndex = 8;
            this.deviceStateDisabledCheckbox.Text = "Disabled";
            this.deviceStateDisabledCheckbox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(459, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select the search criteria below and then click \"Search\" to find native audio dev" +
    "ices connected\r\nto your computer.";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.CausesValidation = false;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.deviceIconColumn,
            this.deviceNameColumn,
            this.deviceTypeColumn,
            this.deviceStatusColumn,
            this.deviceIdColumn,
            this.deviceInstanceIdColumn,
            this.deviceDeviceFriendlyNameColumn,
            this.deviceFriendlyNameColumn,
            this.deviceAudioSessionCountColumn,
            this.deviceAudioMeterMasterPeakColumn,
            this.deviceAudioVolumeMasterVolumeColumn,
            this.deviceAudioVolumeMasterVolumeScalarColumn,
            this.deviceAudioVolumeMuteColumn,
            this.deviceAudioVolumeRangeMaxdBColumn,
            this.deviceAudioVolumeRangeMindBColumn,
            this.deviceAudioVolumeRangeIncrementdBColumn,
            this.deviceAudioVolumeStepColumn,
            this.deviceAudioVolumeStepCountColumn,
            this.deviceAudioClientMixFormatAvgBytesPerSecondColumn,
            this.deviceAudioClientMixFormatBitsPerSampleColumn,
            this.deviceAudioClientMixFormatBlockAlignColumn,
            this.deviceAudioClientMixFormatChannelCountColumn,
            this.deviceAudioClientMixFormatEncodingColumn,
            this.deviceAudioClientMixFormatExtraSizeColumn,
            this.deviceAudioClientMixFormatSampleRateColumn});
            this.dataGridView1.ContextMenuStrip = this.nativeDeviceContextStrip;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(12, 103);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.ReadOnly = true;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(456, 240);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
            // 
            // deviceIconColumn
            // 
            this.deviceIconColumn.HeaderText = "Icon";
            this.deviceIconColumn.Name = "deviceIconColumn";
            this.deviceIconColumn.ReadOnly = true;
            this.deviceIconColumn.ToolTipText = "An icon that represents the audio device in question.";
            this.deviceIconColumn.Width = 32;
            // 
            // deviceNameColumn
            // 
            this.deviceNameColumn.HeaderText = "Name";
            this.deviceNameColumn.Name = "deviceNameColumn";
            this.deviceNameColumn.ReadOnly = true;
            this.deviceNameColumn.ToolTipText = "The name of the device. If this is not available, the device\'s ID is used instead" +
    ".";
            // 
            // deviceTypeColumn
            // 
            this.deviceTypeColumn.HeaderText = "Type";
            this.deviceTypeColumn.Name = "deviceTypeColumn";
            this.deviceTypeColumn.ReadOnly = true;
            this.deviceTypeColumn.ToolTipText = "Determines whether or not the device captures audio or renders it. The former wou" +
    "ld include microphones and loopback drivers, while the latter would include spea" +
    "kers and headphones.";
            // 
            // deviceStatusColumn
            // 
            this.deviceStatusColumn.HeaderText = "Status";
            this.deviceStatusColumn.Name = "deviceStatusColumn";
            this.deviceStatusColumn.ReadOnly = true;
            this.deviceStatusColumn.ToolTipText = resources.GetString("deviceStatusColumn.ToolTipText");
            // 
            // deviceIdColumn
            // 
            this.deviceIdColumn.HeaderText = "ID";
            this.deviceIdColumn.Name = "deviceIdColumn";
            this.deviceIdColumn.ReadOnly = true;
            this.deviceIdColumn.ToolTipText = "The ID of the audio device.";
            // 
            // deviceInstanceIdColumn
            // 
            this.deviceInstanceIdColumn.HeaderText = "Instance ID";
            this.deviceInstanceIdColumn.Name = "deviceInstanceIdColumn";
            this.deviceInstanceIdColumn.ReadOnly = true;
            this.deviceInstanceIdColumn.ToolTipText = "The ID of this instance of the device.";
            // 
            // deviceDeviceFriendlyNameColumn
            // 
            this.deviceDeviceFriendlyNameColumn.HeaderText = "Device Friendly Name";
            this.deviceDeviceFriendlyNameColumn.Name = "deviceDeviceFriendlyNameColumn";
            this.deviceDeviceFriendlyNameColumn.ReadOnly = true;
            this.deviceDeviceFriendlyNameColumn.ToolTipText = "A user-friendly string that describes the audio device.";
            // 
            // deviceFriendlyNameColumn
            // 
            this.deviceFriendlyNameColumn.HeaderText = "Friendly Name";
            this.deviceFriendlyNameColumn.Name = "deviceFriendlyNameColumn";
            this.deviceFriendlyNameColumn.ReadOnly = true;
            this.deviceFriendlyNameColumn.ToolTipText = "A user-friendly descriptive name for the audio device.";
            // 
            // deviceAudioSessionCountColumn
            // 
            this.deviceAudioSessionCountColumn.HeaderText = "Audio Session Count";
            this.deviceAudioSessionCountColumn.Name = "deviceAudioSessionCountColumn";
            this.deviceAudioSessionCountColumn.ReadOnly = true;
            this.deviceAudioSessionCountColumn.ToolTipText = "The total number of open audio sessions to this audio device.";
            // 
            // deviceAudioMeterMasterPeakColumn
            // 
            this.deviceAudioMeterMasterPeakColumn.HeaderText = "Master Peak Volume";
            this.deviceAudioMeterMasterPeakColumn.Name = "deviceAudioMeterMasterPeakColumn";
            this.deviceAudioMeterMasterPeakColumn.ReadOnly = true;
            this.deviceAudioMeterMasterPeakColumn.ToolTipText = "The overall peak volume of the audio device.";
            // 
            // deviceAudioVolumeMasterVolumeColumn
            // 
            this.deviceAudioVolumeMasterVolumeColumn.HeaderText = "Master Volume";
            this.deviceAudioVolumeMasterVolumeColumn.Name = "deviceAudioVolumeMasterVolumeColumn";
            this.deviceAudioVolumeMasterVolumeColumn.ReadOnly = true;
            this.deviceAudioVolumeMasterVolumeColumn.ToolTipText = "The overall volume level of the device.";
            // 
            // deviceAudioVolumeMasterVolumeScalarColumn
            // 
            this.deviceAudioVolumeMasterVolumeScalarColumn.HeaderText = "Master Volume (Scalar)";
            this.deviceAudioVolumeMasterVolumeScalarColumn.Name = "deviceAudioVolumeMasterVolumeScalarColumn";
            this.deviceAudioVolumeMasterVolumeScalarColumn.ReadOnly = true;
            this.deviceAudioVolumeMasterVolumeScalarColumn.ToolTipText = "The overall volume level of the device as a scalar number.";
            // 
            // deviceAudioVolumeMuteColumn
            // 
            this.deviceAudioVolumeMuteColumn.HeaderText = "Muted";
            this.deviceAudioVolumeMuteColumn.Name = "deviceAudioVolumeMuteColumn";
            this.deviceAudioVolumeMuteColumn.ReadOnly = true;
            this.deviceAudioVolumeMuteColumn.ToolTipText = "Whether or not the audio device is muted.";
            // 
            // deviceAudioVolumeRangeMaxdBColumn
            // 
            this.deviceAudioVolumeRangeMaxdBColumn.HeaderText = "Volume (Max, dB)";
            this.deviceAudioVolumeRangeMaxdBColumn.Name = "deviceAudioVolumeRangeMaxdBColumn";
            this.deviceAudioVolumeRangeMaxdBColumn.ReadOnly = true;
            this.deviceAudioVolumeRangeMaxdBColumn.ToolTipText = "The maximum volume of the audio device in decibels.";
            // 
            // deviceAudioVolumeRangeMindBColumn
            // 
            this.deviceAudioVolumeRangeMindBColumn.HeaderText = "Volume (Min, dB)";
            this.deviceAudioVolumeRangeMindBColumn.Name = "deviceAudioVolumeRangeMindBColumn";
            this.deviceAudioVolumeRangeMindBColumn.ReadOnly = true;
            this.deviceAudioVolumeRangeMindBColumn.ToolTipText = "The minimum volume of the audio device in decibels.";
            // 
            // deviceAudioVolumeRangeIncrementdBColumn
            // 
            this.deviceAudioVolumeRangeIncrementdBColumn.HeaderText = "Volume Increment (dB)";
            this.deviceAudioVolumeRangeIncrementdBColumn.Name = "deviceAudioVolumeRangeIncrementdBColumn";
            this.deviceAudioVolumeRangeIncrementdBColumn.ReadOnly = true;
            this.deviceAudioVolumeRangeIncrementdBColumn.ToolTipText = "The volume, in decibels, that separates each step of volume in the audio device. " +
    "";
            // 
            // deviceAudioVolumeStepColumn
            // 
            this.deviceAudioVolumeStepColumn.HeaderText = "Volume Step";
            this.deviceAudioVolumeStepColumn.Name = "deviceAudioVolumeStepColumn";
            this.deviceAudioVolumeStepColumn.ReadOnly = true;
            this.deviceAudioVolumeStepColumn.ToolTipText = "Determines which volume step the audio device is currently at.";
            // 
            // deviceAudioVolumeStepCountColumn
            // 
            this.deviceAudioVolumeStepCountColumn.HeaderText = "Volume Step Count";
            this.deviceAudioVolumeStepCountColumn.Name = "deviceAudioVolumeStepCountColumn";
            this.deviceAudioVolumeStepCountColumn.ReadOnly = true;
            this.deviceAudioVolumeStepCountColumn.ToolTipText = "Determines the total number of volume steps for this audio device.";
            // 
            // deviceAudioClientMixFormatAvgBytesPerSecondColumn
            // 
            this.deviceAudioClientMixFormatAvgBytesPerSecondColumn.HeaderText = "Average Data Rate (B/s)";
            this.deviceAudioClientMixFormatAvgBytesPerSecondColumn.Name = "deviceAudioClientMixFormatAvgBytesPerSecondColumn";
            this.deviceAudioClientMixFormatAvgBytesPerSecondColumn.ReadOnly = true;
            this.deviceAudioClientMixFormatAvgBytesPerSecondColumn.ToolTipText = "Gets the average data rate of the device in bytes per second.";
            // 
            // deviceAudioClientMixFormatBitsPerSampleColumn
            // 
            this.deviceAudioClientMixFormatBitsPerSampleColumn.HeaderText = "Sample Size (b)";
            this.deviceAudioClientMixFormatBitsPerSampleColumn.Name = "deviceAudioClientMixFormatBitsPerSampleColumn";
            this.deviceAudioClientMixFormatBitsPerSampleColumn.ReadOnly = true;
            this.deviceAudioClientMixFormatBitsPerSampleColumn.ToolTipText = "Gets the size of each audio sample in bits.";
            // 
            // deviceAudioClientMixFormatBlockAlignColumn
            // 
            this.deviceAudioClientMixFormatBlockAlignColumn.HeaderText = "Block Alignment";
            this.deviceAudioClientMixFormatBlockAlignColumn.Name = "deviceAudioClientMixFormatBlockAlignColumn";
            this.deviceAudioClientMixFormatBlockAlignColumn.ReadOnly = true;
            this.deviceAudioClientMixFormatBlockAlignColumn.ToolTipText = "Gets the alignment of each block.";
            // 
            // deviceAudioClientMixFormatChannelCountColumn
            // 
            this.deviceAudioClientMixFormatChannelCountColumn.HeaderText = "Channel Count";
            this.deviceAudioClientMixFormatChannelCountColumn.Name = "deviceAudioClientMixFormatChannelCountColumn";
            this.deviceAudioClientMixFormatChannelCountColumn.ReadOnly = true;
            this.deviceAudioClientMixFormatChannelCountColumn.ToolTipText = "Gets the total number of audio channels on the audio device.";
            // 
            // deviceAudioClientMixFormatEncodingColumn
            // 
            this.deviceAudioClientMixFormatEncodingColumn.HeaderText = "Encoding";
            this.deviceAudioClientMixFormatEncodingColumn.Name = "deviceAudioClientMixFormatEncodingColumn";
            this.deviceAudioClientMixFormatEncodingColumn.ReadOnly = true;
            this.deviceAudioClientMixFormatEncodingColumn.ToolTipText = "Gets the audio encoding used by the audio device when capturing or rendering audi" +
    "o.";
            // 
            // deviceAudioClientMixFormatExtraSizeColumn
            // 
            this.deviceAudioClientMixFormatExtraSizeColumn.HeaderText = "Extra Size";
            this.deviceAudioClientMixFormatExtraSizeColumn.Name = "deviceAudioClientMixFormatExtraSizeColumn";
            this.deviceAudioClientMixFormatExtraSizeColumn.ReadOnly = true;
            this.deviceAudioClientMixFormatExtraSizeColumn.ToolTipText = "I\'m not actually sure about this one...";
            // 
            // deviceAudioClientMixFormatSampleRateColumn
            // 
            this.deviceAudioClientMixFormatSampleRateColumn.HeaderText = "Sample Rate";
            this.deviceAudioClientMixFormatSampleRateColumn.Name = "deviceAudioClientMixFormatSampleRateColumn";
            this.deviceAudioClientMixFormatSampleRateColumn.ReadOnly = true;
            this.deviceAudioClientMixFormatSampleRateColumn.ToolTipText = "Gets the total number of samples per second that are recorded/captured by this de" +
    "vice.";
            // 
            // nativeDeviceContextStrip
            // 
            this.nativeDeviceContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripMenuItem});
            this.nativeDeviceContextStrip.Name = "nativeDeviceContextStrip";
            this.nativeDeviceContextStrip.Size = new System.Drawing.Size(137, 26);
            this.nativeDeviceContextStrip.Opening += new System.ComponentModel.CancelEventHandler(this.NativeDeviceContextStrip_Opening);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.propertiesToolStripMenuItem.Text = "&Properties...";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.PropertiesToolStripMenuItem_Click);
            // 
            // obsTab
            // 
            this.obsTab.Controls.Add(this.label3);
            this.obsTab.Location = new System.Drawing.Point(4, 22);
            this.obsTab.Name = "obsTab";
            this.obsTab.Padding = new System.Windows.Forms.Padding(3);
            this.obsTab.Size = new System.Drawing.Size(479, 362);
            this.obsTab.TabIndex = 1;
            this.obsTab.Text = "OBS Mixer";
            this.obsTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.buttonGroupPanel);
            this.splitContainer1.Size = new System.Drawing.Size(487, 432);
            this.splitContainer1.SplitterDistance = 388;
            this.splitContainer1.TabIndex = 1;
            // 
            // buttonGroupPanel
            // 
            this.buttonGroupPanel.Controls.Add(this.cancelButton);
            this.buttonGroupPanel.Controls.Add(this.okButton);
            this.buttonGroupPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonGroupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonGroupPanel.Name = "buttonGroupPanel";
            this.buttonGroupPanel.Padding = new System.Windows.Forms.Padding(4, 8, 4, 8);
            this.buttonGroupPanel.Size = new System.Drawing.Size(487, 39);
            this.buttonGroupPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(327, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(408, 8);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(459, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "Select the search criteria below and then click \"Search\" to find native audio dev" +
    "ices connected\r\nto your computer.";
            // 
            // AddDeviceForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(487, 432);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HelpButton = true;
            this.Name = "AddDeviceForm";
            this.ShowInTaskbar = false;
            this.Text = "Add New Audio Device";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddDeviceForm_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.nativeTab.ResumeLayout(false);
            this.nativeTab.PerformLayout();
            this.deviceFlowGroupBox.ResumeLayout(false);
            this.deviceFlowGroupBox.PerformLayout();
            this.deviceStatusGroupBox.ResumeLayout(false);
            this.deviceStatusGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.nativeDeviceContextStrip.ResumeLayout(false);
            this.obsTab.ResumeLayout(false);
            this.obsTab.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.buttonGroupPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage nativeTab;
        private System.Windows.Forms.TabPage obsTab;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel buttonGroupPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ContextMenuStrip nativeDeviceContextStrip;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button deviceSearchSelectColumnsButton;
        private System.Windows.Forms.Button deviceSearchButton;
        private System.Windows.Forms.GroupBox deviceFlowGroupBox;
        private System.Windows.Forms.CheckBox deviceFlowRenderCheckbox;
        private System.Windows.Forms.CheckBox deviceFlowCaptureCheckbox;
        private System.Windows.Forms.GroupBox deviceStatusGroupBox;
        private System.Windows.Forms.CheckBox deviceStateUnpluggedCheckbox;
        private System.Windows.Forms.CheckBox deviceStateActiveCheckbox;
        private System.Windows.Forms.CheckBox deviceStateNotPresentCheckbox;
        private System.Windows.Forms.CheckBox deviceStateDisabledCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip deviceSearchToolTip;
        private System.Windows.Forms.DataGridViewImageColumn deviceIconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceStatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceInstanceIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceDeviceFriendlyNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceFriendlyNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioSessionCountColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioMeterMasterPeakColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeMasterVolumeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeMasterVolumeScalarColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeMuteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeRangeMaxdBColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeRangeMindBColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeRangeIncrementdBColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeStepColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioVolumeStepCountColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioClientMixFormatAvgBytesPerSecondColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioClientMixFormatBitsPerSampleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioClientMixFormatBlockAlignColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioClientMixFormatChannelCountColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioClientMixFormatEncodingColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioClientMixFormatExtraSizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceAudioClientMixFormatSampleRateColumn;
        private System.Windows.Forms.Label label3;
    }
}