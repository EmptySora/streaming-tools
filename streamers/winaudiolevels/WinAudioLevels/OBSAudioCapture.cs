﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WinAudioLevels {
    class OBSAudioCapture : IAudioCapture, IDisposable  {

        private Thread _thread;
        private readonly object _lock = new object();
        private double[] _last_levels = new double[0];
        private readonly bool _type = false; //false = name, true = index
        private readonly int _index = -1;
        private readonly string _name = null;
        private readonly string _id = null;
        private TimeSpan _wait;

        public long LastSample => this.LastSamples.MaxOrDefault(default);
        public double LastAudioLevel => this.LastAudioLevels.MaxOrDefault(double.NaN);
        public double LastAmplitudePercent => this.LastAmplitudePercents.MaxOrDefault(double.NaN);
        //largest sample out of every channel.

        public IEnumerable<long> LastSamples {
            get {
                foreach(double amp in this.LastAmplitudePercents) {
                    yield return (long)(amp * Math.Pow(2, 31));
                }
            }
        }
        public IEnumerable<double> LastAudioLevels {
            get {
                if (this._disposed_value) {
                    throw new ObjectDisposedException(nameof(OBSAudioCapture));
                }
                lock (this._lock) {
                    foreach (double level in this._last_levels) {
                        yield return level;
                    }
                }
            }
            private set {
                if (this._disposed_value) {
                    throw new ObjectDisposedException(nameof(OBSAudioCapture));
                }
                lock (this._lock) {
                    this._last_levels = value.ToArray();
                }
            }
        }
        public IEnumerable<double> LastAmplitudePercents {
            get {
                foreach(double level in this.LastAudioLevels) {
                    yield return Math.Pow(10, level / 20);
                }
            }
        }
        public bool Valid { get; private set; } = false;

        public void Start() {
            if (this._disposed_value) {
                throw new ObjectDisposedException(nameof(OBSAudioCapture));
            }
            if(!(this._thread is null)) {
                this._thread.Abort();
                OBSCapture.UnregisterCapture();
            }
            this._thread = new Thread(this.CaptureMain) { Name = "OBS Capture Thread" };
            this._thread.Start();
            OBSCapture.RegisterCapture();
        }
        public void Stop() {
            if (this._disposed_value) {
                throw new ObjectDisposedException(nameof(OBSAudioCapture));
            }
            if (this._thread is null) {
                return;
            }
            this._thread.Abort();
            this._thread = null;
            OBSCapture.UnregisterCapture();
        }
        private void CaptureMain() {
            this._wait = TimeSpan.FromMilliseconds(1000F / OBSCapture.FPS);
            if (this._type) {
                while (true) {
                    try {
                        this.CaptureMain_Post(ObsAudioMixerMeter.GetAudioMeterLevel(this._index));
                    } catch (ThreadAbortException) {
                        throw; //rethrow
                    } catch (Exception e) {
                        Console.WriteLine("ERROR in obs audio capture loop: {0}", e);
                    }
                }
            } else if(!(this._id is null)) {
                while (true) {
                    try {
                        this.CaptureMain_Post(ObsAudioMixerMeter.GetAudioMeterLevel(this._id));
                    } catch (ThreadAbortException) {
                        throw; //rethrow
                    } catch (Exception e) {
                        Console.WriteLine("ERROR in obs audio capture loop: {0}", e);
                    }
                }
            } else {
                string themeName = ObsAudioMixerMeter.CurrentObsThemeName;
                string name = null;
                while (true) {
                    try {
                        string cThemeName = ObsAudioMixerMeter.CurrentObsThemeName;
                        if (themeName != cThemeName) {
                            themeName = cThemeName;
                            name = (ObsAudioMixerMeter.CurrentObsTheme ?? ObsTheme.ACRI).GetMeterId(this._name);
                        }
                        if(!(name is null)) {
                            this.CaptureMain_Post(ObsAudioMixerMeter.GetAudioMeterLevel(name));
                        }
                    } catch (ThreadAbortException) {
                        throw; //rethrow
                    } catch (Exception e) {
                        Console.WriteLine("ERROR in obs audio capture loop: {0}", e);
                    }
                }
            }
        }
        private void CaptureMain_Post(double? level) {
            if (!level.HasValue) {
                lock (this._lock) {
                    this._last_levels = new double[0];
                }
                this.Valid = level.HasValue && level.Value != -60;
                Thread.Sleep(this._wait);
                return;
            }

            lock (this._lock) {
                this._last_levels = new double[] { level.Value };
            }
            this.Valid = level.HasValue && level.Value != -60;
            Thread.Sleep(this._wait);
        }

        public OBSAudioCapture(string name, string id = null) {
            this._type = false;
            this._name = name;
            this._id = id;
            //start capturing the OBS Audio Mixer window here.
            //the result will be in dB, so we need to do logbc=a b^a=c ((10^(dB/20))*2^(31)=sample)
            //the 20log_10(amplitude) ignores sign. so, the perceived sample is always positive.
        }
        public OBSAudioCapture(int index) {
            this._type = true;
            this._index = index;
        }

        #region IDisposable Support
        private bool _disposed_value = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!this._disposed_value) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                if(!(this._thread is null)) {
                    OBSCapture.UnregisterCapture();
                    this._thread.Abort();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                this._disposed_value = true;
            }
        }

         ~OBSAudioCapture()
         {
           this.Dispose(false);
         }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
//all of the properties in here that access _last_levels can only access it via a lock.
//that's good. (great, actually. It means I did this right...)