using System.Drawing;

namespace WinAudioLevels {
    public class ObsAudioMixerMeter {
        public const int MAX_PIXELS_BETWEEN_CHANNELS = 10;
        public string ThemeName => this.Theme.name;
        public ObsTheme Theme { get; set; }
        public string MeterName { get; set; }
        public string MeterId { get; set; }
        public Rectangle MeterChannelTop { get; set; }
        public Rectangle MeterChannelBottom { get; set; }
        public bool HasDualMeter => this.MeterChannelBottom != default;
    }
}
