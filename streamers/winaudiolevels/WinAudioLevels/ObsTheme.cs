using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinAudioLevels {
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
        public Color[] meterColors;
        public Color meterTickColor;
        public Color[] meterBackgroundColors;
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
            name = "Acri",
            meterColors = new Color[] {
                Color.FromArgb(132,216,43), //GREEN: ACTIVE
                Color.FromArgb(66,116,12), //GREEN: INACTIVE
                Color.FromArgb(228,215,23), //YELLOW: ACTIVE
                Color.FromArgb(152,143,15), //YELLOW: INACTIVE
                Color.FromArgb(215,65,22), //RED: ACTIVE
                Color.FromArgb(128,32,4), //RED: INACTIVE
                Color.FromArgb(49,54,59), //Dark thingie
            },
            meterTickColor = Color.FromArgb(239, 240, 241),
            meterBackgroundColors = new Color[] {
                Color.FromArgb(24, 24, 25)
            },
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
            meterColors = new Color[] {
                Color.FromArgb(76,255,76), //GREEN: ACTIVE
                Color.FromArgb(38,127,38), //GREEN: INACTIVE
                Color.FromArgb(255,255,76), //YELLOW: ACTIVE
                Color.FromArgb(127,127,38), //YELLOW: INACTIVE
                Color.FromArgb(255,76,76), //RED: ACTIVE
                Color.FromArgb(127,38,38), //RED: INACTIVE
                Color.FromArgb(0,0,0), //Dark thingie
            },
            meterTickColor = Color.FromArgb(225, 224, 225),
            meterBackgroundColors = new Color[] {
                Color.FromArgb(31, 30, 31)
            },
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
            name = "System",
            meterColors = new Color[] {
                Color.FromArgb(50,200,50), //GREEN: ACTIVE
                Color.FromArgb(15,100,15), //GREEN: INACTIVE
                Color.FromArgb(255,200,50), //YELLOW: ACTIVE
                Color.FromArgb(100,100,15), //YELLOW: INACTIVE
                Color.FromArgb(200,50,50), //RED: ACTIVE
                Color.FromArgb(100,15,15), //RED: INACTIVE
                Color.FromArgb(0,0,0), //Dark thingie
            },
            meterTickColor = Color.FromArgb(0, 0, 0),
            meterBackgroundColors = new Color[] { 
                Color.FromArgb(240, 240, 240),
                Color.FromArgb(130, 135, 144) 
            },
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
            name = "Rachni",
            meterColors = new Color[] {
                Color.FromArgb(119,255,143), //GREEN: ACTIVE
                Color.FromArgb(0,128,79), //GREEN: INACTIVE
                Color.FromArgb(255,157,76), //YELLOW: ACTIVE
                Color.FromArgb(128,57,0), //YELLOW: INACTIVE
                Color.FromArgb(255,89,76), //RED: ACTIVE
                Color.FromArgb(128,9,0), //RED: INACTIVE
                Color.FromArgb(49,54,59), //Dark thingie
            },
            meterTickColor = Color.FromArgb(239, 240, 241),
            meterBackgroundColors = new Color[] { 
                Color.FromArgb(49, 54, 59), 
                Color.FromArgb(118, 121, 124) 
            },
        };
        public static readonly Color[] ALL_METER_COLORS = DARK.meterColors
            .Union(ACRI.meterColors)
            .Union(RACHNI.meterColors)
            .Union(SYSTEM.meterColors)
            .Distinct()
            .ToArray();
        public static readonly ObsTheme[] THEMES = new ObsTheme[] {
            ACRI,
            DARK,
            SYSTEM,
            RACHNI
        };

        public Bitmap RenderText(string text) {
            Font f = this.Font;
            SizeF rSize = default;
            using (Bitmap bmp = new Bitmap(2, 2)) {
                using (Graphics g = Graphics.FromImage(bmp)) {
                    rSize = TextRenderer.MeasureText(g, text, f, default, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                }
            }
            Bitmap bitmap = new Bitmap((int)(rSize.Width + 25), 25);
            using (Graphics g = Graphics.FromImage(bitmap)) {
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

        public static ObsTheme GetThemeByName(string name) {
            switch (name.ToLower()) {
            case "acri":
                return ACRI;
            case "dark":
                return DARK;
            case "rachni":
                return RACHNI;
            case "system":
                return SYSTEM;
            }
            throw new KeyNotFoundException("Could not find the theme with name: " + name);
        }
        public static ObsTheme GetThemeByMeterColor(Color a, Color b) {
            try {
                return THEMES.First(c => c.meterColors.Contains(a) || c.meterColors.Contains(b));
            } catch {
                return default;
            }
        }
        public string GetMeterId(string name) {
            List<List<bool>> bits = new List<List<bool>>();
            Color[] bgColors = this.meterBackgroundColors;
            Bitmap bmp = this.RenderText(name);
            Rectangle rect = new Rectangle(default, bmp.Size);
            //step 1: convert the OCR region to bool[][] where false=bg, true=!bg
            for (int x = rect.X, i = 0; x < Math.Min(rect.Right, bmp.Width); x++, i++) {
                bits.Add(new List<bool>());
                for (int y = rect.Y; y < Math.Min(rect.Bottom, bmp.Height); y++) {
                    bits[i].Add(!bgColors.Contains(bmp.GetPixel(x, y)));
                }
            }
            //step 2: trim the top all false rows and the right all false columns.
            bool check = true;
            while (check) {
                check = false;
                //x,y
                if (bits[bits.Count - 1].All(a => !a)) {
                    bits.RemoveAt(bits.Count - 1);
                    check = true;
                    continue;
                }
                if (bits.All(a => !a[0])) {
                    bits.ForEach(a => a.RemoveAt(0));
                    check = true;
                    continue;
                }
            }
            //step 3: compute hash.
            List<bool> tmp = new List<bool>();
            bits.ForEach(a => tmp.AddRange(a));
            BitArray bitArray = new BitArray(tmp.ToArray());
            byte[] bytes = new byte[(bitArray.Length - 1) / 8 + 1];
            bitArray.CopyTo(bytes, 0);
            string hashString = string.Format(
                "{1}.{2}.{0}",
                Convert.ToBase64String(bytes),
                bits.Count,
                bits.Count == 0 ? 0 : bits[0].Count);
            //Console.WriteLine("Hash String: \"{0}\"", hashString);
            byte[] hash = ObsAudioMixerMeter.HASHER.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            //128-bit, so 16-bytes
            //hex would be 2ch/byte = 32 characters.
            //base64 would be 4ch/3bytes = 24 characters.
            //ehhhhh good enough
            string ret = Convert.ToBase64String(hash).Replace("=", "");
            return ret;
        }
    }
}
/*
ERROR IN OBS CAPTURE LOOP:
System.TypeInitializationException: The type initializer for 'WinAudioLevels.ObsTheme' threw an exception. ---> System.ArgumentNullException: Value cannot be null.
Parameter name: first
   at System.Linq.Enumerable.Union[TSource](IEnumerable`1 first, IEnumerable`1 second)
   at WinAudioLevels.ObsTheme..cctor() in C:\Users\Brandon\source\repos\WinAudioLevels\WinAudioLevels\ObsTheme.cs:line 64
   --- End of inner exception stack trace ---
   at WinAudioLevels.ObsAudioMixerMeter.get_MeterCount() in C:\Users\Brandon\source\repos\WinAudioLevels\WinAudioLevels\ObsAudioMixerMeter.cs:line 132
   at WinAudioLevels.ObsAudioMixerMeter.RefreshMeterLists() in C:\Users\Brandon\source\repos\WinAudioLevels\WinAudioLevels\ObsAudioMixerMeter.cs:line 312
   at WinAudioLevels.OBSCapture.CaptureMain() in C:\Users\Brandon\source\repos\WinAudioLevels\WinAudioLevels\OBSCapture.cs:line 263
   at WinAudioLevels.OBSCapture.CaptureLoop() in C:\Users\Brandon\source\repos\WinAudioLevels\WinAudioLevels\OBSCapture.cs:line 226
*/
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
