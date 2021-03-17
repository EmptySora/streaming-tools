using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace WinAudioLevels {
    public static partial class Extensions {
        public static IEnumerable<Process> GetChildProcesses(this Process process) {
            using (ManagementObjectSearcher mos = new ManagementObjectSearcher(
                string.Format("SELECT * FROM Win32_Process WHERE ParentProcessID={0}", process.Id))) {
                foreach (ManagementObject obj in mos.Get()) {
                    yield return Process.GetProcessById(Convert.ToInt32(obj["ProcessID"]));
                }
            }
        }
        #region MaxOrDefault
        public static decimal MaxOrDefault(this IEnumerable<decimal> source, decimal @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }
        public static decimal? MaxOrDefault(this IEnumerable<decimal?> source, decimal? @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }

        public static float MaxOrDefault(this IEnumerable<float> source, float @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }
        public static float? MaxOrDefault(this IEnumerable<float?> source, float? @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }

        public static double MaxOrDefault(this IEnumerable<double> source, double @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }
        public static double? MaxOrDefault(this IEnumerable<double?> source, double? @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }

        public static int MaxOrDefault(this IEnumerable<int> source, int @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }
        public static int? MaxOrDefault(this IEnumerable<int?> source, int? @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }

        public static long MaxOrDefault(this IEnumerable<long> source, long @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }
        public static long? MaxOrDefault(this IEnumerable<long?> source, long? @default) {
            try {
                return source.Max();
            } catch {
                return @default;
            }
        }
        #endregion

        #region MinOrDefault
        public static decimal MinOrDefault(this IEnumerable<decimal> source, decimal @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }
        public static decimal? MinOrDefault(this IEnumerable<decimal?> source, decimal? @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }

        public static float MinOrDefault(this IEnumerable<float> source, float @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }
        public static float? MinOrDefault(this IEnumerable<float?> source, float? @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }

        public static double MinOrDefault(this IEnumerable<double> source, double @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }
        public static double? MinOrDefault(this IEnumerable<double?> source, double? @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }

        public static int MinOrDefault(this IEnumerable<int> source, int @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }
        public static int? MinOrDefault(this IEnumerable<int?> source, int? @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }

        public static long MinOrDefault(this IEnumerable<long> source, long @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }
        public static long? MinOrDefault(this IEnumerable<long?> source, long? @default) {
            try {
                return source.Min();
            } catch {
                return @default;
            }
        }
        #endregion
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach(T item in source) {
                action?.Invoke(item);
            }
        }
    }
}
