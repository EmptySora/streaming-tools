namespace WinAudioLevels {
    partial class ColumnSelectDialog {
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
            this.inactiveColumnsList = new System.Windows.Forms.ListBox();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.inactiveColumnsLabel = new System.Windows.Forms.Label();
            this.showButton = new System.Windows.Forms.Button();
            this.hideButton = new System.Windows.Forms.Button();
            this.activeColumnsList = new System.Windows.Forms.ListBox();
            this.activeColumnsLabel = new System.Windows.Forms.Label();
            this.shiftTopButton = new System.Windows.Forms.Button();
            this.shiftUpButton = new System.Windows.Forms.Button();
            this.shiftDownButton = new System.Windows.Forms.Button();
            this.shiftBottomButton = new System.Windows.Forms.Button();
            this.informationGroupBox = new System.Windows.Forms.GroupBox();
            this.informationBox = new System.Windows.Forms.RichTextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.informationGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // inactiveColumnsList
            // 
            this.inactiveColumnsList.FormattingEnabled = true;
            this.inactiveColumnsList.Location = new System.Drawing.Point(16, 55);
            this.inactiveColumnsList.Name = "inactiveColumnsList";
            this.inactiveColumnsList.Size = new System.Drawing.Size(184, 238);
            this.inactiveColumnsList.TabIndex = 0;
            this.inactiveColumnsList.SelectedIndexChanged += new System.EventHandler(this.InactiveColumnsList_SelectedIndexChanged);
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.AutoSize = true;
            this.instructionsLabel.Location = new System.Drawing.Point(13, 13);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(218, 13);
            this.instructionsLabel.TabIndex = 1;
            this.instructionsLabel.Text = "Select the columns that will appear in the list.\r\n";
            // 
            // inactiveColumnsLabel
            // 
            this.inactiveColumnsLabel.AutoSize = true;
            this.inactiveColumnsLabel.Location = new System.Drawing.Point(13, 39);
            this.inactiveColumnsLabel.Name = "inactiveColumnsLabel";
            this.inactiveColumnsLabel.Size = new System.Drawing.Size(91, 13);
            this.inactiveColumnsLabel.TabIndex = 3;
            this.inactiveColumnsLabel.Text = "Inactive Columns:";
            // 
            // showButton
            // 
            this.showButton.Location = new System.Drawing.Point(206, 146);
            this.showButton.Name = "showButton";
            this.showButton.Size = new System.Drawing.Size(75, 23);
            this.showButton.TabIndex = 4;
            this.showButton.Text = "Show >";
            this.showButton.UseVisualStyleBackColor = true;
            this.showButton.Click += new System.EventHandler(this.ShowButton_Click);
            // 
            // hideButton
            // 
            this.hideButton.Location = new System.Drawing.Point(206, 175);
            this.hideButton.Name = "hideButton";
            this.hideButton.Size = new System.Drawing.Size(75, 23);
            this.hideButton.TabIndex = 5;
            this.hideButton.Text = "< Hide";
            this.hideButton.UseVisualStyleBackColor = true;
            this.hideButton.Click += new System.EventHandler(this.HideButton_Click);
            // 
            // activeColumnsList
            // 
            this.activeColumnsList.FormattingEnabled = true;
            this.activeColumnsList.Location = new System.Drawing.Point(287, 55);
            this.activeColumnsList.Name = "activeColumnsList";
            this.activeColumnsList.Size = new System.Drawing.Size(184, 238);
            this.activeColumnsList.TabIndex = 6;
            this.activeColumnsList.SelectedIndexChanged += new System.EventHandler(this.ActiveColumnsList_SelectedIndexChanged);
            // 
            // activeColumnsLabel
            // 
            this.activeColumnsLabel.AutoSize = true;
            this.activeColumnsLabel.Location = new System.Drawing.Point(284, 39);
            this.activeColumnsLabel.Name = "activeColumnsLabel";
            this.activeColumnsLabel.Size = new System.Drawing.Size(83, 13);
            this.activeColumnsLabel.TabIndex = 7;
            this.activeColumnsLabel.Text = "Active Columns:";
            // 
            // shiftTopButton
            // 
            this.shiftTopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.shiftTopButton.Location = new System.Drawing.Point(477, 55);
            this.shiftTopButton.Name = "shiftTopButton";
            this.shiftTopButton.Size = new System.Drawing.Size(40, 55);
            this.shiftTopButton.TabIndex = 8;
            this.shiftTopButton.Text = "⭱";
            this.shiftTopButton.UseVisualStyleBackColor = true;
            this.shiftTopButton.Click += new System.EventHandler(this.ShiftTopButton_Click);
            // 
            // shiftUpButton
            // 
            this.shiftUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.shiftUpButton.Location = new System.Drawing.Point(477, 116);
            this.shiftUpButton.Name = "shiftUpButton";
            this.shiftUpButton.Size = new System.Drawing.Size(40, 55);
            this.shiftUpButton.TabIndex = 9;
            this.shiftUpButton.Text = "↑";
            this.shiftUpButton.UseVisualStyleBackColor = true;
            this.shiftUpButton.Click += new System.EventHandler(this.ShiftUpButton_Click);
            // 
            // shiftDownButton
            // 
            this.shiftDownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.shiftDownButton.Location = new System.Drawing.Point(477, 177);
            this.shiftDownButton.Name = "shiftDownButton";
            this.shiftDownButton.Size = new System.Drawing.Size(40, 55);
            this.shiftDownButton.TabIndex = 10;
            this.shiftDownButton.Text = "↓";
            this.shiftDownButton.UseVisualStyleBackColor = true;
            this.shiftDownButton.Click += new System.EventHandler(this.ShiftDownButton_Click);
            // 
            // shiftBottomButton
            // 
            this.shiftBottomButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.shiftBottomButton.Location = new System.Drawing.Point(477, 238);
            this.shiftBottomButton.Name = "shiftBottomButton";
            this.shiftBottomButton.Size = new System.Drawing.Size(40, 55);
            this.shiftBottomButton.TabIndex = 11;
            this.shiftBottomButton.Text = "⭳";
            this.shiftBottomButton.UseVisualStyleBackColor = true;
            this.shiftBottomButton.Click += new System.EventHandler(this.ShiftBottomButton_Click);
            // 
            // informationGroupBox
            // 
            this.informationGroupBox.Controls.Add(this.informationBox);
            this.informationGroupBox.Location = new System.Drawing.Point(16, 300);
            this.informationGroupBox.Name = "informationGroupBox";
            this.informationGroupBox.Size = new System.Drawing.Size(501, 100);
            this.informationGroupBox.TabIndex = 12;
            this.informationGroupBox.TabStop = false;
            this.informationGroupBox.Text = "Information";
            // 
            // informationBox
            // 
            this.informationBox.BackColor = System.Drawing.SystemColors.Control;
            this.informationBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.informationBox.CausesValidation = false;
            this.informationBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.informationBox.Location = new System.Drawing.Point(6, 19);
            this.informationBox.Name = "informationBox";
            this.informationBox.ReadOnly = true;
            this.informationBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.informationBox.ShortcutsEnabled = false;
            this.informationBox.Size = new System.Drawing.Size(489, 75);
            this.informationBox.TabIndex = 15;
            this.informationBox.Text = "";
            this.informationBox.SelectionChanged += new System.EventHandler(this.RichTextBox1_SelectionChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(442, 406);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 13;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(361, 406);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 14;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ColumnSelectDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(533, 441);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.informationGroupBox);
            this.Controls.Add(this.shiftBottomButton);
            this.Controls.Add(this.shiftDownButton);
            this.Controls.Add(this.shiftUpButton);
            this.Controls.Add(this.shiftTopButton);
            this.Controls.Add(this.activeColumnsLabel);
            this.Controls.Add(this.activeColumnsList);
            this.Controls.Add(this.hideButton);
            this.Controls.Add(this.showButton);
            this.Controls.Add(this.inactiveColumnsLabel);
            this.Controls.Add(this.instructionsLabel);
            this.Controls.Add(this.inactiveColumnsList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColumnSelectDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Columns...";
            this.informationGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox inactiveColumnsList;
        private System.Windows.Forms.Label instructionsLabel;
        private System.Windows.Forms.Label inactiveColumnsLabel;
        private System.Windows.Forms.Button showButton;
        private System.Windows.Forms.Button hideButton;
        private System.Windows.Forms.ListBox activeColumnsList;
        private System.Windows.Forms.Label activeColumnsLabel;
        private System.Windows.Forms.Button shiftTopButton;
        private System.Windows.Forms.Button shiftUpButton;
        private System.Windows.Forms.Button shiftDownButton;
        private System.Windows.Forms.Button shiftBottomButton;
        private System.Windows.Forms.GroupBox informationGroupBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RichTextBox informationBox;
    }
}