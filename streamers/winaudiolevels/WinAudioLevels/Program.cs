using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinAudioLevels {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arguments) {
            if(arguments.Any(a=>a.ToLower() == "--test")) {
                Testing();
            } else if(arguments.Any(a => a.ToLower() == "--browser")) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new CefBrowser());
            } else {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LoadingForm());
            }
            
        }

        static void Testing() {
            //ApplicationSettings.LoadOrDefault();
            /*
            Console.WriteLine("DOES FILE.EXIST CARE ABOUT RESOURCE PATHS?\n" +
                "    WinAudioLevels.exe... {0}\n" +
                "    WinAudioLevels.exe,-145... {1}",
                File.Exists("WinAudioLevels.exe") ? "EXISTS" : "DOES NOT EXIST",
                File.Exists("WinAudioLevels.exe,-145") ? "EXISTS" : "DOES NOT EXIST");
            Console.ReadLine();
            */
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
