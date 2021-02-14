using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAudioLevels {
    public class AudioDeviceProperties {
        #region Functionality crap...
        private readonly MMDevice _device;
        public AudioDeviceProperties(MMDevice device) {
            this._device = device;
        }

        public static implicit operator AudioDeviceProperties(MMDevice device) {
            return new AudioDeviceProperties(device);
        }
        public static implicit operator MMDevice(AudioDeviceProperties properties) {
            return properties._device;
        }
        #endregion
        //CATEGORY, DISPLAYNAME, DESCRIPTION, BROWSABLE
        #region General
        [Category("General")]
        [DisplayName("ID")]
        [Description("The ID of the audio device.")]
        public string ID => ErrorWrapping(() => this._device.ID);
        [Category("General")]
        [DisplayName("Instance ID")]
        [Description("The ID of the instance of the audio device.")]
        public string InstanceId => ErrorWrapping(() => this._device.InstanceId);
        [Category("General")]
        [DisplayName("Icon Path")]
        [Description("The file path leading to an icon representing the audio device.")]
        public string IconPath => ErrorWrapping(() => this._device.IconPath);
        [Category("General")]
        [DisplayName("Friendly Device Name")]
        [Description("A user-friendly rendering of the name of the audio device.")]
        public string DeviceFriendlyName => ErrorWrapping(() => this._device.DeviceFriendlyName);
        [Category("General")]
        [DisplayName("Friendly Name")]
        [Description("A user-friendly rendering of the name of the device. Not sure how this differs from \"DeviceFriendlyName\"...")]
        public string FriendlyName => ErrorWrapping(() => this._device.FriendlyName);
        [Category("General")]
        [DisplayName("State")]
        [Description("The state of the audio device. (One of \"Active\", \"Disabled\", \"Unplugged\", or \"Not present\")")]
        public DeviceState? State => ErrorWrapping(() => this._device?.State);
        [Category("General")]
        [DisplayName("Data Flow")]
        [Description("Describes the data flow of the audio device. (One of \"Capture\" or \"Render\")")]
        public DataFlow? DataFlow => ErrorWrapping(() => this._device?.DataFlow);
        [Category("General")]
        [DisplayName("Session Count")]
        [Description("The total number of sessions on the audio device.")]
        public int? AudioSessionCount => ErrorWrapping(() => this._device.AudioSessionManager.Sessions?.Count);
        #endregion
        #region AudioMeter
        [Category("Audio Meter Information")]
        [DisplayName("Hardware Support")]
        [Description("Describes what the audio meter hardware supports. (Flags: \"Meter\", \"Mute\" and/or \"Volume\")")]
        public EEndpointHardwareSupport? AudioMeterHardwareSupport => ErrorWrapping(() => this._device.AudioMeterInformation?.HardwareSupport);
        [Category("Audio Meter Information")]
        [DisplayName("Master Peak Value")]
        [Description("The value of the overall peak of the audio meter.")] //might be what we want...?
        public float? AudioMeterMasterPeak => ErrorWrapping(() => this._device.AudioMeterInformation?.MasterPeakValue);
        [Category("Audio Meter Information")]
        [DisplayName("Channel Peaks")]
        [Description("The individual peaks of each channel in the audio meter.")]
        public float[] AudioMeterChannelPeaks => ErrorWrapping(() => {
            AudioMeterInformationChannels peaks = this._device.AudioMeterInformation.PeakValues;
            List<float> floats = new List<float>();
            for (int i = 0; i < peaks.Count; i++) {
                floats.Add(peaks[i]);
            }
            return floats.ToArray();
        });
        #endregion
        #region AudioEndpointVolume
        [Category("Volume Information")]
        [DisplayName("Hardware Support")]
        [Description("Describes what the audio volume hardware supports. (Flags: \"Meter\", \"Mute\" and/or \"Volume\")")]
        public EEndpointHardwareSupport? AudioVolumeHardwareSupport => ErrorWrapping(() => this._device.AudioEndpointVolume?.HardwareSupport);
        [Category("Volume Information")]
        [DisplayName("Master Volume Level")]
        [Description("The master volume level of the audio device.")]
        public float? AudioVolumeMasterVolume => ErrorWrapping(() => this._device.AudioEndpointVolume?.MasterVolumeLevel);
        [Category("Volume Information")]
        [DisplayName("Master Volume Level (Scalar)")]
        [Description("The master volume level of the audio device as a scalar number.")]
        public float? AudioVolumeMasterVolumeScalar => ErrorWrapping(() => this._device.AudioEndpointVolume?.MasterVolumeLevelScalar);
        [Category("Volume Information")]
        [DisplayName("Muted?")]
        [Description("Determines whether or not the audio device has been muted.")]
        public bool? AudioVolumeMute => ErrorWrapping(() => this._device.AudioEndpointVolume?.Mute);
        [Category("Volume Information")]
        [DisplayName("Maximum Volume (dB)")]
        [Description("The maximum volume of the audio device in decibels.")]
        public float? AudioVolumeRangeMaxdB => ErrorWrapping(() => this._device.AudioEndpointVolume.VolumeRange?.MaxDecibels);
        [Category("Volume Information")]
        [DisplayName("Minimum Volume (dB)")]
        [Description("The minimum volume of the audio device in decibels.")]
        public float? AudioVolumeRangeMindB => ErrorWrapping(() => this._device.AudioEndpointVolume.VolumeRange?.MinDecibels);
        [Category("Volume Information")]
        [DisplayName("Increments (dB)")]
        [Description("The volume, in decibels, each increment or decrement of audio volume will change.")]
        public float? AudioVolumeRangeIncrementdB => ErrorWrapping(() => this._device.AudioEndpointVolume.VolumeRange?.IncrementDecibels);
        [Category("Volume Information")]
        [DisplayName("Current Volume Step")]
        [Description("The current step the audio volume level is at. (Think the ticks the volume buttons do on an iPhone)")]
        public uint? AudioVolumeStep => ErrorWrapping(() => this._device.AudioEndpointVolume.StepInformation?.Step);
        [Category("Volume Information")]
        [DisplayName("Volume Step Count")]
        [Description("The total number of steps the audio volume level is split into. (Think the ticks the volume buttons do on an iPhone)")]
        public uint? AudioVolumeStepCount => ErrorWrapping(() => this._device.AudioEndpointVolume.StepInformation?.StepCount);
        [Category("Volume Information")]
        [DisplayName("Channel Volume Levels")]
        [Description("The individual volume levels of all of the channels in the audio device.")]
        public float[] AudioVolumeChannelVolumes => ErrorWrapping(() => {
            AudioEndpointVolumeChannels peaks = this._device.AudioEndpointVolume.Channels;
            List<float> floats = new List<float>();
            for (int i = 0; i < peaks.Count; i++) {
                floats.Add(peaks[i].VolumeLevel);
            }
            return floats.ToArray();
        });
        [Category("Volume Information")]
        [DisplayName("Channel Volume Levels (Scalar)")]
        [Description("The individual volume levels of all of the channels in the audio device as a scalar number.")]
        public float[] AudioVolumeChannelVolumesScalar => ErrorWrapping(() => {
            AudioEndpointVolumeChannels peaks = this._device.AudioEndpointVolume.Channels;
            List<float> floats = new List<float>();
            for (int i = 0; i < peaks.Count; i++) {
                floats.Add(peaks[i].VolumeLevelScalar);
            }
            return floats.ToArray();
        });
        #endregion
        #region AudioControl
        [Category("Audio Control Information")]
        [DisplayName("State")]
        [Description("The state of the audio session controller. (One of \"Active\", \"Inactive\", or \"Expired\")")]
        public AudioSessionState? AudioControlState => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl?.State);
        [Category("Audio Control Information")]
        [DisplayName("Is System Session?")]
        [Description("Whether or not the audio session controller is a part of the system session.")]
        public bool? AudioControlIsSystem => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl?.IsSystemSoundsSession);
        [Category("Audio Control Information")]
        [DisplayName("Icon Path")]
        [Description("The file path leading to an icon representing the audio session controller.")]
        public string AudioControlIconPath => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl.IconPath);
        [Category("Audio Control Information")]
        [DisplayName("Instance ID")]
        [Description("The ID of the instance of the audio session controller.")]
        public string AudioControlSessionInstanceId => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl.GetSessionInstanceIdentifier);
        [Category("Audio Control Information")]
        [DisplayName("ID")]
        [Description("The ID of the audio session controller.")]
        public string AudioControlSessionId => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl.GetSessionIdentifier);
        [Category("Audio Control Information")]
        [DisplayName("Process ID")]
        [Description("The PID for the process that owns this audio session controller... I think...")]
        public uint? AudioControlProcessId => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl?.GetProcessID);
        [Category("Audio Control Information")]
        [DisplayName("Grouping Parameter")]
        [Description("The grouping parameter this audio session controller uses.")]
        public Guid? AudioControlGroupingParam => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl?.GetGroupingParam());
        [Category("Audio Control Information")]
        [DisplayName("Display Name")]
        [Description("The display name of the audio session controller.")]
        public string AudioControlDisplayName => ErrorWrapping(() => this._device.AudioSessionManager.AudioSessionControl.DisplayName);
        #endregion
        #region Audio Client Information
        [Category("Audio Client Information")]
        [DisplayName("Buffer Size")]
        [Description("The size of the audio buffer that the audio client uses in bytes.")]
        public int? AudioClientBufferSize => ErrorWrapping(() => this._device.AudioClient?.BufferSize);
        [Category("Audio Client Information")]
        [DisplayName("Current Padding")]
        [Description("The number of padding bytes that the audio client uses.")]
        public int? AudioClientPadding => ErrorWrapping(() => this._device.AudioClient?.CurrentPadding);
        [Category("Audio Client Information")]
        [DisplayName("Default Device Period")]
        [Description("The default device period of the audio client. I think this is in ticks or filetime.")]
        public long? AudioClientDefaultDevicePeriod => ErrorWrapping(() => this._device.AudioClient?.DefaultDevicePeriod);
        [Category("Audio Client Information")]
        [DisplayName("Minimum Device Period")]
        [Description("The minimum device period of the audio client. I think this is in ticks or filetime.")]
        public long? AudioClientMinimumDevicePeriod => ErrorWrapping(() => this._device.AudioClient?.MinimumDevicePeriod);
        [Category("Audio Client Information")]
        [DisplayName("Stream Latency")]
        [Description("The total latency of the audio client's stream. I think this is in ticks or filetime.")]
        public long? AudioClientStreamLatency => ErrorWrapping(() => this._device.AudioClient?.StreamLatency);
        [Category("Audio Client Information")]
        [DisplayName("Clock Frequency")]
        [Description("The frequency the audio client's clock ticks. Not sure what this is measured in.")]
        public ulong? AudioClientClockFrequency => ErrorWrapping(() => this._device.AudioClient.AudioClockClient?.Frequency);
        [Category("Audio Client Information")]
        [DisplayName("Clock Characteristics")]
        [Description("This one, I have no clue.")]
        public int? AudioClientClockCharacteristics => ErrorWrapping(() => this._device.AudioClient.AudioClockClient?.Characteristics);
        [Category("Audio Client Information")]
        [DisplayName("Clock position can be adjusted?")]
        [Description("Determines whether or not the audio client's clock's position can be adjusted.")]
        public bool? AudioClientClockCanAdjustPosition => ErrorWrapping(() => this._device.AudioClient.AudioClockClient?.CanAdjustPosition);
        [Category("Audio Client Information")]
        [DisplayName("Clock Adjusted Position")]
        [Description("The adjusted position of the audio client's clock.")]
        public ulong? AudioClientClockAdjustedPosition => ErrorWrapping(() => this._device.AudioClient.AudioClockClient?.AdjustedPosition);
        #endregion
        #region Audio Client Mix Format
        [Category("Mix Format")]
        [DisplayName("Average Data Rate")]
        [Description("The average data rate of the audio stream in bytes per second.")]
        public int? AudioClientMixFormatAvgBytesPerSecond => ErrorWrapping(() => this._device.AudioClient.MixFormat?.AverageBytesPerSecond);
        [Category("Mix Format")]
        [DisplayName("Sample Bit Rate")]
        [Description("The number of bits in each audio sample.")]
        public int? AudioClientMixFormatBitsPerSample => ErrorWrapping(() => this._device.AudioClient.MixFormat?.BitsPerSample);
        [Category("Mix Format")]
        [DisplayName("Block Alignment")]
        [Description("This, I'm not entirely sure.")]
        public int? AudioClientMixFormatBlockAlign => ErrorWrapping(() => this._device.AudioClient.MixFormat?.BlockAlign);
        [Category("Mix Format")]
        [DisplayName("Channel Count")]
        [Description("The total number of audio channels the output audio stream writes to.")]
        public int? AudioClientMixFormatChannelCount => ErrorWrapping(() => this._device.AudioClient.MixFormat?.Channels);
        [Category("Mix Format")]
        [DisplayName("Encoding")]
        [Description("The audio encoding of the audio stream.")]
        public WaveFormatEncoding? AudioClientMixFormatEncoding => ErrorWrapping(() => this._device.AudioClient.MixFormat?.Encoding);
        [Category("Mix Format")]
        [DisplayName("Extra Size")]
        [Description("No clue...")]
        public int? AudioClientMixFormatExtraSize => ErrorWrapping(() => this._device.AudioClient.MixFormat?.ExtraSize);
        [Category("Mix Format")]
        [DisplayName("Sample Rate")]
        [Description("The number of audio samples that are recorded every second.")]
        public int? AudioClientMixFormatSampleRate => ErrorWrapping(() => this._device.AudioClient.MixFormat?.SampleRate);
        #endregion

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
