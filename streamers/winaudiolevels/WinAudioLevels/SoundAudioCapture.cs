using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinAudioLevels {
    class SoundAudioCapture : IAudioCapture {
        private MMDevice _device;
        private readonly string _device_id;
        private AudioDeviceProperties _properties;
        private MyWasapiCapture _capture;
        private WaveFormat _format;
        private Thread _thread;
        private IEnumerable<long> _last_samples = new long[0];
        private DataFlow _flow;
        private int _bytes_per_sample_set;
        private byte[] _sample_set_buffer;
        private readonly object _lock = new object();
        public SoundAudioCapture(string deviceId) {
            this._device_id = deviceId;
        }

        public long LastSample {
            get {
                try {
                    return this._last_samples.Count() > 1
                        ? this._last_samples.Max()
                        : this._last_samples.FirstOrDefault();
#warning race condition... samples can be updated after counting. Need to lock this...
                } catch { return 0; }
            }
        }
        public double LastAudioLevel => 20 * Math.Log10(Math.Abs(this.LastSample) / Math.Pow(2, 31));
        public double LastAmplitudePercent => Math.Abs(this.LastSample) / Math.Pow(2, 31);

        //largest sample out of every channel.
        public IEnumerable<long> LastSamples {
            get {
                lock (this._lock) {
                    return (long[])this._last_samples.ToArray().Clone(); //clone so the original array isn't touched
                }
            }
            private set {
                lock (this._lock) {
                    this._last_samples = value;
                }
            }
        }
        public IEnumerable<double> LastAudioLevels => this.LastSamples.Select(a => 20 * Math.Log10(Math.Abs(a) / Math.Pow(2, 31)));
        public IEnumerable<double> LastAmplitudePercents => this.LastSamples.Select(a => Math.Abs(a) / Math.Pow(2, 31));

        public bool Valid => this.LastSample != 0;

        //last sample in every channel.

        public void Start() {
            this._thread = new Thread(this.CaptureMain) { Name = "Audio Capture Thread" };
            this._thread.Start();
        }
        public void Stop() {
            this._thread.Abort();
            this._thread = null;
        }

        private void Initialize() {
            using (MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator()) {
                this._device = deviceEnumerator.GetDevice(this._device_id);
            }
            this._properties = this._device;
            this._flow = this._device.DataFlow;
            //ugh... threading issues suck
        }
        private void CaptureMain() {
            try {
                this.Initialize();
                this.CaptureAudio(this._flow == DataFlow.Render);
                //capture will be more difficult... PSYCH!
            } catch (ThreadAbortException) {
                //cleanup
                this._capture.Dispose();
                this._format = null;
            }
        }
        private bool CaptureAudio(bool loopback = true) {
            this._capture = loopback 
                ? new MyWasapiLoopbackCapture(this._device) 
                : new MyWasapiCapture(this._device);

            this._format = this._capture.WaveFormat;
            this._bytes_per_sample_set = (this._format.BitsPerSample / 8) * this._format.Channels;
            this._sample_set_buffer = new byte[this._bytes_per_sample_set];
            this._capture.DataAvailable += this.DataAvailable;
            this._capture.RecordingStopped += this.Capture_RecordingStopped;
            this._capture.StartRecording();
            this.OutputFormat();
            return true; //dummy so we can merge things.
        }

        private void Capture_RecordingStopped(object sender, StoppedEventArgs e) {
            throw e.Exception;
        }

        private void OutputFormat() {
            Console.WriteLine("WaveFormat:\n" +
                "    Capture Device Name:  {7}\n" +
                "    Avg bytes per second: {0}\n" +
                "    Bits per sample:      {1}\n" +
                "    Block align:          {2}\n" +
                "    Channels:             {3}\n" +
                "    Encoding:             {4}\n" +
                "    Extra size:           {5}\n" +
                "    Sample rate:          {6}",
                this._format.AverageBytesPerSecond,
                this._format.BitsPerSample,
                this._format.BlockAlign,
                this._format.Channels,
                Enum.GetName(typeof(WaveFormatEncoding), this._format.Encoding),
                this._format.ExtraSize,
                this._format.SampleRate,
                this._properties.FriendlyName ?? this._properties.DeviceFriendlyName ?? this._device.ID);
        }
        private void DataAvailable(object sender, WaveInEventArgs e) {
            //e.Buffer, e.BytesRecorded
            //use this._format to detect channel count and sample size.
            //check the encoding type to see how things are laid out.
            //get most recent sample.
            //we need to be able to rapidly fetch the latest sample, but there's no control on how often this event fires.
            //set this.LastSample and this.LastSamples.
            if (e.BytesRecorded == 0) {
                this.LastSamples = new long[0];
                //BitConverter.ToSingle will work.
            } else {
                Array.Copy(
                    e.Buffer,
                    e.BytesRecorded - this._bytes_per_sample_set,
                    this._sample_set_buffer,
                    0,
                    this._bytes_per_sample_set); //get last sample.
                this.LastSamples = GetSamples(this._sample_set_buffer, this._format.Encoding, this._format.BitsPerSample / 8);
            }
        }
        private static long[] GetSamples(byte[] buffer, WaveFormatEncoding encoding, int sampleSize) {
            WaveBuffer buff = new WaveBuffer((byte[])buffer.Clone());
            switch (encoding) {
            case WaveFormatEncoding.Pcm:
                //sampleSize-byte PCM samples (what we want.)
                switch (sampleSize) {
                case 1:
                    return buffer.Cast<long>().ToArray(); //easy.
                case 2:
                    return buff.ShortBuffer.Cast<long>().Take(buffer.Length / sampleSize).ToArray();
                case 4:
                    return buff.IntBuffer.Cast<long>().Take(buffer.Length / sampleSize).ToArray();
                case 3:
                default:
                    throw new NotImplementedException("Only 8-bit, 16-bit, and 32-bit audio streams are supported!");
                }
            case WaveFormatEncoding.IeeeFloat:
                double pow = Math.Pow(2, 31);
                return buff.FloatBuffer.Select(sample => {
                    try { 
                        return Math.Abs((long)(sample * pow)); 
                    } catch {
                        return long.MaxValue;
                    }
                }).Take(buffer.Length / sampleSize).ToArray();
            default:
                throw new NotImplementedException("Only uncompressed audio streams are supported!");
            }
        }

        public static SoundAudioCapture[] CaptureAllAudio() {
            using (MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator()) {
                return deviceEnumerator
                    .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
                    .Select(device => new SoundAudioCapture(device.ID))
                    .ToArray();
            }
        }
        public static SoundAudioCapture[] CaptureAllOutputAudio() {
            using (MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator()) {
                return deviceEnumerator
                    .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                    .Select(device => new SoundAudioCapture(device.ID))
                    .ToArray();
            }
        }
        /*
        public void CaptureAudio(MMDevice device) {
            //WasapiLoopbackCapture loopback = new WasapiLoopbackCapture(device);
            AudioClient client = device.AudioClient;
            client.Initialize(
                AudioClientShareMode.Shared,
                AudioClientStreamFlags.Loopback, //0 for eCapture
                TimeSpan.FromSeconds(1).Ticks, 
                0, 
                client.MixFormat,
                Guid.Empty);
            AudioCaptureClient capture = client.AudioCaptureClient;
            client.Start();
            while (true) {
                int packetSize = capture.GetNextPacketSize();
                while (packetSize > 0) {
                    IntPtr data = capture.GetBuffer(out int numFramesAvailable, out AudioClientBufferFlags flags);
                    if (flags.HasFlag(AudioClientBufferFlags.Silent)) {
                        data = IntPtr.Zero; //Tell CopyData to write silence
                    }
                    CopyData(data, numFramesAvailable);
                    capture.ReleaseBuffer(numFramesAvailable);
                    packetSize = capture.GetNextPacketSize();
                    client.MixFormat.
                }
            }
            
        }
        public void CopyData(IntPtr data, int framesAvailable) {

        }
        */

    }
}
