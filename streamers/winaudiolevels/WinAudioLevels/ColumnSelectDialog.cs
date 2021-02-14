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
    public partial class ColumnSelectDialog : Form {
        private readonly Dictionary<string,Tuple<string,string,int>> _columns; //column.name, column.text, help text, status
        //0 = enabled, 1 = force enabled, -1 = disabled
        [Obsolete("This method is required by Visual Studio. Use the parameterized version of this constructor instead.",false)]
        public ColumnSelectDialog() {
            this.InitializeComponent();
        }
#pragma warning disable CS0618 //Method is obsolete.
        /// <summary>
        /// Creates a new <see cref="ColumnSelectDialog"/> with the specified columns.
        /// </summary>
        /// <param name="columns">The columns to select from.</param>
        /// <param name="columnDisplayNames">The names of the columns.</param>
        /// <param name="helpText">Informational text pertaining to each column.</param>
        /// <param name="columnStates">
        /// A value indicating whether the column is forcibly enabled (<c>1</c>), 
        /// enabled (<c>0</c>), or disabled (<c>-1</c>).
        /// </param>
        public ColumnSelectDialog(string[] columns, string[] columnDisplayNames, string[] helpText, int[] columnStates)
            : this() {
            this._columns = columns
                .Zip(helpText, (a, b) => new Tuple<string, string>(a, b))
                .Zip(columnDisplayNames, (a, b) => new Tuple<string, string, string>(a.Item1, a.Item2, b))
                .Zip(columnStates, (a, b) => new Tuple<string, string, string, int>(a.Item1, a.Item2, a.Item3, b))
                .ToDictionary(a => a.Item2, a => new Tuple<string, string, int>(a.Item1, a.Item3, a.Item4));
            this.ResetColumnLists();
            this.RefreshButtons();
        }
#pragma warning restore CS0618 //Method is obsolete.

        public bool AllowReorder { get; set; } = true;
        private IEnumerable<string> InitiallyActiveColumns => this._columns
            .Where(a => a.Value.Item3 >= 0)
            .Select(a => a.Key);
        public string[] ActiveColumns => this
            .activeColumnsList
            .Items
            .Cast<string>()
            .ToArray();
        public string[] ActiveColumnIds => this.ActiveColumns.Select(a => this._columns[a].Item1).ToArray();
        private void ResetColumnLists() {
            this.inactiveColumnsList.BeginUpdate();
            this.activeColumnsList.BeginUpdate();
            this.inactiveColumnsList.Items.Clear();
            this.activeColumnsList.Items.Clear();
            foreach (KeyValuePair<string, Tuple<string, string, int>> pair in this._columns) {
                if (pair.Value.Item3 < 0) {
                    this.inactiveColumnsList.Items.Add(pair.Key);
                } else {
                    this.activeColumnsList.Items.Add(pair.Key);
                }
            }
            this.inactiveColumnsList.EndUpdate();
            this.activeColumnsList.EndUpdate();
        }
        private void RefreshButtons() {
            if (this.activeColumnsList.SelectedItem is string selActive) {
                this.hideButton.Enabled = this._columns[selActive].Item3 <= 0;
                this.shiftTopButton.Enabled = this.activeColumnsList.SelectedIndex > 0 && this.AllowReorder;
                this.shiftUpButton.Enabled = this.activeColumnsList.SelectedIndex > 0 && this.AllowReorder;
                this.shiftDownButton.Enabled 
                    = this.activeColumnsList.SelectedIndex < this.activeColumnsList.Items.Count - 1 && this.AllowReorder;
                this.shiftBottomButton.Enabled 
                    = this.activeColumnsList.SelectedIndex < this.activeColumnsList.Items.Count - 1 && this.AllowReorder;
            } else {
                this.hideButton.Enabled =
                    this.shiftTopButton.Enabled =
                    this.shiftUpButton.Enabled =
                    this.shiftDownButton.Enabled =
                    this.shiftBottomButton.Enabled = false;
            }
            this.showButton.Enabled = this.inactiveColumnsList.SelectedItem is string;
            this.okButton.Enabled = !this.ActiveColumns.SequenceEqual(this.InitiallyActiveColumns);
        }
        private void RefreshHelp(bool active = true) {
            this.informationBox.Text = active
                ? this.activeColumnsList.SelectedItem is string selActive
                    ? this._columns[selActive].Item2
                    : ""
                : this.inactiveColumnsList.SelectedItem is string selInactive
                    ? this._columns[selInactive].Item2
                    : "";
        }
        private void RichTextBox1_SelectionChanged(object sender, EventArgs e) {
            this.informationBox.SelectionChanged -= this.RichTextBox1_SelectionChanged;
            this.informationBox.SelectionLength = 0;
            this.informationBox.SelectionChanged += this.RichTextBox1_SelectionChanged;
        }

        private void OkButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ShowButton_Click(object sender, EventArgs e) {
            this.inactiveColumnsList.BeginUpdate();
            this.activeColumnsList.BeginUpdate();
            string item = this.inactiveColumnsList.SelectedItem as string;
            int index = this.inactiveColumnsList.SelectedIndex;
            this.inactiveColumnsList.Items.RemoveAt(index);
            this.activeColumnsList.Items.Add(item);
            this.inactiveColumnsList.SelectedIndex = index;
            this.inactiveColumnsList.EndUpdate();
            this.activeColumnsList.EndUpdate();
            this.RefreshButtons();
        }

        private void HideButton_Click(object sender, EventArgs e) {
            this.inactiveColumnsList.BeginUpdate();
            this.activeColumnsList.BeginUpdate();
            string item = this.activeColumnsList.SelectedItem as string;
            int index = this.activeColumnsList.SelectedIndex;
            this.activeColumnsList.Items.RemoveAt(index);
            this.inactiveColumnsList.Items.Add(item);
            this.activeColumnsList.SelectedIndex = index;
            this.inactiveColumnsList.EndUpdate();
            this.activeColumnsList.EndUpdate();
            this.RefreshButtons();
        }

        private void ShiftTopButton_Click(object sender, EventArgs e) {
            this.activeColumnsList.BeginUpdate();
            string item = this.activeColumnsList.SelectedItem as string;
            int index = this.activeColumnsList.SelectedIndex;
            this.activeColumnsList.Items.RemoveAt(index);
            this.activeColumnsList.Items.Insert(0, item);
            this.activeColumnsList.SelectedIndex = 0;
            this.activeColumnsList.EndUpdate();
            this.RefreshButtons();
        }

        private void ShiftUpButton_Click(object sender, EventArgs e) {
            this.activeColumnsList.BeginUpdate();
            string item = this.activeColumnsList.SelectedItem as string;
            int index = this.activeColumnsList.SelectedIndex;
            this.activeColumnsList.Items.RemoveAt(index);
            this.activeColumnsList.Items.Insert(index - 1, item);
            this.activeColumnsList.SelectedIndex = index - 1;
            this.activeColumnsList.EndUpdate();
            this.RefreshButtons();
        }

        private void ShiftDownButton_Click(object sender, EventArgs e) {
            this.activeColumnsList.BeginUpdate();
            string item = this.activeColumnsList.SelectedItem as string;
            int index = this.activeColumnsList.SelectedIndex;
            this.activeColumnsList.Items.RemoveAt(index);
            this.activeColumnsList.Items.Insert(index + 1, item);
            this.activeColumnsList.SelectedIndex = index + 1;
            this.activeColumnsList.EndUpdate();
            this.RefreshButtons();
        }

        private void ShiftBottomButton_Click(object sender, EventArgs e) {
            this.activeColumnsList.BeginUpdate();
            string item = this.activeColumnsList.SelectedItem as string;
            int index = this.activeColumnsList.SelectedIndex;
            this.activeColumnsList.Items.RemoveAt(index);
            this.activeColumnsList.Items.Add(item);
            this.activeColumnsList.SelectedIndex = this.activeColumnsList.Items.Count - 1;
            this.activeColumnsList.EndUpdate();
            this.RefreshButtons();
        }

        private void ActiveColumnsList_SelectedIndexChanged(object sender, EventArgs e) {
            this.RefreshButtons();
            this.RefreshHelp(true);
        }

        private void InactiveColumnsList_SelectedIndexChanged(object sender, EventArgs e) {
            this.RefreshButtons();
            this.RefreshHelp(false);
        }
    }
}
