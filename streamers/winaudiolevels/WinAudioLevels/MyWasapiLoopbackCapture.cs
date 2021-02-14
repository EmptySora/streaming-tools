using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAudioLevels {

    /// <summary>
    /// WASAPI Loopback Capture
    /// based on a contribution from "Pygmy" - http://naudio.codeplex.com/discussions/203605
    /// </summary>
    public class MyWasapiLoopbackCapture : MyWasapiCapture {
        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        public MyWasapiLoopbackCapture() :
            this(GetDefaultLoopbackCaptureDevice()) {
        }

        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        /// <param name="captureDevice">Capture device to use</param>
        public MyWasapiLoopbackCapture(MMDevice captureDevice) :
            base(captureDevice) {
        }

        /// <summary>
        /// Gets the default audio loopback capture device
        /// </summary>
        /// <returns>The default audio loopback capture device</returns>
        public static MMDevice GetDefaultLoopbackCaptureDevice() {
            MMDeviceEnumerator devices = new MMDeviceEnumerator();
            return devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        /// <summary>
        /// Capturing wave format
        /// </summary>
        public override WaveFormat WaveFormat {
            get => base.WaveFormat;
            set => throw new InvalidOperationException("WaveFormat cannot be set for WASAPI Loopback Capture");
        }

        /// <summary>
        /// Specify loopback
        /// </summary>
        protected override AudioClientStreamFlags GetAudioClientStreamFlags() {
            return AudioClientStreamFlags.Loopback;
        }
    }
}
