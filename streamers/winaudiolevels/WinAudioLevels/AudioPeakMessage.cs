using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAudioLevels {
    class AudioPeakMessage {
        [JsonIgnore()]
        public AudioPeakMessageStatus Status = AudioPeakMessageStatus.Success;
        [JsonProperty(PropertyName = "status")]
        public string StatusTextual => Enum.GetName(typeof(AudioPeakMessageStatus), this.Status);
        [JsonProperty(PropertyName = "type")]
        [JsonIgnore]
        public string TypeTextual => Enum.GetName(typeof(AudioPeakMessageType), this.Type);
        [JsonIgnore()]
        public AudioPeakMessageType Type = AudioPeakMessageType.Peaks;
        [JsonProperty(PropertyName = "serverTime")]
        [JsonIgnore]
        public DateTime ServerTime = DateTime.UtcNow;
        [JsonProperty(PropertyName = "data")]
        public object Data; //Exception if Status

        public class AudioPeaks {
            [JsonProperty(PropertyName = "peaks")]
            public double[] Peaks = new double[0];
            [JsonProperty(PropertyName = "max")]
            public double Maximum => this.Peaks.Length == 0 ? 0 : this.Peaks.Max();
            [JsonProperty(PropertyName = "min")]
            public double Minimum => this.Peaks.Length == 0 ? 0 : this.Peaks.Min();
            [JsonProperty(PropertyName = "avg")]
            public double Average => this.Peaks.Length == 0 ? 0 : this.Peaks.Average();
        }

        public static AudioPeakMessage NewPing() {
            return new AudioPeakMessage() {
                Type = AudioPeakMessageType.Ping
            };
        }
        public static AudioPeakMessage NewPeaks(params double[] peaks) {
            return new AudioPeakMessage() {
                Data = new AudioPeaks() {
                    Peaks = peaks
                }
            };
        }
    }
}
