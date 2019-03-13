using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Configuration.Install;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.IO.Pipes;
using System.Speech.Synthesis;
using System.ServiceProcess;
using System.ServiceModel;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security;

namespace TTSBot
{
    class Program
    {
        static SpeechSynthesizer _synth;
        static Random lol;
        static List<string> names;
        static void Speak(string text)
        {
                _synth.SelectVoice(names[lol.Next(names.Count)]);
                _synth.Rate = lol.Next(-5, 6);
                _synth.Speak(text);
        }
        static void TTSMan()
        {
            _synth = new SpeechSynthesizer();
            lol = new Random();
            names = new List<string>();
            names.AddRange(_synth.GetInstalledVoices().Select(voice => voice.VoiceInfo.Name));
            NamedPipeServerStream spipe = new NamedPipeServerStream("tts_comm_pipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.None, 4096, 4096);
            while (true)
            {
                spipe.WaitForConnection();
                byte[] buffer = new byte[4096];
                int zcount = 0;
                while (true)
                {
                    buffer = new byte[4096];

                    if (spipe.Read(buffer, 0, buffer.Length) == 0)
                    {
                        zcount++;
                        if (zcount > 10)
                            break;
                    }
                    else zcount = 0;
                    string data = Encoding.UTF8.GetString(buffer).Replace("\0", "");//remove null chars
                    Speak(data);
                    Thread.Sleep(1000);
                }
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var svcs = new ServiceBase[] { getService() };
                ServiceBase.Run(svcs);
            }
            else
            {
                switch (args[0].ToLower().Trim())
                {
                    case "-i": case "--install":
                        Install();
                        break;
                    case "-u": case "--uninstall":
                        Uninstall();
                        break;
                    case "-r": case "--reinstall":
                        Reinstall();
                        break;

                    case "-h": case "-?": case "--help":
                        Console.WriteLine("TTS Server\r\n");
                        Console.WriteLine("Usage:\r\n" +
                            "\t-i    --install      Installs the service.\r\n" +
                            "\t-u    --uninstall    Uninstalls the service.\r\n" +
                            "\t-r    --reinstall    Reinstalls the service.");
                        break;
                    case "--initiate-client-side-application-now":
                        TTSMan();
                        break;
                }
            }
        }
        static TTSService getService()
        {
            TTSService ret = new TTSService()
            {
                AutoLog = true,
                CanHandlePowerEvent = true,
                CanHandleSessionChangeEvent = true,
                CanPauseAndContinue = true,
                CanShutdown = true,
                CanStop = true,
                ServiceName = "TTS_Server",
            };
            return ret;
        }
        static void Install()
        {
            if (IsInstalled)
                return;
            ManagedInstallerClass.InstallHelper(new string[] { ExecutablePath + "TTSBot.exe" });
        }
        static void Uninstall()
        {
            if (IsInstalled)
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", ExecutablePath + "TTSBot.exe" });
            }
        }
        static void Reinstall()
        {
            if (IsInstalled)
                Uninstall();
            Install();
        }
        static bool IsInstalled
        {
            get { return ServiceController.GetServices().Any(svc => svc.ServiceName == "TTS_Server"); }
        }
        public static string ExecutablePath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "") + "\\"; }
        }
    }
    [RunInstaller(true)]
    public class TTSInstaller : Installer
    {
        public TTSInstaller()
        {
            
            Installers.Add(getServiceInstaller());
            Installers.Add(getProcInstaller());
        }
        ServiceInstaller getServiceInstaller()
        {
            ServiceInstaller ret = new ServiceInstaller()
            {
                DelayedAutoStart = true,
                Description = "Enables Applications to create TTS Alerts through the \\\\.\\pipe\\tts_pipe Pipe",
                DisplayName = "TTS Server",
                ServiceName = "TTS_Server",
                StartType = ServiceStartMode.Automatic,    
            };
            
            return ret;
        }
        ServiceProcessInstaller getProcInstaller()
        {
            ServiceProcessInstaller ret = new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalService
            };
            
            return ret;
        }
        
    }
    public class TTSService : ServiceBase
    {
        Thread tts;
        bool terminate;
        bool waiting = false;
        bool pause;
        public string DescribeVoice(InstalledVoice voice)
        {
            var x = voice.VoiceInfo;
            return string.Format("AGE: {0}\r\n", Enum.GetName(typeof(VoiceAge), x.Age)) +
                string.Format("CULTURE: {0}\r\n", x.Culture.ToString()) +
                string.Format("DESCRIPTION: {0}\r\n", x.Description) +
                string.Format("GENDER: {0}\r\n", Enum.GetName(typeof(VoiceGender), x.Gender)) +
                string.Format("ID: {0}\r\n", x.Id) +
                string.Format("NAME: {0}\r\n", x.Name);
        }
        void DoPause()
        {
            while (pause)
                Thread.Sleep(1000);
        }
        NamedPipeClientStream cpipe;
        Process subprocess = null;
        public void TTSMain()
        {
            EventLog.WriteEntry("Beginning to start the TTS server", EventLogEntryType.Information,0);
            NamedPipeServerStream spipe = null;
            spawnSubprocess();
            while (true)
            {
                spipe = new NamedPipeServerStream("tts_pipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.None, 4096, 4096);
                EventLog.WriteEntry("Waiting for a client to connect to the pipe", EventLogEntryType.Information, 0);
                if (terminate)
                    break;
                DoPause();
                waiting = true;
                spipe.WaitForConnection();
                waiting = false;
                EventLog.WriteEntry("A TTS client has connected to the pipe", EventLogEntryType.Information, 0);
                byte[] buffer = new byte[4096];
                int zcount = 0;
                while (true)
                {
                    if (terminate)
                        break;
                    DoPause();
                    buffer = new byte[4096];

                    if (spipe.Read(buffer, 0, buffer.Length) == 0)
                    {
                        zcount++;
                        if (zcount > 10)
                            break;
                    }
                    else zcount = 0;
                    string data = Encoding.UTF8.GetString(buffer).Replace("\0", "");//remove null chars
                    if (data.Length > 0)
                    {
                        EventLog.WriteEntry("Speaking: " + data, EventLogEntryType.Information, 0);
                        Speak(buffer);
                        Thread.Sleep(1000);
                    }
                }
            }

        }
        void spawnSubprocess()
        {
            SecureString pstr = new SecureString();
            pstr.AppendChar('f');
            pstr.AppendChar('?');
            pstr.AppendChar('a');
            pstr.AppendChar('T');
            pstr.AppendChar('?');
            pstr.AppendChar('5');
            pstr.AppendChar('8');
            pstr.AppendChar('U');
            subprocess = Process.Start(new ProcessStartInfo()
            {
                Arguments = "--initiate-client-side-application-now",
                FileName = Program.ExecutablePath + "TTSBot.exe",
                //Password = pstr,
                //UserName = "Brandon"
            });
            while (cpipe == null)
            {
                try { cpipe = new NamedPipeClientStream(".", "tts_comm_pipe", PipeDirection.InOut, PipeOptions.None); }
                catch { cpipe = null; }
            }
            cpipe.Connect();
        }
        void Speak(byte[] text)
        {
            if (Enabled)
            {
                
                if(subprocess.HasExited)
                    spawnSubprocess();
                cpipe.Write(text, 0, text.Length);
            }
        }
        public bool Enabled { get; set; }


        protected override void OnContinue()
        {
            pause = false;
        }
        protected override void OnPause()
        {
            pause = true;
        }
        public TTSService()
        {
            Enabled = true;
            cpipe = null;
             tts = null;
            terminate = false;
            pause = false;
        }

        protected override void OnStart(string[] args)
        {
            if (subprocess != null)
                subprocess.Kill();
            Thread tts_thread = new Thread(new ThreadStart(TTSMain)) { Name = "TTS Listener Thread" };
            tts = tts_thread;
            tts_thread.Start();
        }
        public string ExcepInfo(Exception err)
        {
            return string.Format("Help Link: {0}\r\n" +
                "HResult: {1}\r\n" +
                "Message: {2}\r\n" +
                "Source: {3}\r\n" +
                "Stack Trace: {4}\r\n" +
                "Target Site: {5}", err.HelpLink, err.HResult, err.Message, err.Source, err.StackTrace, err.TargetSite.Name);
        }
        protected override void OnStop()
        {
            if (tts != null)
            {
                if (waiting)
                {
                    tts.Abort();
                }
                terminate = true;
                tts.While(thread => thread.ThreadState != System.Threading.ThreadState.Stopped);
                tts = null;
                if (subprocess != null)
                    subprocess.Kill();
            }
        }
        protected override void OnShutdown()
        {
            if (tts != null)
            {
                if (waiting)
                {
                    tts.Abort();
                }
                terminate = true;
                tts.While(thread => thread.ThreadState != System.Threading.ThreadState.Stopped);
                tts = null;
                if (subprocess != null)
                    subprocess.Kill();
            }
        }
    }
    public static class Extensions
    {
        /// <summary>
        /// Pauses execution on the current thread until a specified condition is met.
        /// </summary>
        /// <typeparam name="T">The type of object being watched</typeparam>
        /// <param name="src">The object to watch</param>
        /// <param name="predicate">A function that should return true if we are to continue waiting, or false when the condition is met.</param>
        public static void While<T>(this T src, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new NullReferenceException();
            while (predicate.Invoke(src))
                Thread.Sleep(1000);
        }
    }
}
