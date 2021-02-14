using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinAudioLevels {

    /// <summary>
    /// Audio Capture using Wasapi
    /// See http://msdn.microsoft.com/en-us/library/dd370800%28VS.85%29.aspx
    /// </summary>
    public class MyWasapiCapture : IWaveIn {
        private const long REFTIMES_PER_SEC = 10000000;
        private const long REFTIMES_PER_MILLISEC = 10000;
        private const long WAIT_DIVISOR = 64; //... from 48 to 64, the packets/second jumps from 64 to 32768... with no gradient, lol.
        //128: 11520
        //64: 23040
        private volatile CaptureState _capture_state;
        private byte[] _record_buffer;
        private Thread _capture_thread;
        private AudioClient _audio_client;
        private int _bytes_per_frame;
        private WaveFormat _wave_format;
        private bool _initialized;
        private readonly SynchronizationContext _sync_context;
        private readonly bool _is_using_event_sync;
        private EventWaitHandle _frame_event_wait_handle;
        private readonly int _audio_buffer_milliseconds_length;

        /// <summary>
        /// Indicates recorded data is available 
        /// </summary>
        public event EventHandler<WaveInEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        public event EventHandler<StoppedEventArgs> RecordingStopped;

        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        public MyWasapiCapture() :
            this(GetDefaultCaptureDevice()) {
        }

        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        /// <param name="captureDevice">Capture device to use</param>
        public MyWasapiCapture(MMDevice captureDevice)
            : this(captureDevice, false) {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyWasapiCapture"/> class.
        /// </summary>
        /// <param name="captureDevice">The capture device.</param>
        /// <param name="useEventSync">true if sync is done with event. false use sleep.</param>
        public MyWasapiCapture(MMDevice captureDevice, bool useEventSync)
            : this(captureDevice, useEventSync, 100) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyWasapiCapture" /> class.
        /// </summary>
        /// <param name="captureDevice">The capture device.</param>
        /// <param name="useEventSync">true if sync is done with event. false use sleep.</param>
        /// <param name="audioBufferMillisecondsLength">Length of the audio buffer in milliseconds. A lower value means lower latency but increased CPU usage.</param>
        public MyWasapiCapture(MMDevice captureDevice, bool useEventSync, int audioBufferMillisecondsLength) {
            this._sync_context = SynchronizationContext.Current;
            this._audio_client = captureDevice.AudioClient;
            this.ShareMode = AudioClientShareMode.Shared;
            this._is_using_event_sync = useEventSync;
            this._audio_buffer_milliseconds_length = audioBufferMillisecondsLength;
            this._wave_format = this._audio_client.MixFormat;
        }

        /// <summary>
        /// Share Mode - set before calling StartRecording
        /// </summary>
        public AudioClientShareMode ShareMode { get; set; }

        /// <summary>
        /// Current Capturing State
        /// </summary>

        public CaptureState CaptureState => this._capture_state;


        /// <summary>
        /// Capturing wave format
        /// </summary>
        public virtual WaveFormat WaveFormat {
            get =>
                // for convenience, return a WAVEFORMATEX, instead of the real
                // WAVEFORMATEXTENSIBLE being used
                this._wave_format.AsStandardWaveFormat();

            set => this._wave_format = value;

        }

        /// <summary>
        /// Gets the default audio capture device
        /// </summary>
        /// <returns>The default audio capture device</returns>
        public static MMDevice GetDefaultCaptureDevice() {
            MMDeviceEnumerator devices = new MMDeviceEnumerator();
            return devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
        }

        private void InitializeCaptureDevice() {
            if (this._initialized) {
                return;
            }
            long requestedDuration = REFTIMES_PER_MILLISEC * this._audio_buffer_milliseconds_length;
            if (!this._audio_client.IsFormatSupported(this.ShareMode, this._wave_format)) {
                throw new ArgumentException("Unsupported Wave Format");
            }
            AudioClientStreamFlags streamFlags = this.GetAudioClientStreamFlags();
            // If using EventSync, setup is specific with shareMode
            if (this._is_using_event_sync) {
                // Init Shared or Exclusive
                if (this.ShareMode == AudioClientShareMode.Shared) {

                    // With EventCallBack and Shared, both latencies must be set to 0
                    this._audio_client.Initialize(this.ShareMode, AudioClientStreamFlags.EventCallback | streamFlags, requestedDuration, 0,
                        this._wave_format, Guid.Empty);
                } else {
                    // With EventCallBack and Exclusive, both latencies must equals
                    this._audio_client.Initialize(this.ShareMode, AudioClientStreamFlags.EventCallback | streamFlags, requestedDuration, requestedDuration,
                                        this._wave_format, Guid.Empty);
                }
                // Create the Wait Event Handle
                this._frame_event_wait_handle = new EventWaitHandle(false, EventResetMode.AutoReset);
                this._audio_client.SetEventHandle(this._frame_event_wait_handle.SafeWaitHandle.DangerousGetHandle());
            } else {
                // Normal setup for both sharedMode
                this._audio_client.Initialize(this.ShareMode,
                streamFlags,
                requestedDuration,
                0,
                this._wave_format,
                Guid.Empty);
            }
            int bufferFrameCount = this._audio_client.BufferSize;
            this._bytes_per_frame = this._wave_format.Channels * this._wave_format.BitsPerSample / 8;
            this._record_buffer = new byte[bufferFrameCount * this._bytes_per_frame];
            //Debug.WriteLine(string.Format("record buffer size = {0}", this.recordBuffer.Length));
            this._initialized = true;

        }

        /// <summary>
        /// To allow overrides to specify different flags (e.g. loopback)
        /// </summary>
        protected virtual AudioClientStreamFlags GetAudioClientStreamFlags() {
            return AudioClientStreamFlags.None;
        }

        /// <summary>
        /// Start Capturing
        /// </summary>
        public void StartRecording() {
            if (this._capture_state != CaptureState.Stopped) {
                throw new InvalidOperationException("Previous recording still in progress");
            }
            this._capture_state = CaptureState.Starting;
            this.InitializeCaptureDevice();
            this._capture_thread = new Thread(() => this.CaptureThread(this._audio_client));
            this._capture_thread.Start();
        }

        /// <summary>
        /// Stop Capturing (requests a stop, wait for RecordingStopped event to know it has finished)
        /// </summary>
        public void StopRecording() {
            if (this._capture_state != CaptureState.Stopped) {
                this._capture_state = CaptureState.Stopping;
            }
        }

        private void CaptureThread(AudioClient client) {
            Exception exception = null;
            try {
                this.DoRecording(client);
            } catch (Exception e) {
                exception = e;
            } finally {
                client.Stop();
                // don't dispose - the AudioClient only gets disposed when WasapiCapture is disposed
            }
            this._capture_thread = null;
            this._capture_state = CaptureState.Stopped;
            this.RaiseRecordingStopped(exception);
        }

        private void DoRecording(AudioClient client) {
            //Debug.WriteLine(String.Format("Client buffer frame count: {0}", client.BufferSize));
            int bufferFrameCount = client.BufferSize;
            // Calculate the actual duration of the allocated buffer.
            long actualDuration = (long)((double)REFTIMES_PER_SEC *
                             bufferFrameCount / this._wave_format.SampleRate);
            double sleepMilliseconds = ((double)actualDuration / REFTIMES_PER_MILLISEC / 2 / WAIT_DIVISOR);
            double waitMilliseconds = (3D * actualDuration / REFTIMES_PER_MILLISEC / WAIT_DIVISOR);

            AudioCaptureClient capture = client.AudioCaptureClient;
            client.Start();
            // avoid race condition where we stop immediately after starting

            if (this._capture_state == CaptureState.Starting) {
                this._capture_state = CaptureState.Capturing;
            }

            while (this._capture_state == CaptureState.Capturing) {
                if (this._is_using_event_sync) {
                    this._frame_event_wait_handle.WaitOne(TimeSpan.FromMilliseconds(waitMilliseconds), false);
                } else {
                    Thread.Sleep(TimeSpan.FromMilliseconds(sleepMilliseconds));
                }

                if (this._capture_state != CaptureState.Capturing) {
                    break;
                }
                // If still recording
                this.ReadNextPacket(capture);
            }
        }

        private void RaiseRecordingStopped(Exception e) {
            EventHandler<StoppedEventArgs> handler = RecordingStopped;
            if (handler == null) {
                return;
            }

            if (this._sync_context == null) {
                handler(this, new StoppedEventArgs(e));
            } else {
                this._sync_context.Post(state => handler(this, new StoppedEventArgs(e)), null);
            }
        }

        private void ReadNextPacket(AudioCaptureClient capture) {
            int packetSize = capture.GetNextPacketSize();
            int recordBufferOffset = 0;
            //Debug.WriteLine(string.Format("packet size: {0} samples", packetSize / 4));

            while (packetSize != 0) {
                IntPtr buffer = capture.GetBuffer(out int framesAvailable, out AudioClientBufferFlags flags);
                int bytesAvailable = framesAvailable * this._bytes_per_frame;
                // apparently it is sometimes possible to read more frames than we were expecting?
                // fix suggested by Michael Feld:
                int spaceRemaining = Math.Max(0, this._record_buffer.Length - recordBufferOffset);
                if (spaceRemaining < bytesAvailable && recordBufferOffset > 0) {
                    DataAvailable?.Invoke(this, new WaveInEventArgs(this._record_buffer, recordBufferOffset));
                    recordBufferOffset = 0;
                }
                // if not silence...
                if ((flags & AudioClientBufferFlags.Silent) != AudioClientBufferFlags.Silent) {
                    Marshal.Copy(buffer, this._record_buffer, recordBufferOffset, bytesAvailable);
                } else {
                    Array.Clear(this._record_buffer, recordBufferOffset, bytesAvailable);
                }
                recordBufferOffset += bytesAvailable;
                capture.ReleaseBuffer(framesAvailable);
                packetSize = capture.GetNextPacketSize();
            }
            DataAvailable?.Invoke(this, new WaveInEventArgs(this._record_buffer, recordBufferOffset));
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() {
            this.StopRecording();
            if (this._capture_thread != null) {
                this._capture_thread.Join();
                this._capture_thread = null;
            }
            if (this._audio_client != null) {
                this._audio_client.Dispose();
                this._audio_client = null;
            }
        }
    }
}
