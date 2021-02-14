using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAudioLevels {
    public class AudioControlProperties {
        #region Functionality crap...
        private readonly AudioSessionControl _device;
        public AudioControlProperties(AudioSessionControl device) {
            this._device = device;
        }

        public static implicit operator AudioControlProperties(AudioSessionControl device) {
            return new AudioControlProperties(device);
        }
        #endregion
        #region AudioControl
        [Category("Audio Control Information")]
        [DisplayName("State")]
        [Description("The state of the audio session controller. (One of \"Active\", \"Inactive\", or \"Expired\")")]
        public AudioSessionState? AudioControlState => ErrorWrapping(() => this._device?.State);
        [Category("Audio Control Information")]
        [DisplayName("Is System Session?")]
        [Description("Whether or not the audio session controller is a part of the system session.")]
        public bool? AudioControlIsSystem => ErrorWrapping(() => this._device?.IsSystemSoundsSession);
        [Category("Audio Control Information")]
        [DisplayName("Icon Path")]
        [Description("The file path leading to an icon representing the audio session controller.")]
        public string AudioControlIconPath => ErrorWrapping(() => this._device.IconPath);
        [Category("Audio Control Information")]
        [DisplayName("Instance ID")]
        [Description("The ID of the instance of the audio session controller.")]
        public string AudioControlSessionInstanceId => ErrorWrapping(() => this._device.GetSessionInstanceIdentifier);
        [Category("Audio Control Information")]
        [DisplayName("ID")]
        [Description("The ID of the audio session controller.")]
        public string AudioControlSessionId => ErrorWrapping(() => this._device.GetSessionIdentifier);
        [Category("Audio Control Information")]
        [DisplayName("Process ID")]
        [Description("The PID for the process that owns this audio session controller... I think...")]
        public uint? AudioControlProcessId => ErrorWrapping(() => this._device?.GetProcessID);
        [Category("Audio Control Information")]
        [DisplayName("Grouping Parameter")]
        [Description("The grouping parameter this audio session controller uses.")]
        public Guid? AudioControlGroupingParam => ErrorWrapping(() => this._device?.GetGroupingParam());
        [Category("Audio Control Information")]
        [DisplayName("Display Name")]
        [Description("The display name of the audio session controller.")]
        public string AudioControlDisplayName => ErrorWrapping(() => this._device.DisplayName);
        #endregion
        //copy the "AudioControl*" properties from the below class.
        private static T ErrorWrapping<T>(Func<T> func)
            where T : class {
            try {
                return func.Invoke();
            } catch {
                return null;
            }
        }
        private static T? ErrorWrapping<T>(Func<T?> func)
            where T : struct {
            try {
                return func.Invoke();
            } catch {
                return null;
            }
        }
    }
}
