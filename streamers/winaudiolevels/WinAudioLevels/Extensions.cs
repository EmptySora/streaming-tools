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
    }
}
