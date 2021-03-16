using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinAudioLevels {
    public partial class ObsTestTheme : Form {
        private static readonly Dictionary<string, ObsTheme> OBS_THEMES = new Dictionary<string, ObsTheme>() {
            { "Acri", ObsTheme.ACRI },
            { "Dark", ObsTheme.DARK },
            { "System", ObsTheme.SYSTEM },
            { "Rachni", ObsTheme.RACHNI }
        };
        /*
         {
                "Arial",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "Arial"
                     }
                }
            }, {
                "Arial Unicode MS",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "Arial Unicode MS"
                     }
                }
            }, {
                "Courier New",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "Courier New"
                     }
                }
            }, {
                "MS Gothic",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "MS Gothic"
                     }
                }
            }, {
                "Segoe UI",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "Segoe UI"
                     }
                }
            }, {
                "Segoe UI Emoji",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "Segoe UI Emoji"
                     }
                }
            }, {
                "SimSun",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "SimSun"
                     }
                }
            }, {
                "Tahoma",
                new ObsTheme(){
                     backgroundColor = Color.FromArgb(31,30,31),
                     textColor = Color.FromArgb(225,224,225),
                     fontSizeUnit = GraphicsUnit.Pixel,
                     fontSizeValue = 10,
                     fontFamily = new string[] {
                         "Tahoma"
                     }
                }
            }
        */
        public ObsTestTheme() {
            this.InitializeComponent();
            this.flowLayoutPanel1.SuspendLayout();
            foreach (KeyValuePair<string, ObsTheme> theme in OBS_THEMES) {
                this.flowLayoutPanel1.Controls.Add(new Label() {
                    Text = string.Format("{0} - THE QUICK BROWN FOX JUMPED OVER THE LAZY DOG.", theme.Key),
                    Name = string.Format("{0} - THE QUICK BROWN FOX JUMPED OVER THE LAZY DOG.", theme.Key),
                    Padding = new Padding(15),
                    BackColor = theme.Value.backgroundColor,
                    ForeColor = theme.Value.textColor,
                    Font = theme.Value.Font ?? this.Font,
                    AutoSize = true,
                    Margin = new Padding(5),
                });
                this.flowLayoutPanel1.Controls.Add(new Label() {
                    Text = string.Format("{0} - the quick brown fox jumped over the lazy dog.", theme.Key),
                    Name = string.Format("{0} - the quick brown fox jumped over the lazy dog.", theme.Key),
                    Padding = new Padding(15),
                    BackColor = theme.Value.backgroundColor,
                    ForeColor = theme.Value.textColor,
                    Font = theme.Value.Font ?? this.Font,
                    AutoSize = true,
                    Margin = new Padding(5),
                });
            }/*
            foreach (FontFamily family in FontFamily.Families) {
                this.flowLayoutPanel1.Controls.Add(new Label() {
                    Text = string.Format("ABCDEFGHIJKLMNOPQRSTUVWXYZ - {0}", family.Name),
                    Name = string.Format("ABCDEFGHIJKLMNOPQRSTUVWXYZ - {0}", family.Name),
                    Padding = new Padding(15),
                    BackColor = Color.FromArgb(31, 30, 31),
                    ForeColor = Color.FromArgb(225, 224, 225),
                    Font = new Font(family, 10, GraphicsUnit.Pixel),
                    AutoSize = true,
                    Margin = new Padding(5),
                });
            }*/
            this.flowLayoutPanel1.Controls.Add(ObsTheme.ACRI.RenderBox("Audio Cap"));
            this.flowLayoutPanel1.Controls.Add(ObsTheme.DARK.RenderBox("Audio Cap"));
            this.flowLayoutPanel1.Controls.Add(ObsTheme.RACHNI.RenderBox("Audio Cap"));
            this.flowLayoutPanel1.Controls.Add(ObsTheme.SYSTEM.RenderBox("Audio Cap"));
            this.flowLayoutPanel1.Controls.Add(ObsTheme.ACRI.RenderBox("Headphones Output"));
            this.flowLayoutPanel1.Controls.Add(ObsTheme.ACRI.RenderBox("Speaker Audio Output"));
            this.flowLayoutPanel1.Controls.Add(ObsTheme.ACRI.RenderBox("Audio Cap"));
            this.flowLayoutPanel1.Controls.Add(ObsTheme.ACRI.RenderBox("Microphone Input"));
            this.flowLayoutPanel1.ResumeLayout();
        }
    }
}