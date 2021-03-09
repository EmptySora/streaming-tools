using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Runtime.InteropServices;

namespace WinAudioLevels {
    class AudioCapture : IAudioCapture {
        private readonly List<IAudioCapture> _captures = new List<IAudioCapture>();
        

        public IEnumerable<double> LastAudioLevels {
            get {
                foreach (IEnumerable<long> @enum in this._captures.Where(capture => capture.Valid).Select(capture => capture.LastAudioLevels)) {
                    foreach (long @long in @enum) {
                        yield return @long;
                    }
                }
            }
        }
        public double LastAudioLevel {
            get {
                try {
                    return this.LastAudioLevels.Max();
                } catch {
                    return double.NaN;
                }
            }
        }

        public long LastSample {
            get {
                try {
                    return this.LastSamples.Max();
                } catch {
                    return 0;
                }
            }
        }

        public IEnumerable<long> LastSamples {
            get {
                foreach (IEnumerable<long> @enum in this._captures.Where(capture => capture.Valid).Select(capture => capture.LastSamples)) {
                    foreach (long @long in @enum) {
                        yield return @long;
                    }
                }
            }
        }

        public double LastAmplitudePercent {
            get {
                try {
                    return this.LastAmplitudePercents.Max();
                } catch {
                    return double.NaN;
                }
            }
        }

        public IEnumerable<double> LastAmplitudePercents {
            get {
                foreach(IEnumerable<double> @enum in this._captures.Where(capture => capture.Valid).Select(capture => capture.LastAmplitudePercents)) {
                    foreach(double @double in @enum) {
                        yield return @double;
                        //how I did not think to use iterators is beyond me.
                        //assuming they function like JS generators, this should VASTLY improve speed.
                        //instead of copying the arrays every time, we give an iterator function that will do that for us.
                        //it also means we don't have to zip anything.
                    }
                }
            }
        }

        public bool Valid => this._captures.Any(a => a.Valid);

        public void Start() {
            this._captures.ForEach(a => a.Start());
        }
        public void Stop() {
            this._captures.ForEach(a => a.Stop());
        }

        public AudioCapture(params IAudioCapture[] captures) {
            this._captures = captures.ToList();
        }
        public AudioCapture()
            : this(GetAudioCaptures()) {

        }
        public static IAudioCapture[] GetAudioCaptures() {
            List<IAudioCapture> captures = new List<IAudioCapture>() {
                //new OBSAudioCapture()
            };
            captures.AddRange(SoundAudioCapture.CaptureAllAudio());
            //add in OBS capture.
            captures.ForEach(capture => capture.Start());
            return captures.ToArray();
        }
        public static IAudioCapture[] GetOutputAudioCaptures() {
            List<IAudioCapture> captures = new List<IAudioCapture>() {
                //new OBSAudioCapture()
            };
            captures.AddRange(SoundAudioCapture.CaptureAllOutputAudio());
            //add in OBS capture.
            //captures.ForEach(capture => capture.Start());
            return captures.ToArray();
        }

        public static AudioCapture GetOutputCapture() {
            return new AudioCapture(GetOutputAudioCaptures());
        }

        public static AudioCapture GetOutputPlusSettingsCapture(ApplicationSettings.SettingsV0 settings) {
            List<IAudioCapture> clients = new List<IAudioCapture>();
            clients.AddRange(GetOutputAudioCaptures());
            foreach(ApplicationSettings.SettingsV0.AudioDeviceSettings dev in settings.Devices) {
                if (!string.IsNullOrWhiteSpace(dev.ObsName)) {
                    clients.Add(new OBSAudioCapture(dev.ObsName));
                }
            }
            return new AudioCapture(clients.ToArray());
        }

    }


}
