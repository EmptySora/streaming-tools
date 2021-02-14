using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace WinAudioLevels {
    public class Prompt : Form {
        private Button _button_1;
        private Button _button_2;
        private Button _button_3;
        private TextBox _text_box_1;
        private TextBox _text_box_2;
        private void TextBox1_TextChanged(object sender, EventArgs e) {
            this.Resizeall();
        }
        private void Resizeall() {
            this._text_box_1.Height = TextRenderer.MeasureText(this._text_box_1.Text, this._text_box_1.Font).Height;
            this._text_box_2.Top = this._text_box_1.Top + this._text_box_1.Height + 3;
            this._button_1.Top = this._button_2.Top = this._button_3.Top = this._text_box_2.Top + this._text_box_2.Height + 3;
            this.Height = this._text_box_2.Top + this._text_box_2.Height + 3 + 20 + 3;
        }
        public string Caption {
            get => this._text_box_1.Text;
            set => this._text_box_1.Text = value;
        }
        public bool Required {
            get => this.ControlBox;
            set => this.ControlBox = value;
        }
        public string Value {
            get => this._text_box_2.Text;
            set => this._text_box_2.Text = value;
        }
        public Prompt(string Message) {
            this.SetupPrompt(Message, "", "", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        public Prompt(string Message, string Title) {
            this.SetupPrompt(Message, Title, "", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        public Prompt(string Message, string Title, string InitialValue) {
            this.SetupPrompt(Message, Title, InitialValue, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        public Prompt(string Message, string Title, string InitialValue, MessageBoxButtons Buttons) {
            this.SetupPrompt(Message, Title, InitialValue, Buttons, MessageBoxIcon.None);
        }
        public Prompt(string Message, string Title, string InitialValue, MessageBoxButtons Buttons, MessageBoxIcon Icon) {
            this.SetupPrompt(Message, Title, InitialValue, Buttons, Icon);
        }
        private void SetupPrompt(string Message, string Title, string InitialValue, MessageBoxButtons Buttons, MessageBoxIcon XIcon) {
            this.InitializeComponent();
            this._text_box_1.Text = Message;
            this._text_box_2.Text = InitialValue;
            this.Text = Title;
            this.Resizeall();
            this.SetButtons(Buttons);
            if (XIcon == MessageBoxIcon.None) {
                this.ShowIcon = false;
            } else {
                this.Icon = (Icon)(typeof(SystemIcons).GetProperty(Enum.GetName(typeof(MessageBoxIcon), XIcon), BindingFlags.Static | BindingFlags.Public).GetValue(null, null));
            }

            this._text_box_2.Focus();
        }
        private void SetButtons(MessageBoxButtons buttons) {
            switch (buttons) {
            case MessageBoxButtons.AbortRetryIgnore:
                this._button_1.Text = "Abort";
                this._button_2.Text = "Retry";
                this._button_3.Text = "Ignore";
                this.AcceptButton = this._button_2;
                this.CancelButton = this._button_1;
                break;
            case MessageBoxButtons.OK:
                this._button_1.Visible = this._button_2.Visible = false;
                this._button_3.Text = "OK";
                this.AcceptButton = this._button_3;
                break;
            case MessageBoxButtons.OKCancel:
                this._button_1.Visible = false;
                this._button_2.Text = "OK";
                this._button_3.Text = "Cancel";
                this.AcceptButton = this._button_2;
                this.CancelButton = this._button_3;
                break;
            case MessageBoxButtons.RetryCancel:
                this._button_1.Visible = false;
                this._button_2.Text = "Retry";
                this.AcceptButton = this._button_2;
                this._button_3.Text = "Cancel";
                this.CancelButton = this._button_3;
                break;
            case MessageBoxButtons.YesNo:
                this._button_1.Visible = false;
                this.AcceptButton = this._button_2;
                this.CancelButton = this._button_3;
                this._button_2.Text = "Yes";
                this._button_3.Text = "No";
                break;
            case MessageBoxButtons.YesNoCancel:
                this.AcceptButton = this._button_1;
                this.CancelButton = this._button_3;
                this._button_1.Text = "Yes";
                this._button_2.Text = "No";
                this._button_3.Text = "Cancel";
                break;
            }
        }
        private void SetDialogResult(string buttons) {
            this.DialogResult = (DialogResult)Enum.Parse(typeof(DialogResult), buttons);
        }
        private void Button1_Click(object sender, EventArgs e) {
            this.SetDialogResult(this._button_1.Text);
        }
        private void Button2_Click(object sender, EventArgs e) {
            this.SetDialogResult(this._button_2.Text);
        }
        private void Button3_Click(object sender, EventArgs e) {
            this.SetDialogResult(this._button_3.Text);
        }
        private readonly IContainer _components = null;
        protected override void Dispose(bool disposing) {
            if (disposing && (this._components != null)) {
                this._components.Dispose();
            }

            base.Dispose(disposing);
        }
        private void InitializeComponent() {
            this._button_1 = new Button();
            this._button_2 = new Button();
            this._button_3 = new Button();
            this._text_box_1 = new TextBox();
            this._text_box_2 = new TextBox();
            this.SuspendLayout();

            this._text_box_1.BackColor = SystemColors.Control;
            this._text_box_1.BorderStyle = BorderStyle.None;
            this._text_box_1.Cursor = Cursors.Default;
            this._text_box_1.Enabled = !(this._text_box_1.Multiline = true);
            this._button_1.TabIndex = 0;
            this._button_2.TabIndex = 1;
            this._button_3.TabIndex = 2;
            this._text_box_1.TabIndex = 3;
            this._text_box_2.TabIndex = 4;
            this._button_1.Click += this.Button1_Click;
            this._button_2.Click += this.Button2_Click;
            this._button_3.Click += this.Button3_Click;
            this._text_box_1.TextChanged += this.TextBox1_TextChanged;
            this._button_1.Location = new Point(12, 65);
            this._button_2.Location = new Point(93, 65);
            this._button_3.Location = new Point(174, 65);
            this._text_box_1.Location = new Point(12, 12);
            this._text_box_2.Location = new Point(13, 39);
            this._button_1.Name = this._button_1.Text = "button1";
            this._button_2.Name = this._button_2.Text = "button2";
            this._button_3.Name = this._button_3.Text = "button3";
            this._text_box_1.Name = this._text_box_1.Text = "textBox1";
            this._text_box_2.Name = "textBox2";
            this._button_1.Size = this._button_2.Size = this._button_3.Size = new Size(75, 23);
            this._text_box_1.Size = new Size(237, 21);
            this._text_box_2.Size = new Size(236, 20);
            this._button_1.UseVisualStyleBackColor = this._button_2.UseVisualStyleBackColor = this._button_3.UseVisualStyleBackColor = true;

            this.AutoScaleDimensions = new SizeF(6, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = !(this.ShowInTaskbar = this.MaximizeBox = this.MinimizeBox = false);
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.ClientSize = new Size(261, 92);
            this.Controls.AddRange(new Control[] { this._text_box_2, this._text_box_1, this._button_3, this._button_2, this._button_1 });
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Name = this.Text = "Prompt";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public static string ShowPrompt(string Message, out DialogResult result) {
            Prompt x = new Prompt(Message);
            result = x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title, out DialogResult result) {
            Prompt x = new Prompt(Message, Title);
            result = x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title, string InitialValue, out DialogResult result) {
            Prompt x = new Prompt(Message, Title, InitialValue);
            result = x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title, string InitialValue, MessageBoxButtons Buttons, out DialogResult result) {
            Prompt x = new Prompt(Message, Title, InitialValue, Buttons);
            result = x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title, string InitialValue, MessageBoxButtons Buttons, MessageBoxIcon Icon, out DialogResult result) {
            Prompt x = new Prompt(Message, Title, InitialValue, Buttons, Icon);
            result = x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message) {
            Prompt x = new Prompt(Message);
            x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title) {
            Prompt x = new Prompt(Message, Title);
            x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title, string InitialValue) {
            Prompt x = new Prompt(Message, Title, InitialValue);
            x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title, string InitialValue, MessageBoxButtons Buttons) {
            Prompt x = new Prompt(Message, Title, InitialValue, Buttons);
            x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
        public static string ShowPrompt(string Message, string Title, string InitialValue, MessageBoxButtons Buttons, MessageBoxIcon Icon) {
            Prompt x = new Prompt(Message, Title, InitialValue, Buttons, Icon);
            x.ShowDialog();
            string ret = x.Value;
            x.Dispose();
            return ret;
        }
    }
}
