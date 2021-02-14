using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAudioLevels {
    interface IAudioCapture {
        long LastSample { get; }
        IEnumerable<long> LastSamples { get; }
        double LastAudioLevel { get; }
        IEnumerable<double> LastAudioLevels { get; }
        double LastAmplitudePercent { get; }
        IEnumerable<double> LastAmplitudePercents { get; }
        bool Valid { get; }
        void Start();
        void Stop();
    }
}
