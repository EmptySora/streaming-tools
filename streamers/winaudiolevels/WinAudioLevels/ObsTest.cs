using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace WinAudioLevels {
    public partial class ObsTest : Form {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private readonly List<Rectangle> _ocr_rects = new List<Rectangle>();
        private readonly object _ocr_lock = new object();
        private Image _image;
        private readonly object _image_lock = new object();
        public ObsTest() {
            this.InitializeComponent();
            this.FormClosed += this.ObsTest_FormClosed;
            this.DoTest();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        /*
        private static int GCD(params int[] numbers) {
            int innerGCD(int n, int m) {
                if(n > m) {
                    return innerGCD(m, n); //flip them to simplify some arithmetic.
                }
                int rem;
                while (n != 0) {
                    rem = m % n;
                    m = n;
                    n = rem;
                }
                return m;
                //8 12: 4; 8 12 18: 2 (4, 6: 2)
            }
            if (numbers.Length == 0) {
                throw new ArgumentException("Need at least one number!");
            } else if(numbers.Length == 1) {
                return numbers[0];
            }
            List<int> nums = numbers.ToList();
            List<int> numbs = new List<int>();
            int g;
            while(nums.Count > 1) {
                for(int i = 1; i < nums.Count; i++) {
                    numbs.Add(g = innerGCD(nums[i - 1], nums[i]));
                    if(g == 1) {
                        return 1; //if it's 1, it'll always be one...
                    }
                }
                nums = numbs;
                numbs = new List<int>();
            }
            return nums[0];
        }
        */
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            lock (this._image_lock) {
                if(this._image is null) {
                    return;
                }
                SizeF clientArea = this.ClientSize;
                SizeF pictureSize = this._image.Size;
                float ratio;
                if (pictureSize.Width > clientArea.Width) {
                    ratio = clientArea.Width / pictureSize.Width;
                    pictureSize.Width = clientArea.Width;
                    pictureSize.Height = ratio * pictureSize.Height;
                }
                if (pictureSize.Height > clientArea.Height) {
                    ratio = clientArea.Height / pictureSize.Height;
                    pictureSize.Height = clientArea.Height;
                    pictureSize.Width = ratio * pictureSize.Width;
                }
                //image is officially smaller than window.
                PointF picturePosition = new PointF(
                    (clientArea.Width - pictureSize.Width) / 2,
                    (clientArea.Height - pictureSize.Height) / 2);
                RectangleF result = new RectangleF(picturePosition, pictureSize);
                e.Graphics.DrawImage(this._image, result);
                using(Bitmap bmp = new Bitmap(this._image.Width, this._image.Height)) {
                    using(Graphics gfx = Graphics.FromImage(bmp)) {
                        //hopefully, this is transparent to begin with...
                        lock (this._ocr_lock) {
                            if (this._ocr_rects.Count > 0) {
                                using (Pen pen = new Pen(Color.Red, 1)) {
                                    gfx.DrawRectangles(pen, this._ocr_rects.ToArray());
                                }
                            }
                        }
                    }
                    //we draw to a bitmap first so we can apply the same transformation the window capture had to all the rects.
                    //removes the complex math from the problem. We just have to hope it defaults to transparent (which seems to be the case...)
                    e.Graphics.DrawImage(bmp, result); //draw rect on top.
                }
            }
        }

        private void ObsTest_FormClosed(object sender, FormClosedEventArgs e) {
            OBSCapture.UnregisterCapture();
        }

        private void DoTest() {
            Console.WriteLine("WE ARE A {0}-bit PROCESS", Environment.Is64BitProcess ? 64 : 32);
            OBSCapture.AudioMixerWindowCaptured += this.OBSCapture_AudioMixerWindowCaptured;
            OBSCapture.AudioMixerWindowNotFoundError += this.OBSCapture_AudioMixerWindowNotFoundError;
            OBSCapture.OBSErrorFixed += this.OBSCapture_OBSErrorFixed;
            OBSCapture.OBSNotFoundError += this.OBSCapture_OBSNotFoundError;
            OBSCapture.AudioMixerOcrAttempt += this.OBSCapture_AudioMixerOcrAttempt;
            OBSCapture.AudioMixerOcrStarting += this.OBSCapture_AudioMixerOcrStarting;
            OBSCapture.RegisterCapture();
#warning Check to see if the elevated permissions are actually necessary. Always better to not require them. (Especially since this isn't signed)
        }

        private void OBSCapture_AudioMixerOcrStarting(object sender, EventArgs e) {
            lock (this._ocr_lock) {
                this._ocr_rects.Clear();
                this.Invalidate();
            }
        }

        private void OBSCapture_AudioMixerOcrAttempt(object sender, Tuple<Rectangle, int> e) {
            lock (this._ocr_lock) {
                while(this._ocr_rects.Count <= e.Item2) {
                    this._ocr_rects.Add(default);
                }
                this._ocr_rects[e.Item2] = e.Item1;
                this.Invalidate();
            }
        }

        private void OBSCapture_OBSNotFoundError(object sender, EventArgs e) {
            if (this._dispatcher.Thread != Thread.CurrentThread) {
                this._dispatcher.InvokeAsync(() => {
                    this.OBSCapture_OBSNotFoundError(sender, e);
                });
                return;
            }
            this.Text = "ERROR: OBS CANNOT BE FOUND!";
        }

        private void OBSCapture_OBSErrorFixed(object sender, EventArgs e) {
            if (this._dispatcher.Thread != Thread.CurrentThread) {
                this._dispatcher.InvokeAsync(() => {
                    this.OBSCapture_OBSErrorFixed(sender, e);
                });
                return;
            }
            this.Text = "INFO: ERROR RESOLVED!";
        }

        private void OBSCapture_AudioMixerWindowNotFoundError(object sender, EventArgs e) {
            if (this._dispatcher.Thread != Thread.CurrentThread) {
                this._dispatcher.InvokeAsync(() => {
                    this.OBSCapture_AudioMixerWindowNotFoundError(sender, e);
                });
                return;
            }
            this.Text = "ERROR: CANNOT FIND OBS AUDIO MIXER WINDOW!";
        }

        private void OBSCapture_AudioMixerWindowCaptured(object sender, Image e) {
            if(this._dispatcher.Thread != Thread.CurrentThread) {
                this._dispatcher.InvokeAsync(() => {
                    this.OBSCapture_AudioMixerWindowCaptured(sender, e);
                });
                return;
            }
            Image realImage = (Image)e.Clone();
            lock (this._image_lock) {
                if (!(this._image is null)) {
                    this._image.Dispose();
                    this._image = null;
                }
                this._image = realImage;
                this.Invalidate();
            }
        }
    }
}
