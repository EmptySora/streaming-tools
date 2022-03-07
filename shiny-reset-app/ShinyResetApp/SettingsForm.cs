using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShinyResetApp {
    partial class SettingsForm : Form {
        private bool Modified =>
               ((ulong)this._treset_box.Value != this.Settings.Resets)
            || ((ulong)this._tshiny_box.Value != this.Settings.Shinies)
            || (this._ravg_box.Text != this.Settings.ResetAverageFile)
            || (this._rcount_box.Text != this.Settings.ResetCountFile);

        private Settings Settings { get; set; }

        public bool Updated { get; private set; } = false;

        public SettingsForm(Settings settings) {
            this.InitializeComponent();
            this.Settings = settings;
            this._ravg_box.Text = this.Settings.ResetAverageFile;
            this._rcount_box.Text = this.Settings.ResetCountFile;
            this._treset_box.Value = this.Settings.Resets;
            this._tshiny_box.Value = this.Settings.Shinies;
        }

        private void Ok_button_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            if (this.Modified) {
                this.Save();
            }

            this.Close();
        }

        private void Cancel_button_Click(object sender, EventArgs e) {
            if (this.Modified) {
                switch (MessageBox.Show(Resources.UnsavedChangesWarningText, Resources.UnsavedChangesWarningTitleText, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3)) {
                case DialogResult.Yes:
                    this.Save();
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    return;
                }
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Apply_button_Click(object sender, EventArgs e) {
            if (this.Modified) {
                this.Save();
            }
        }

        private void Save() {
            this.Settings.ResetAverageFile = this._ravg_box.Text;
            this.Settings.ResetCountFile = this._rcount_box.Text;
            this.Settings.Resets = (ulong)this._treset_box.Value;
            this.Settings.Shinies = (ulong)this._tshiny_box.Value;
            this.Settings.Save(null);
            this.Updated = true;
        }

        private void UpdateInterface() {
            this._apply_button.Enabled = this.Modified;
            this._ravg_box.ForeColor = this._ravg_box.Text != this.Settings.ResetAverageFile 
                ? Color.Red 
                : SystemColors.WindowText;
            this._rcount_box.ForeColor = this._rcount_box.Text != this.Settings.ResetCountFile
                ? Color.Red
                : SystemColors.WindowText;
            this._treset_box.ForeColor = (ulong)this._treset_box.Value != this.Settings.Resets
                ? Color.Red
                : SystemColors.WindowText;
            this._tshiny_box.ForeColor = (ulong)this._tshiny_box.Value != this.Settings.Shinies
                ? Color.Red
                : SystemColors.WindowText;
        }

        private static bool BrowseFile(string title, out string? file, string? baseFile) {
            file = null;
            using OpenFileDialog dialog = new OpenFileDialog() {
                AddExtension = false,
                AutoUpgradeEnabled = true,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = baseFile ?? "",
                DefaultExt = "txt",
                DereferenceLinks = true,
                Filter = Resources.OpenFileDialogFilterText,
                FilterIndex = 0,
                InitialDirectory = Path.GetDirectoryName(baseFile) ?? Environment.ExpandEnvironmentVariables(@"%userprofile%\"),
                Multiselect = false,
                ReadOnlyChecked = false,
                RestoreDirectory = true,
                ShowHelp = false,
                ShowReadOnly = false,
                SupportMultiDottedExtensions = true,
                ValidateNames = false,
                Title = title
            };
            switch (dialog.ShowDialog()) {
            default:
            case DialogResult.Cancel:
                return false;
            case DialogResult.OK:
                file = dialog.FileName;
                return true;
            }
        }

        private void Rcount_browse_button_Click(object sender, EventArgs e) {
            if (BrowseFile(Resources.SelectResetCountFileText, out string? file, this._rcount_box.Text)) {
                this._rcount_box.Text = file;
            }

            this.UpdateInterface();
        }

        private void Ravg_browse_button_Click(object sender, EventArgs e) {
            if (BrowseFile(Resources.SelectResetAverageFileText, out string? file, this._ravg_box.Text)) {
                this._ravg_box.Text = file;
            }

            this.UpdateInterface();
        }

        #region Update Events
        private void Rcount_box_TextChanged(object sender, EventArgs e) {
            this.UpdateInterface();
        }

        private void Ravg_box_TextChanged(object sender, EventArgs e) {
            this.UpdateInterface();
        }

        private void Treset_box_ValueChanged(object sender, EventArgs e) {
            this.UpdateInterface();
        }

        private void Tshiny_box_ValueChanged(object sender, EventArgs e) {
            this.UpdateInterface();
        }

        private void Tshiny_box_KeyUp(object sender, KeyEventArgs e) {
            this.UpdateInterface();
        }

        private void Treset_box_KeyUp(object sender, KeyEventArgs e) {
            this.UpdateInterface();
        }

        private void Ravg_box_KeyUp(object sender, KeyEventArgs e) {
            this.UpdateInterface();
        }

        private void Rcount_box_KeyUp(object sender, KeyEventArgs e) {
            this.UpdateInterface();
        }
        #endregion
    }
}
