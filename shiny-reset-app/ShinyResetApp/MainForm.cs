using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace ShinyResetApp {
    public partial class MainForm : Form {
        private DateTime _last_click;
        private DateTime _first_click;
        private readonly Stack<DateTime> _time_list;
        private readonly Settings _settings;

        public MainForm() {
            this.InitializeComponent();
            this._settings = Settings.Load(null);
            this._time_list = new Stack<DateTime>();
            this.Form1_Resize(this, new EventArgs());
        }
        #region Events
        private void Shiny_decrement_button_Click(object sender, EventArgs e) {
            this._settings.Shinies--;
            this.UpdateFiles();
        }
        private void Shiny_increment_button_Click(object sender, EventArgs e) {
            this._settings.Shinies++;
            this.UpdateFiles();
        }
        private void Timer_Tick(object sender, EventArgs e) {
            if (this._last_click == default) {
                return;
            }

            this._detail_label.Text = string.Format(Resources.DetailLabelText, DateTime.Now - this._last_click);
        }
        private void Increment_button_Click(object sender, EventArgs e) {
            this._settings.Resets++;
            this._last_click = DateTime.Now;
            if (this._first_click == default) {
                this._first_click = this._last_click;
            }

            this._time_list.Push(this._last_click);
            this.UpdateFiles();
        }
        private void Decrement_button_Click(object sender, EventArgs e) {
            this._settings.Resets--;
            if (this._time_list.Count > 0) {
                _ = this._time_list.Pop();
                this._last_click = this._time_list.Count > 0
                    ? this._time_list.Peek()
                    : default;
            }

            if (this._time_list.Count == 0) {
                this._first_click = default;
            }

            this.UpdateFiles();
        }

        private void Form1_Resize(object sender, EventArgs e) {
            try {
                this._main_container.SplitterDistance = 35 + this.menuStrip1.ClientSize.Height;
                this._split_container.SplitterDistance = this._main_container.Panel2.Width / 2;
                this._attempt_split_container.SplitterDistance = this._split_container.Panel2.ClientSize.Width / 2;
                this._shiny_split_container.SplitterDistance = this._split_container.Panel1.ClientSize.Width / 2;
            } catch { }
        }
        #endregion

        private void UpdateFiles() {
            string GetAverage() {
                return this._first_click != default
                    ? string.Format(Resources.AverageText, (DateTime.Now - this._first_click) / this._time_list.Count)
                    : Resources.AverageUnavailableText;

            }

            static string Odds(double pct) {
                double odds = 100d / pct;
                return odds >= 3d
                    ? string.Format(Resources.OddsText1, Math.Ceiling(odds))
                    : string.Format(Resources.OddsText2, Math.Ceiling(odds * 100d) / 100d);
            }
            /*
             * (1 - [(1 - (4096 ^ -(s + 1))) ^ (3 * (r + 1))]) * 100
             * s = shinies encountered before current attempt
             * r = number of resets/prior attempts.
             * This math may be wrong. (if it is, only the shiny exponent part is wrong. everything else is right.)
             */
            double pct = Math.Round((1d - Math.Pow(1d - (1d / Math.Pow(4096d, this._settings.Shinies + 1d)), 3d * (this._settings.Resets + 1))) * 100d, 2);

            
            File.WriteAllText(this._settings.ResetCountFile, string.Format(Resources.ResetCountText, this._settings.Resets + 1, pct, Odds(pct), this._settings.Shinies));
            File.WriteAllText(this._settings.ResetAverageFile, string.Format(Resources.ResetAverageText, GetAverage()));
            this._settings.Save(null);
        }

        private void ResetToolStripMenuItem_Click(object sender, EventArgs e) {
            this._last_click = default;
            this._first_click = default;
            this._settings.Shinies = 0;
            this._settings.Resets = 0;
            this._time_list.Clear();
            this.UpdateFiles();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e) {
            using SettingsForm settingsForm = new SettingsForm(this._settings);
            _ = settingsForm.ShowDialog();
            if (settingsForm.Updated) {
                this.UpdateFiles();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}