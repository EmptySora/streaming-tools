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
    public struct ObsTheme {
        private static readonly Dictionary<string, FontFamily> GENERIC_FAMILIES = new Dictionary<string, FontFamily>() {
                { "@sans serif", FontFamily.GenericSansSerif },
                { "@serif", FontFamily.GenericSerif },
                { "@monospace", FontFamily.GenericMonospace }
            };
        private static Font GetFontFromList(GraphicsUnit units, float size, params string[] fonts) {
            FontFamily fam = null;
            /*
            Console.WriteLine(
                "Current fonts: {0}",
                string.Join(", ", FontFamily.Families.Select(a => a.Name).ToArray()));
            */
            foreach (string fontName in fonts.Select(a => a.ToLower())) {
                bool @break = false;
                if (fontName.StartsWith("@")) {
                    if (GENERIC_FAMILIES.ContainsKey(fontName)) {
                        //Console.WriteLine("Generic font match: {0}", fontName);
                        fam = GENERIC_FAMILIES[fontName];
                        break;
                    }
                    continue;
                }
                foreach (FontFamily family in FontFamily.Families) {
                    if (family.Name.ToLower() == fontName) {
                        //Console.WriteLine("Font match: {0}", family.Name);
                        @break = true;
                        fam = family;
                        break;
                    }
                }
                if (@break) {
                    break;
                }
            }
            return fam is null
                ? null
                : new Font(fam, size, FontStyle.Regular, units);
        }
        public Color backgroundColor;
        public Color textColor;
        public string[] fontFamily;
        public GraphicsUnit fontSizeUnit;
        public float fontSizeValue;
        public int marginLeft;
        public int marginBottom;
        public string name;
        public Font Font => GetFontFromList(this.fontSizeUnit, this.fontSizeValue, this.fontFamily);

        public static readonly ObsTheme ACRI = new ObsTheme() {
            backgroundColor = Color.FromArgb(24, 24, 25),
            textColor = Color.FromArgb(225, 224, 225),
            fontSizeUnit = GraphicsUnit.Pixel,
            fontSizeValue = 12,
            fontFamily = new string[] {
                "Open Sans",
                "Tahoma",
                "Arial",
                "@sans serif"
            },
             marginLeft = 4,
             marginBottom = 2,
             name="Acri",
        };
        public static readonly ObsTheme DARK = new ObsTheme() {
            backgroundColor = Color.FromArgb(31, 30, 31),
            textColor = Color.FromArgb(225, 224, 225),
            fontSizeUnit = GraphicsUnit.Pixel,
            fontSizeValue = 10,
            fontFamily = new string[] {
                "Segoe UI"
            },
            marginLeft = 5,
            marginBottom = 2,
            name = "Dark",
        };
        public static readonly ObsTheme SYSTEM = new ObsTheme() {
            backgroundColor = Color.FromArgb(240, 240, 240),
            textColor = Color.FromArgb(0, 0, 0),
            fontSizeUnit = GraphicsUnit.Pixel,
            fontSizeValue = 10,
            fontFamily = new string[] {
                "Segoe UI"
            },
            marginLeft = 5,
            marginBottom = 2,
            name="System",
        };
        public static readonly ObsTheme RACHNI = new ObsTheme() {
            backgroundColor = Color.FromArgb(49, 54, 59),
            textColor = Color.FromArgb(239, 240, 241),
            fontSizeUnit = GraphicsUnit.Pixel,
            fontSizeValue = 11,
            fontFamily = new string[] {
                "Noto Sans",
                "Tahoma"
            },
            marginLeft = 4, //this might actually be 5 if the gray border counts as part of the borderless capture. test this.
            marginBottom = 2,
            name="Rachni",
        };

        public Bitmap RenderText(string text) {
            Font f = this.Font;
            SizeF rSize = default;
            using(Bitmap bmp = new Bitmap(2, 2)) {
                using(Graphics g = Graphics.FromImage(bmp)) {
                    rSize = TextRenderer.MeasureText(g, text, f, default, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                }
            }
            Bitmap bitmap = new Bitmap((int)(rSize.Width + 25), 25);
            using(Graphics g = Graphics.FromImage(bitmap)) {
                //clear bg
                g.Clear(this.backgroundColor);
                Rectangle drawRectB = new Rectangle(
                    new Point(this.marginLeft, (int)(bitmap.Height - rSize.Height - this.marginBottom) + 1),
                    new Size((int)rSize.Width, (int)rSize.Height));
                //padding of six...?   7    x = ?     4 - x = 4
                TextRenderer.DrawText(g, text, f, drawRectB, this.textColor, this.backgroundColor, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            }
            //ocr rect: (where top is the bounds for the topmost audio meter)
            //x = 0                             #leftmost pixel
            //y = max(0, top.Y - 26)            #at most 26px above the top of the meter
            //width = top.Width + top.X - 70    #70px less than the rightmost point of the meter
            //height = 25 + min(0, top.Y - 26)  #at most 25px
            //so, the 25 rows above the top of the topmost meter cropped to 70 less than the rightmost pixel of the meter

            //text render start appears to be even with the left margin (dark is 5px, and acri is 4px)
            //maybe add those margins to the theme values.
            //oh god, I just remembered, this is going to be a hassle to render due to vertical alignment...
            //I might render it with a rectf

            //height = 25, width = sizeofString + 25
            return bitmap;
        }
        public PictureBox RenderBox(string text) {
            PictureBox box = new PictureBox() {
                Image = this.RenderText(text)
            };
            box.Size = box.Image.Size;
            return box;
        }

        public string GetId(string meterName) {
            return OBSCapture.GetMeterId(this, meterName);
        }

    }
}
/*
Font match: Tahoma
Hash String: "116.13.AAAAAAAA4D/8h/8AASDAf/gP/wEO8Af+QBJ4Am/ABcSBPJAF/sAf8AM+4A/8gSD+x//4H/gP/+E/BIE/8Ac8+B//438gAPyBP+AHfMAf+ANB4A/8AR/wB/7AHwgAf+AP+AEO8Af+QBJ4Am/ABbiBN/AE9sAegAAAAAAAgA/4g/8wGALCYPgP/oAP8AP+wB8AAn/gD/zhD/yH/0AQCAL/4T/8hyDwB/6AB/gBf+APAIE/8Af+8Af+w38gCAQB"
IDs Match
EXPECTED, TRUE: "T1A2c7yAjELCRqciAMIfqA" "T1A2c7yAjELCRqciAMIfqA" (Headphones Output)

Font match: Tahoma
Hash String: "127.13.AAAAAAAAwDF8ho8QEeLDfBgH/If/8B+CwB/4Ax6AA/yBP5AEnsAbcAFxIA9kgT/wB/z4H//jf+AHzIEwwAX+wB9IAk/gDbiAP/AH/kAACAAAAAAAADjgh3/wA37AP8APwIEf8Af+ABD4A3/gD/iAP/AHgvgf/+N/6A/9oT/wB/5AEPgDf8AHAAAAAAAAwAf8wX8YDAFhMPwHf8AH+AF/4A8AgT/wB/7wB/7DfyAIBIH/8B/+QxD4A3/AA/yAP/AHgMAf+AN/+AP/4T8QBIIA"
IDs Match
EXPECTED, TRUE: "WW2S6OP8F0/rpBP2bGp2jA" "WW2S6OP8F0/rpBP2bGp2jA" (Speaker Audio Output)

Font match: Tahoma
Hash String: "59.13.AAAAAAAAADjgh3/wA37AP8APwIEf8Af+ABD4A3/gD/iAP/AHgvgf/+N/6A/9oT/wB/5AEPgDf8AHAAAAAAAAwAf8wX8YDAFhMAwG40AeyAJ/4A/4gf/wH/5DEPgDf8AD"
IDs Match
EXPECTED, TRUE: "Jrk68psf0AM9uuGgh7Vg3Q" "Jrk68psf0AM9uuGgh7Vg3Q" (Audio Cap)

Font match: Tahoma
Hash String: "101.13.AAAAAAAA4D/8h//gB/CAH/gP/+E/9If+0B/4A38gCASBIPAH/sAfCAA/4A/8gSDwB/6AD/gP/+E/BIE/8Ac8+B//438gAPyBP+AHfMAf+ANB4A/8AR/wB/7AHwgAf+AP+AEO8Af+QBJ4Am/ABQAAAAAAABAQ/sN/+A/9gT/wBwLAH/gDfuA//If/EAT+wB/wAD/gD/wBIPAH/sAf/sB/+A8EgSAA"
IDs Match
EXPECTED, TRUE: "jbRrAIQ/VUEePKlSv4Ou/A" "jbRrAIQ/VUEePKlSv4Ou/A" (Microphone Input)
 * */
