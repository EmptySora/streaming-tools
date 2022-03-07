namespace ShinyResetApp {
    partial class MainForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._main_container = new System.Windows.Forms.SplitContainer();
            this._detail_label = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._split_container = new System.Windows.Forms.SplitContainer();
            this._shiny_split_container = new System.Windows.Forms.SplitContainer();
            this._shiny_decrement_button = new System.Windows.Forms.Button();
            this._shiny_increment_button = new System.Windows.Forms.Button();
            this._attempt_split_container = new System.Windows.Forms.SplitContainer();
            this._decrement_button = new System.Windows.Forms.Button();
            this._increment_button = new System.Windows.Forms.Button();
            this._timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._main_container)).BeginInit();
            this._main_container.Panel1.SuspendLayout();
            this._main_container.Panel2.SuspendLayout();
            this._main_container.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._split_container)).BeginInit();
            this._split_container.Panel1.SuspendLayout();
            this._split_container.Panel2.SuspendLayout();
            this._split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._shiny_split_container)).BeginInit();
            this._shiny_split_container.Panel1.SuspendLayout();
            this._shiny_split_container.Panel2.SuspendLayout();
            this._shiny_split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._attempt_split_container)).BeginInit();
            this._attempt_split_container.Panel1.SuspendLayout();
            this._attempt_split_container.Panel2.SuspendLayout();
            this._attempt_split_container.SuspendLayout();
            this.SuspendLayout();
            // 
            // _main_container
            // 
            resources.ApplyResources(this._main_container, "_main_container");
            this._main_container.Name = "_main_container";
            // 
            // _main_container.Panel1
            // 
            this._main_container.Panel1.Controls.Add(this._detail_label);
            this._main_container.Panel1.Controls.Add(this.menuStrip1);
            // 
            // _main_container.Panel2
            // 
            this._main_container.Panel2.Controls.Add(this._split_container);
            // 
            // _detail_label
            // 
            resources.ApplyResources(this._detail_label, "_detail_label");
            this._detail_label.ForeColor = System.Drawing.Color.Blue;
            this._detail_label.Name = "_detail_label";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            resources.ApplyResources(this.mainToolStripMenuItem, "mainToolStripMenuItem");
            this.mainToolStripMenuItem.Text = global::ShinyResetApp.Resources.MainToolStripItemText;
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            resources.ApplyResources(this.resetToolStripMenuItem, "resetToolStripMenuItem");
            this.resetToolStripMenuItem.Text = global::ShinyResetApp.Resources.ResetToolStripItemText;
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Text = global::ShinyResetApp.Resources.SettingsToolStripItemText;
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Text = global::ShinyResetApp.Resources.ExitToolStripItemText;
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // _split_container
            // 
            resources.ApplyResources(this._split_container, "_split_container");
            this._split_container.Name = "_split_container";
            // 
            // _split_container.Panel1
            // 
            this._split_container.Panel1.Controls.Add(this._shiny_split_container);
            // 
            // _split_container.Panel2
            // 
            this._split_container.Panel2.Controls.Add(this._attempt_split_container);
            // 
            // _shiny_split_container
            // 
            resources.ApplyResources(this._shiny_split_container, "_shiny_split_container");
            this._shiny_split_container.Name = "_shiny_split_container";
            // 
            // _shiny_split_container.Panel1
            // 
            this._shiny_split_container.Panel1.Controls.Add(this._shiny_decrement_button);
            // 
            // _shiny_split_container.Panel2
            // 
            this._shiny_split_container.Panel2.Controls.Add(this._shiny_increment_button);
            // 
            // _shiny_decrement_button
            // 
            this._shiny_decrement_button.BackColor = System.Drawing.Color.DarkRed;
            resources.ApplyResources(this._shiny_decrement_button, "_shiny_decrement_button");
            this._shiny_decrement_button.ForeColor = System.Drawing.Color.Red;
            this._shiny_decrement_button.Name = "_shiny_decrement_button";
            this._shiny_decrement_button.Text = global::ShinyResetApp.Resources.ShinyDecrementText;
            this._shiny_decrement_button.UseVisualStyleBackColor = false;
            this._shiny_decrement_button.Click += new System.EventHandler(this.Shiny_decrement_button_Click);
            // 
            // _shiny_increment_button
            // 
            this._shiny_increment_button.BackColor = System.Drawing.Color.Green;
            resources.ApplyResources(this._shiny_increment_button, "_shiny_increment_button");
            this._shiny_increment_button.ForeColor = System.Drawing.Color.Lime;
            this._shiny_increment_button.Name = "_shiny_increment_button";
            this._shiny_increment_button.Text = global::ShinyResetApp.Resources.ShinyIncrementText;
            this._shiny_increment_button.UseVisualStyleBackColor = false;
            this._shiny_increment_button.Click += new System.EventHandler(this.Shiny_increment_button_Click);
            // 
            // _attempt_split_container
            // 
            resources.ApplyResources(this._attempt_split_container, "_attempt_split_container");
            this._attempt_split_container.Name = "_attempt_split_container";
            // 
            // _attempt_split_container.Panel1
            // 
            this._attempt_split_container.Panel1.Controls.Add(this._decrement_button);
            // 
            // _attempt_split_container.Panel2
            // 
            this._attempt_split_container.Panel2.Controls.Add(this._increment_button);
            // 
            // _decrement_button
            // 
            this._decrement_button.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this._decrement_button, "_decrement_button");
            this._decrement_button.ForeColor = System.Drawing.Color.DarkRed;
            this._decrement_button.Name = "_decrement_button";
            this._decrement_button.Text = global::ShinyResetApp.Resources.AttemptDecrementText;
            this._decrement_button.UseVisualStyleBackColor = false;
            this._decrement_button.Click += new System.EventHandler(this.Decrement_button_Click);
            // 
            // _increment_button
            // 
            this._increment_button.BackColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this._increment_button, "_increment_button");
            this._increment_button.ForeColor = System.Drawing.Color.Green;
            this._increment_button.Name = "_increment_button";
            this._increment_button.Text = global::ShinyResetApp.Resources.AttemptIncrementText;
            this._increment_button.UseVisualStyleBackColor = false;
            this._increment_button.Click += new System.EventHandler(this.Increment_button_Click);
            // 
            // _timer
            // 
            this._timer.Enabled = true;
            this._timer.Interval = 10;
            this._timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._main_container);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this._main_container.Panel1.ResumeLayout(false);
            this._main_container.Panel1.PerformLayout();
            this._main_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._main_container)).EndInit();
            this._main_container.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this._split_container.Panel1.ResumeLayout(false);
            this._split_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._split_container)).EndInit();
            this._split_container.ResumeLayout(false);
            this._shiny_split_container.Panel1.ResumeLayout(false);
            this._shiny_split_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._shiny_split_container)).EndInit();
            this._shiny_split_container.ResumeLayout(false);
            this._attempt_split_container.Panel1.ResumeLayout(false);
            this._attempt_split_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._attempt_split_container)).EndInit();
            this._attempt_split_container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _split_container;
        private System.Windows.Forms.SplitContainer _attempt_split_container;
        private System.Windows.Forms.SplitContainer _shiny_split_container;
        private System.Windows.Forms.Button _increment_button;
        private System.Windows.Forms.Button _decrement_button;
        private System.Windows.Forms.Button _shiny_increment_button;
        private System.Windows.Forms.Button _shiny_decrement_button;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.Label _detail_label;
        private System.Windows.Forms.SplitContainer _main_container;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

