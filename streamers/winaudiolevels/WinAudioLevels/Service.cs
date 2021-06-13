using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;

namespace WinAudioLevels {
    static class Service {
        public static void Install() {
            if (IsInstalled) {
                return;
            }
            ManagedInstallerClass.InstallHelper(new string[]{

            });
        }
        public static void Uninstall() {

        }
        public static void Reinstall() {

        }
        public static bool IsInstalled => ServiceController.GetServices().Any(svc => svc.ServiceName == "WALTestService");
    }
    [RunInstaller(true)]
    public class MyServiceInstaller : Installer {
        public MyServiceInstaller() {
            this.Installers.AddRange(new Installer[] {
                new ServiceInstaller() {
                    DelayedAutoStart = false,
                    Description = "Test Service",
                    DisplayName = "Test Service",
                    ServiceName = "WALTestService",
                    StartType = ServiceStartMode.Manual
                },
                new ServiceProcessInstaller() {
                    Account = ServiceAccount.LocalService
                }
            });
        }
    }
    public class MyService : ServiceBase {
        protected override void OnContinue() {
            base.OnContinue();
        }
        protected override void OnPause() {
            base.OnPause();
        }
        protected override void OnShutdown() {
            base.OnShutdown();
        }
        protected override void OnStop() {
            base.OnStop();
        }
        protected override void OnStart(string[] args) {
            base.OnStart(args);
        }
        public static MyService Service => new MyService() {
            CanHandlePowerEvent = true,
            CanPauseAndContinue = true,
            CanShutdown = true,
            CanStop = true,
            CanHandleSessionChangeEvent = true,
            ServiceName = "WALTestService"
        };
    }
}
