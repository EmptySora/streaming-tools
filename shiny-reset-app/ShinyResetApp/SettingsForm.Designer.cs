
namespace ShinyResetApp {
    partial class SettingsForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._tshiny_box = new System.Windows.Forms.NumericUpDown();
            this._treset_box = new System.Windows.Forms.NumericUpDown();
            this._ravg_box = new System.Windows.Forms.TextBox();
            this._rcount_box = new System.Windows.Forms.TextBox();
            this._rcount_browse_button = new System.Windows.Forms.Button();
            this._ravg_browse_button = new System.Windows.Forms.Button();
            this._apply_button = new System.Windows.Forms.Button();
            this._cancel_button = new System.Windows.Forms.Button();
            this._ok_button = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tshiny_box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._treset_box)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._tshiny_box, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this._treset_box, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._ravg_box, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._rcount_box, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._rcount_browse_button, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this._ravg_browse_button, 2, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // _tshiny_box
            // 
            resources.ApplyResources(this._tshiny_box, "_tshiny_box");
            this._tshiny_box.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this._tshiny_box.Name = "_tshiny_box";
            this._tshiny_box.ValueChanged += new System.EventHandler(this.Tshiny_box_ValueChanged);
            this._tshiny_box.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Tshiny_box_KeyUp);
            // 
            // _treset_box
            // 
            resources.ApplyResources(this._treset_box, "_treset_box");
            this._treset_box.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this._treset_box.Name = "_treset_box";
            this._treset_box.ValueChanged += new System.EventHandler(this.Treset_box_ValueChanged);
            this._treset_box.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Treset_box_KeyUp);
            // 
            // _ravg_box
            // 
            resources.ApplyResources(this._ravg_box, "_ravg_box");
            this._ravg_box.Name = "_ravg_box";
            this._ravg_box.TextChanged += new System.EventHandler(this.Ravg_box_TextChanged);
            this._ravg_box.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Ravg_box_KeyUp);
            // 
            // _rcount_box
            // 
            resources.ApplyResources(this._rcount_box, "_rcount_box");
            this._rcount_box.Name = "_rcount_box";
            this._rcount_box.TextChanged += new System.EventHandler(this.Rcount_box_TextChanged);
            this._rcount_box.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Rcount_box_KeyUp);
            // 
            // _rcount_browse_button
            // 
            resources.ApplyResources(this._rcount_browse_button, "_rcount_browse_button");
            this._rcount_browse_button.Name = "_rcount_browse_button";
            this._rcount_browse_button.Text = global::ShinyResetApp.Resources.SettingsBrowseButtonText;
            this._rcount_browse_button.UseVisualStyleBackColor = true;
            this._rcount_browse_button.Click += new System.EventHandler(this.Rcount_browse_button_Click);
            // 
            // _ravg_browse_button
            // 
            resources.ApplyResources(this._ravg_browse_button, "_ravg_browse_button");
            this._ravg_browse_button.Name = "_ravg_browse_button";
            this._ravg_browse_button.Text = global::ShinyResetApp.Resources.SettingsBrowseButtonText;
            this._ravg_browse_button.UseVisualStyleBackColor = true;
            this._ravg_browse_button.Click += new System.EventHandler(this.Ravg_browse_button_Click);
            // 
            // _apply_button
            // 
            resources.ApplyResources(this._apply_button, "_apply_button");
            this._apply_button.Name = "_apply_button";
            this._apply_button.Text = global::ShinyResetApp.Resources.SettingsApplyButtonText;
            this._apply_button.UseVisualStyleBackColor = true;
            this._apply_button.Click += new System.EventHandler(this.Apply_button_Click);
            // 
            // _cancel_button
            // 
            this._cancel_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this._cancel_button, "_cancel_button");
            this._cancel_button.Name = "_cancel_button";
            this._cancel_button.Text = global::ShinyResetApp.Resources.SettingsCancelButtonText;
            this._cancel_button.UseVisualStyleBackColor = true;
            this._cancel_button.Click += new System.EventHandler(this.Cancel_button_Click);
            // 
            // _ok_button
            // 
            this._ok_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this._ok_button, "_ok_button");
            this._ok_button.Name = "_ok_button";
            this._ok_button.Text = global::ShinyResetApp.Resources.SettingsOkButtonText;
            this._ok_button.UseVisualStyleBackColor = true;
            this._ok_button.Click += new System.EventHandler(this.Ok_button_Click);
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ok_button);
            this.Controls.Add(this._cancel_button);
            this.Controls.Add(this._apply_button);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tshiny_box)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._treset_box)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button _apply_button;
        private System.Windows.Forms.Button _cancel_button;
        private System.Windows.Forms.Button _ok_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown _tshiny_box;
        private System.Windows.Forms.NumericUpDown _treset_box;
        private System.Windows.Forms.TextBox _ravg_box;
        private System.Windows.Forms.TextBox _rcount_box;
        private System.Windows.Forms.Button _rcount_browse_button;
        private System.Windows.Forms.Button _ravg_browse_button;
    }
}