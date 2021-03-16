using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAudioLevels {
    public enum ObsAudioMeterStatus {
        Success,
        WindowTooThin,
        NoMetersFound,
        TicksNotFound,
        CouldNotFindObsProcess,
        CouldNotFindAudioMixerWindow,
        FailedToCaptureAudioMixerWindow
    }
}
