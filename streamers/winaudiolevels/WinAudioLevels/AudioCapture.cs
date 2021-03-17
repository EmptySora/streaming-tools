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
                foreach (IEnumerable<long> levels in this._captures.Where(a => a.Valid).Select(a => a.LastAudioLevels)) {
                    foreach (long level in levels) {
                        yield return level;
                    }
                }
            }
        }
        public IEnumerable<long> LastSamples {
            get {
                foreach (IEnumerable<long> samples in this._captures.Where(a => a.Valid).Select(a => a.LastSamples)) {
                    foreach (long sample in samples) {
                        yield return sample;
                    }
                }
            }
        }
        public IEnumerable<double> LastAmplitudePercents {
            get {
                foreach (IEnumerable<double> amps in this._captures.Where(a => a.Valid).Select(a => a.LastAmplitudePercents)) {
                    foreach (double amp in amps) {
                        yield return amp;
                    }
                }
            }
        }

        public double LastAudioLevel => this.LastAudioLevels.MaxOrDefault(double.NaN);
        public long LastSample => this.LastSamples.MaxOrDefault(default);
        public double LastAmplitudePercent => this.LastAmplitudePercents.MaxOrDefault(double.NaN);


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
            return SoundAudioCapture.CaptureAllOutputAudio();
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
