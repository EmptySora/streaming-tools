namespace WinAudioLevels {
    partial class NativeAudioDevicePropertiesDialog {
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
            this.devicePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonGroupPanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.buttonGroupPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // devicePropertyGrid
            // 
            this.devicePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicePropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.devicePropertyGrid.Name = "devicePropertyGrid";
            this.devicePropertyGrid.Size = new System.Drawing.Size(487, 573);
            this.devicePropertyGrid.TabIndex = 1;
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
            this.splitContainer1.Panel1.Controls.Add(this.devicePropertyGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.buttonGroupPanel);
            this.splitContainer1.Size = new System.Drawing.Size(487, 616);
            this.splitContainer1.SplitterDistance = 573;
            this.splitContainer1.TabIndex = 2;
            // 
            // buttonGroupPanel
            // 
            this.buttonGroupPanel.Controls.Add(this.okButton);
            this.buttonGroupPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonGroupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonGroupPanel.Name = "buttonGroupPanel";
            this.buttonGroupPanel.Padding = new System.Windows.Forms.Padding(4, 8, 4, 8);
            this.buttonGroupPanel.Size = new System.Drawing.Size(487, 39);
            this.buttonGroupPanel.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(408, 8);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // NativeAudioDevicePropertiesDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(487, 616);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NativeAudioDevicePropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Native Audio Device Properties";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.buttonGroupPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid devicePropertyGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel buttonGroupPanel;
        private System.Windows.Forms.Button okButton;
    }
}