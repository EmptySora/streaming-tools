using Microsoft.Win32.SafeHandles;using System;using System.Collections.Generic;using System.ComponentModel;using System.Configuration;using System.Configuration.Install;using System.Diagnostics;using System.IO;using System.IO.Pipes;using System.Linq;using System.Reflection;using System.Net;using System.Runtime.InteropServices;using System.Security;
using System.ServiceModel;using System.ServiceProcess;using System.Text;using System.Threading;using System.Threading.Tasks;using System.Net.Sockets;

namespace BotStorage
{
    class Program
    {
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
                    case "-i":
                    case "--install":
                        Install();
                        break;
                    case "-u":
                    case "--uninstall":
                        Uninstall();
                        break;
                    case "-r":
                    case "--reinstall":
                        Reinstall();
                        break;
                    case "-h":
                    case "-?":
                    case "--help":
                        Console.WriteLine("TTS Server\r\n");
                        Console.WriteLine("Usage:\r\n" +
                            "\t-i    --install      Installs the service.\r\n" +
                            "\t-u    --uninstall    Uninstalls the service.\r\n" +
                            "\t-r    --reinstall    Reinstalls the service.");
                        break;
                    case "--run":
                        Console.WriteLine("starting server...");
                        var svc = new BotStorageService();
                        svc.server.Start();
                        while (true)
                            Thread.Sleep(1000);
                        break;
                }
            }
        }        //NAME: BotStorageService
        static BotStorageService getService()
        {
            BotStorageService ret = new BotStorageService()
            {
                AutoLog = true,
                CanHandlePowerEvent = false,
                CanHandleSessionChangeEvent = false,
                CanPauseAndContinue = false,
                CanShutdown = true,
                CanStop = true,
                ServiceName = "BotStorageService",
            };
            return ret;
        }
        static void Install()
        {
            if (IsInstalled)
                return;
            ManagedInstallerClass.InstallHelper(new string[] { ExecutablePath + "BotStorage.exe" });
        }
        static void Uninstall()
        {
            if (IsInstalled)
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", ExecutablePath + "BotStorage.exe" });
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
            get { return ServiceController.GetServices().Any(svc => svc.ServiceName == "BotStorageService"); }
        }
        public static string ExecutablePath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "") + "\\"; }
        }
    }
    [RunInstaller(true)]
    public class BotStorageServiceInstaller : Installer
    {
        public BotStorageServiceInstaller()
        {
            Installers.Add(getServiceInstaller());
            Installers.Add(getProcInstaller());
        }
        ServiceInstaller getServiceInstaller()
        {
            ServiceInstaller ret = new ServiceInstaller()
            {
                DelayedAutoStart = true,
                Description = "Allows synchronization of the persistent data across the many services that utilitize a shared storage.",
                DisplayName = "BotStorageService",
                ServiceName = "BotStorageService",
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
    public class BotStorageService : ServiceBase
    {
        internal RESTServer server;
        BotStorageObject obj;
        static BotStorageService _instance = null;
        object lockObject = new object();
        string filepath = @"C:\Users\Brandon\Documents\SoraBot\persistentStorage.dat";
        Random rand = new Random();
        public BotStorageService()
        {
            _instance = this;
            server = DefaultRESTServer();
            obj = BotStorageObject.FromFile(filepath);
        }
        protected override void OnStart(string[] args)
        {
            server.Start();
        }
        protected override void OnStop()
        {
            server.Stop();
        }
        protected override void OnShutdown()
        {
            server.Stop();
        }
        public static RESTServer DefaultRESTServer()
        {
            RESTServer ret = new RESTServer();
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_Version,
                Method = "GET",
                Path = "/Version",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #region REST_GET_CustomCommand
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_CustomCommand,
                Method = "GET",
                Path = "/CustomCommand/{commandname}",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_CustomCommands
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_CustomCommands,
                Method = "GET",
                Path = "/CustomCommands",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_DELETE_CustomCommand
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_DELETE_CustomCommand,
                Method = "DELETE",
                Path = "/CustomCommand/{commandname}",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_PUT_CustomCommand
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_CustomCommand,
                Method = "PUT",
                Path = "/CustomCommand/{commandname}",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_Quotes
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Quotes,
                Method = "GET",
                Path = "/Quotes",
                QueryParameters = new RESTQueryParameter[] { }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_GET_Quotes_Rec
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Quotes_Rec,
                Method = "GET",
                Path = "/Quotes/Recommend",
                QueryParameters = new RESTQueryParameter[] { }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_PUT_Quote
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_Quote,
                Method = "PUT",
                Path = "/Quote",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_PUT_Quote2
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_Quote2,
                Method = "PUT",
                Path = "/Quote",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "id", Required = true } }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_GET_Quote
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Quote,
                Method = "GET",
                Path = "/Quote",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "id", Required = true } }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_GET_Quote2
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Quote2,
                Method = "GET",
                Path = "/Quote",
                QueryParameters = new RESTQueryParameter[] { }
                //eg: /Quote
            });
            #endregion
            #region REST_DELETE_Quote
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_DELETE_Quote,
                Method = "DELETE",
                Path = "/Quote",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "id", Required = true } }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_PUT_Quote_Rec
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_Quote_Rec,
                Method = "PUT",
                Path = "/Quote/Recommend",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_PUT_Quote2_Rec
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_Quote2_Rec,
                Method = "PUT",
                Path = "/Quote/Recommend",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "id", Required = true } }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_GET_Quote_Rec
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Quote_Rec,
                Method = "GET",
                Path = "/Quote/Recommend",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "id", Required = true } }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_GET_Quote2_Rec
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Quote2_Rec,
                Method = "GET",
                Path = "/Quote/Recommend",
                QueryParameters = new RESTQueryParameter[] { }
                //eg: /Quote
            });
            #endregion
            #region REST_DELETE_Quote_Rec
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_DELETE_Quote_Rec,
                Method = "DELETE",
                Path = "/Quote/Recommend",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "id", Required = true } }
                //eg: /Quote?id=[id]
            });
            #endregion
            #region REST_GET_Blanks_User
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Blanks_User,
                Method = "GET",
                Path = "/Blanks/{username}",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_Blanks
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Blanks,
                Method = "GET",
                Path = "/Blanks",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_Purchase
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_Purchase,
                Method = "GET",
                Path = "/Purchase/{username}",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "cost", Required = true } }
                //eg: /Purchase/emptysora_?cost=[cost]
            });
            #endregion
            #region REST_PUT_Blanks
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_Blanks,
                Method = "PUT",
                Path = "/Blanks",
                QueryParameters = new RESTQueryParameter[] { }
                //used to add sorablanks to users for watching stream
            });
            #endregion
            #region REST_PUT_Blanks_User
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_Blanks_User,
                Method = "PUT",
                Path = "/Blanks/{username}",
                QueryParameters = new RESTQueryParameter[] { new RESTQueryParameter() { Name = "blanks", Required = true } }
                //manual set blanks
                // /Blanks/emptysora_?blanks=[new blanks]
            });
            #endregion
            #region REST_GET_TTS_Enabled
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_TTS_Enabled,
                Method = "GET",
                Path = "/TTS/Enabled",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_PUT_TTS_Disable
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_TTS_Disable,
                Method = "GET",
                Path = "/TTS/Enable",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_PUT_TTS_Enable
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_TTS_Enable,
                Method = "GET",
                Path = "/TTS/Disable",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_TTS_Banned
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_TTS_Banned,
                Method = "GET",
                Path = "/TTS/Banned",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_TTS_Banned_User
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_TTS_Banned_User,
                Method = "GET",
                Path = "/TTS/Banned/{username}",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_TTS_Ban_User
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_TTS_Ban_User,
                Method = "GET",
                Path = "/TTS/Ban/{username}",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_PUT_URLBanBreakers_User
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_URLBanBreakers_User,
                Method = "PUT",
                Path = "/URLBanBreakers/{username}",
                QueryParameters = new RESTQueryParameter[] { }
                //body is the new value to set the user at
                //"urlban_breakers"
            });
            #endregion
            #region REST_GET_URLBanBreakers_User
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_URLBanBreakers_User,
                Method = "GET",
                Path = "/URLBanBreakers/{username}",
                QueryParameters = new RESTQueryParameter[] { }
                //body is the new value to set the user at
                //"urlban_breakers"
            });
            #endregion
            #region REST_GET_URLBanBreakers_Broken
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_URLBanBreakers_Broken,
                Method = "GET",
                Path = "/URLBanBreakers/Broken/{username}",
                QueryParameters = new RESTQueryParameter[] { }
                //body is the new value to set the user at
                //"urlban_breakers"
            });
            #endregion
            #region REST_GET_TTS_Unban_User
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_TTS_Unban_User,
                Method = "GET",
                Path = "/TTS/Unban/{username}",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_GET_LastFollow
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_GET_LastFollow,
                Method = "GET",
                Path = "/LastFollow",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            #region REST_PUT_LastFollow
            ret.Register(new RESTMethodDefinition()
            {
                handler = REST_PUT_LastFollow,
                Method = "PUT",
                Path = "/LastFollow",
                QueryParameters = new RESTQueryParameter[] { }
            });
            #endregion
            return ret;
        }
        public static BotStorageService CurrentInstance
        {
            get { return _instance; }
        }
        const string STATUS_SUCCESS = "Success";
        const string STATUS_FAILURE = "Failure";

        private static RESTStatus REST_GET_URLBanBreakers_Broken(RESTQuery query, RESTMethodDefinition self)
        {
            string user = query.URLParameters["username"];
            lock (CurrentInstance.lockObject)
            {
                if (!CurrentInstance.obj.urlban_breakers.ContainsKey(user))
                    CurrentInstance.obj.urlban_breakers[user] = 0;
                CurrentInstance.obj.urlban_breakers[user]++;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                return new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed", Data = CurrentInstance.obj.urlban_breakers[user] };
            }
        }
        private static RESTStatus REST_GET_URLBanBreakers_User(RESTQuery query, RESTMethodDefinition self)
        {
            string user = query.URLParameters["username"];
            lock (CurrentInstance.lockObject)
            {
                if (!CurrentInstance.obj.urlban_breakers.ContainsKey(user))
                    CurrentInstance.obj.urlban_breakers[user] = 0;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                return new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed", Data = CurrentInstance.obj.urlban_breakers[user] };
            }
        }
        private static RESTStatus REST_PUT_URLBanBreakers_User(RESTQuery query, RESTMethodDefinition self)
        {
            string user = query.URLParameters["username"];
            lock (CurrentInstance.lockObject)
            {
                CurrentInstance.obj.urlban_breakers[user] = ulong.Parse(query.Body);
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                return new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed", Data = CurrentInstance.obj.urlban_breakers[user] };
            }
        }
        private static RESTStatus REST_PUT_Blanks_User(RESTQuery query, RESTMethodDefinition self)
        {
            string user = query.URLParameters["username"];
            ulong blanks = ulong.Parse(query.QueryParameters["blanks"]);
            lock (CurrentInstance.lockObject)
            {
                CurrentInstance.obj.Blanks[user] = blanks;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                return new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed", Data = CurrentInstance.obj.Blanks[user] };
            }
        }

        private static RESTStatus REST_Version(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = "Success", StatusMessage = "Successfully Completed", Data = new Version("1.0.0.0") };
            return ret;
        }

        private static RESTStatus REST_GET_CustomCommands(RESTQuery query, RESTMethodDefinition self)
        {
            lock (CurrentInstance.lockObject)
            {
                return new RESTStatus()
                {
                    Status = STATUS_SUCCESS,
                    StatusMessage = "Successfully Completed",
                    Data = CurrentInstance.obj.custom_commands
                };
            }
        }
        private static RESTStatus REST_GET_CustomCommand(RESTQuery query, RESTMethodDefinition self)
        {
            string cmd = query.URLParameters["commandname"];
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                if (CurrentInstance.obj.custom_commands.ContainsKey(cmd))
                {
                    ret.Status = STATUS_SUCCESS;
                    ret.StatusMessage = "Successfully Completed";
                    ret.Data = CurrentInstance.obj.custom_commands[cmd];
                }
                else
                {
                    ret.Status = STATUS_FAILURE;
                    ret.StatusMessage = "No such command called \"" + cmd + "\"!";
                    ret.Data = null;
                }
            }
            return ret;
        }
        private static RESTStatus REST_DELETE_CustomCommand(RESTQuery query, RESTMethodDefinition self)
        {
            string cmd = query.URLParameters["commandname"];
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                if (CurrentInstance.obj.custom_commands.ContainsKey(cmd))
                {
                    ret.Status = STATUS_SUCCESS;
                    ret.StatusMessage = "Successfully Completed";
                    ret.Data = CurrentInstance.obj.custom_commands[cmd];
                    CurrentInstance.obj.custom_commands.Remove(cmd);
                    CurrentInstance.obj.Save(CurrentInstance.filepath);
                }
                else
                {
                    ret.Status = STATUS_FAILURE;
                    ret.StatusMessage = "No such command called \"" + cmd + "\"!";
                    ret.Data = null;
                }
            }
            return ret;
        }
        private static RESTStatus REST_PUT_CustomCommand(RESTQuery query, RESTMethodDefinition self)
        {
            string cmd = query.URLParameters["commandname"];
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                bool nret = CurrentInstance.obj.custom_commands.ContainsKey(cmd);
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = (nret) ? "Edit" : "New";
                CurrentInstance.obj.custom_commands[cmd] = query.Body;
                ret.Data = CurrentInstance.obj.custom_commands[cmd];
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }

        private static RESTStatus REST_GET_Quotes(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                ret.Data = CurrentInstance.obj.quotes;
            }
            return ret;
        }
        private static RESTStatus REST_GET_Quotes_Rec(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                ret.Data = CurrentInstance.obj.quotes_rec;
            }
            return ret;
        }
        private static RESTStatus REST_GET_Quote(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                ret.Data = new BotQuote()
                {
                    quote = CurrentInstance.obj.quotes[int.Parse(query.QueryParameters["id"])],
                    number = ulong.Parse(query.QueryParameters["id"])
                };
            }
            return ret;
        }
        private static RESTStatus REST_GET_Quote2(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                int val = CurrentInstance.rand.Next(0, CurrentInstance.obj.quotes.Length);
                ret.Data = new BotQuote()
                {
                    quote = CurrentInstance.obj.quotes[val],
                    number = (ulong)val
                };
            }
            return ret;
        }
        private static RESTStatus REST_DELETE_Quote(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                ret.Data = new BotQuote()
                {
                    quote = CurrentInstance.obj.quotes[int.Parse(query.QueryParameters["id"])],
                    number = ulong.Parse(query.QueryParameters["id"])
                };
                var x = CurrentInstance.obj.quotes.ToList();
                x.RemoveAt(int.Parse(query.QueryParameters["id"]));
                CurrentInstance.obj.quotes = x.ToArray();
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }
        private static RESTStatus REST_PUT_Quote2(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                CurrentInstance.obj.quotes[int.Parse(query.QueryParameters["id"])] = query.Body;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                ret.Data = new BotQuote() { number = ulong.Parse(query.QueryParameters["id"]), quote = query.Body };
            }
            return ret;
        }
        private static RESTStatus REST_PUT_Quote(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                var x = CurrentInstance.obj.quotes.ToList();
                x.Add(query.Body);
                ulong retx = (ulong)(x.Count - 1);
                CurrentInstance.obj.quotes = x.ToArray();
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                ret.Data = new BotQuote() { number = retx, quote = query.Body };
            }
            return ret;
        }

        private static RESTStatus REST_GET_Quote_Rec(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                ret.Data = new BotQuote()
                {
                    quote = CurrentInstance.obj.quotes_rec[int.Parse(query.QueryParameters["id"])],
                    number = ulong.Parse(query.QueryParameters["id"])
                };
            }
            return ret;
        }
        private static RESTStatus REST_GET_Quote2_Rec(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                int val = CurrentInstance.rand.Next(0, CurrentInstance.obj.quotes_rec.Length);
                ret.Data = new BotQuote()
                {
                    quote = CurrentInstance.obj.quotes_rec[val],
                    number = (ulong)val
                };
            }
            return ret;
        }
        private static RESTStatus REST_DELETE_Quote_Rec(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();//need to add error handling
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                ret.Data = new BotQuote()
                {
                    quote = CurrentInstance.obj.quotes_rec[int.Parse(query.QueryParameters["id"])],
                    number = ulong.Parse(query.QueryParameters["id"])
                };
                var x = CurrentInstance.obj.quotes_rec.ToList();
                x.RemoveAt(int.Parse(query.QueryParameters["id"]));
                CurrentInstance.obj.quotes_rec = x.ToArray();
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }
        private static RESTStatus REST_PUT_Quote2_Rec(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                CurrentInstance.obj.quotes_rec[int.Parse(query.QueryParameters["id"])] = query.Body;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                ret.Data = new BotQuote() { number = ulong.Parse(query.QueryParameters["id"]), quote = query.Body };
            }
            return ret;
        }
        private static RESTStatus REST_PUT_Quote_Rec(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully Completed";
                var x = CurrentInstance.obj.quotes_rec.ToList();
                x.Add(query.Body);
                ulong retx = (ulong)(x.Count - 1);
                CurrentInstance.obj.quotes_rec = x.ToArray();
                CurrentInstance.obj.Save(CurrentInstance.filepath);
                ret.Data = new BotQuote() { number = retx, quote = query.Body };
            }
            return ret;
        }

        private static RESTStatus REST_GET_Blanks(RESTQuery query, RESTMethodDefinition self)
        {
            lock (CurrentInstance.lockObject)
            {
                return new RESTStatus()
                {
                    Status = STATUS_SUCCESS,
                    StatusMessage = "Successfully completed",
                    Data = CurrentInstance.obj.Blanks
                };
            }
        }
        private static RESTStatus REST_GET_Blanks_User(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                if (CurrentInstance.obj.Blanks.ContainsKey(query.URLParameters["username"]))
                {
                    ret.Status = STATUS_SUCCESS;
                    ret.StatusMessage = "Successfully completed";
                    ret.Data = CurrentInstance.obj.Blanks[query.URLParameters["username"]];
                }
                else
                {
                    ret.Status = STATUS_SUCCESS;
                    ret.StatusMessage = "Successfully completed (User does not exist)";
                    ret.Data = 0UL;
                }
            }
            return ret;
        }
        private static RESTStatus REST_GET_Purchase(RESTQuery query,RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            lock (CurrentInstance.lockObject)
            {
                if (CurrentInstance.obj.Blanks.ContainsKey(query.URLParameters["username"]))
                {
                    ulong cblanks = CurrentInstance.obj.Blanks[query.URLParameters["username"]];
                    ulong cost = ulong.Parse(query.QueryParameters["cost"]);
                    if (cost <= cblanks)
                    {
                        ret.Status = STATUS_SUCCESS;
                        ret.StatusMessage = "User does not have enough Blanks";
                        ret.Data = cblanks - cost;
                        CurrentInstance.obj.Blanks[query.URLParameters["username"]] = cblanks - cost;
                        CurrentInstance.obj.Save(CurrentInstance.filepath);
                    }
                    else
                    {
                        ret.Status = STATUS_FAILURE;
                        ret.StatusMessage = "Successfully completed";
                        ret.Data = cblanks - cost;
                    }
                }
                else
                {
                    ret.Status = STATUS_FAILURE;
                    ret.StatusMessage = "User does not have enough Blanks";
                    ret.Data = 0UL;
                }
            }
            return ret;
        }
        private static RESTStatus REST_PUT_Blanks(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            Dictionary<string, ulong> body = JSONSerializer.Deserialize<Dictionary<string, ulong>>(query.Body);
            lock (CurrentInstance.lockObject)
            {
                ret.Status = STATUS_SUCCESS;
                ret.StatusMessage = "Successfully completed";
                ret.Data = null;
                foreach(string key in body.Keys)
                {
                    if (!CurrentInstance.obj.Blanks.ContainsKey(key))
                        CurrentInstance.obj.Blanks[key] = 0;
                    CurrentInstance.obj.Blanks[key] += body[key];
                }
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }

        private static RESTStatus REST_GET_TTS_Enabled(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                ret.Data = CurrentInstance.obj.enable_tts;
            }
            return ret;
        }
        private static RESTStatus REST_PUT_TTS_Enabled(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                CurrentInstance.obj.enable_tts = query.Body.ToLower() == "true";
                ret.Data = CurrentInstance.obj.enable_tts;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }
        private static RESTStatus REST_GET_TTS_Enable(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                CurrentInstance.obj.enable_tts = true;
                ret.Data = true;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }
        private static RESTStatus REST_GET_TTS_Disable(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                CurrentInstance.obj.enable_tts = false;
                ret.Data = false;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }
        private static RESTStatus REST_GET_TTS_Banned(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                ret.Data = CurrentInstance.obj.banned_tts;
            }
            return ret;
        }
        private static RESTStatus REST_GET_TTS_Banned_User(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                ret.Data = CurrentInstance.obj.banned_tts.Contains(query.URLParameters["username"]);
            }
            return ret;
        }
        private static RESTStatus REST_GET_TTS_Ban_User(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            string user = query.URLParameters["username"];
            lock (CurrentInstance.lockObject)
            {
                if (!CurrentInstance.obj.banned_tts.Contains(query.URLParameters["username"]))
                {
                    ret.Status = STATUS_SUCCESS;
                    ret.StatusMessage = "Successfully Completed";
                    var x = CurrentInstance.obj.banned_tts.ToList();
                    x.Add(user);
                    CurrentInstance.obj.banned_tts = x.ToArray();
                    CurrentInstance.obj.Save(CurrentInstance.filepath);
                    ret.Data = null;
                }
                else
                {
                    ret.Status = STATUS_FAILURE;
                    ret.StatusMessage = "User \"" + user + "\" is already banned.";
                    ret.Data = null;
                }
            }
            return ret;
        }
        private static RESTStatus REST_GET_TTS_Unban_User(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus();
            string user = query.URLParameters["username"];
            lock (CurrentInstance.lockObject)
            {
                if (CurrentInstance.obj.banned_tts.Contains(query.URLParameters["username"]))
                {
                    ret.Status = STATUS_SUCCESS;
                    ret.StatusMessage = "Successfully Completed";
                    var x = CurrentInstance.obj.banned_tts.ToList();
                    x.Remove(user);
                    CurrentInstance.obj.banned_tts = x.ToArray();
                    CurrentInstance.obj.Save(CurrentInstance.filepath);
                    ret.Data = null;
                }
                else
                {
                    ret.Status = STATUS_FAILURE;
                    ret.StatusMessage = "User \"" + user + "\" is not banned.";
                    ret.Data = null;
                }
            }
            return ret;
        }

        private static RESTStatus REST_GET_LastFollow(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                ret.Data = CurrentInstance.obj.lastfollow;
            }
            return ret;
        }
        private static RESTStatus REST_PUT_LastFollow(RESTQuery query, RESTMethodDefinition self)
        {
            RESTStatus ret = new RESTStatus() { Status = STATUS_SUCCESS, StatusMessage = "Successfully Completed" };
            lock (CurrentInstance.lockObject)
            {
                ret.Data = CurrentInstance.obj.lastfollow = query.Body;
                CurrentInstance.obj.Save(CurrentInstance.filepath);
            }
            return ret;
        }
    }
    public struct BotQuote
    {
        public ulong number;
        public string quote;
    }
    public class BotStorageObject
    {
        public string demo="";
        public Dictionary<string, string> custom_commands = new Dictionary<string, string>();
        public object poll_data = null;
        public string[] quotes = new string[] { };
        public Dictionary<string, ulong> Blanks = new Dictionary<string, ulong>();
        public bool enable_tts = false;
        public string[] banned_tts = new string[] { };
        public string lastfollow = "";//datetime
        public Dictionary<string, ulong> urlban_breakers = new Dictionary<string, ulong>();
        public string[] quotes_rec = new string[] { };


        public static BotStorageObject FromFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;
            return JSONSerializer.Deserialize<BotStorageObject>(File.ReadAllText(filename));
        }
        public void Save(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return;
            File.WriteAllText(filename, JSONSerializer.Serialize<BotStorageObject>(this));
        }

        /// <summary>
        /// Adds the given quotes to the list of quotes, and returns the index of the added quote
        /// </summary>
        /// <param name="quote">The quote to add.</param>
        /// <returns>The index of the quote.</returns>
        public int AddQuote(string quote)
        {
            var qts = quotes.ToList();
            qts.Add(quote);
            quotes = qts.ToArray();
            return quotes.Length - 1;
        }
    }
    class PortConnection
    {
        NetworkStream stream;
        public readonly TcpClient client;
        List<byte> cache;
        bool disposed;
        public PortConnection(TcpClient client,NetworkStream stream)
        {
            this.stream = stream;
            this.client = client;
            cache = new List<byte>();
            disposed = false;
        }
        public byte[] Read(ref int count)
        {
            byte[] ret = new byte[count];
            count = stream.Read(ret, 0, count);
            return ret;
        }
        public byte ReadByte()
        {
            return (byte)stream.ReadByte();
        }
        public string ReadLine()
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    byte[] buffer = new byte[4096];
                    int readcount = stream.Read(buffer, 0, 4096);
                    if (readcount < 4096)
                    {
                        var tmp = buffer.ToList();
                        tmp.RemoveRange(readcount, 4096 - readcount);
                        cache.AddRange(tmp);
                        break;
                    }
                    else
                        cache.AddRange(buffer);
                }
                int pos = SearchForLineBreak();
                if (pos == -1)
                    Thread.Sleep(1000);//CR+LF not found, waiting
                else
                {
                    var line = cache.GetRange(0, pos + 1);
                    cache.RemoveRange(0, pos + 1);//remove the data from the cache
                    return Encoding.UTF8.GetString(line.ToArray()).Replace("\r\n", "");//remove line ending
                }
            }
            throw new NotImplementedException();
        }
        public string Read()
        {
            int rlen = 21;
            while (true)
            {
                byte[] buffer = new byte[4096];
                rlen = stream.Read(buffer, 0, buffer.Length);
                if (rlen < 4096)
                {
                    var tmp = buffer.ToList();
                    tmp.RemoveRange(rlen, 4096 - rlen);
                    cache.AddRange(tmp);
                    break;
                }
                else
                    cache.AddRange(buffer);
            }
            string ret = Encoding.UTF8.GetString(cache.ToArray());
            cache = new List<byte>();
            return ret;
        }
        public byte[] ReadAllBytes()
        {
            int rlen = 21;
            while (true)
            {
                byte[] buffer = new byte[4096];
                rlen = stream.Read(buffer, 0, buffer.Length);
                if (rlen < 4096)
                {
                    var tmp = buffer.ToList();
                    tmp.RemoveRange(rlen, 4096 - rlen);
                    cache.AddRange(tmp);
                    break;
                }
                else
                    cache.AddRange(buffer);
            }
            var ret = cache.ToArray();
            cache = new List<byte>();
            return ret;
        }
        private int SearchForLineBreak()
        {
            //searches for "\r\n" 13 10 in the cache list
            //returns the index of the line feed (10) upon success
            //returns -1 otherwise
            bool carriagereturn = false;
            for (int i = 0; i < cache.Count; i++)
            {
                if (carriagereturn)
                {
                    if (cache[i] == 10)
                        return i;
                    else
                        carriagereturn = false;
                }
                else if (cache[i] == 13)
                    carriagereturn = true;
            }
            return -1;
        }
        public void Write(byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }
        public void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }
        public void WriteLine(string line)
        {
            if (!line.EndsWith("\r\n"))
                line += "\r\n";
            byte[] buffer = Encoding.UTF8.GetBytes(line);
            stream.Write(buffer, 0, buffer.Length);
        }
        public void WriteLine()
        {
            WriteLine("");
        }
        public void Write(string value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
        public void Dispose()
        {
            if(!disposed)
            {
                disposed = true;
                client.Close();
                stream = null;
            }
        }
    }
    public class RESTServer
    {
        const ushort port = 40000;
        Thread restthread;
        List<RESTMethodDefinition> definitions;
        //[Request Type] [Resource] HTTP/[Version]
        //[HeaderName]: [HeaderValue]
        //[blank name]
        //[body]
        public RESTServer()
        {
            definitions = new List<RESTMethodDefinition>();
        }
        private void RESTThread()
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            listener.Start();
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                PortConnection connection = new PortConnection(client, client.GetStream());
                Thread t = new Thread(new ParameterizedThreadStart(RESTClientThread));
                t.Start(connection);
            }
        }
        private void RESTClientThread(object data)
        {
            PortConnection connection = (PortConnection)data;
            string RequestLine = connection.ReadLine();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            while (true)
            {
                string header = connection.ReadLine();
                if (header.Length == 0)
                    break;
                else
                {
                    var parts = header.Split(new char[] { ':' }, 2).Select(a => a.Trim()).ToArray();
                    headers[parts[0]] = parts[1];
                }
            }

            string body = "";
            if (connection.client.Available>0)
                body = connection.Read();
            string[] reqLine = RequestLine.Split(' ');
            string _method = reqLine[0];
            string _path = reqLine[1];
            string _query = (_path.Split('?').Length > 1) ? _path.Split('?')[1] : "";
            _path = _path.Split('?')[0];
            Dictionary<string, string> _queryparams = new Dictionary<string, string>();
            foreach(string qp in _query.Split('&'))
            {
                var temp = qp.Split('=');
                if (temp.Length == 1)
                    _queryparams.Add(temp[0], "");
                else
                    _queryparams.Add(temp[0], DecodeURIComponent(temp[1]));
            }
            RESTQuery query = new RESTQuery()
            {
                Body = body,
                Method = _method,
                Path = _path,
                Headers = headers,
                QueryParameters = _queryparams
            };
            RESTMethodDefinition method = default(RESTMethodDefinition);
            foreach(RESTMethodDefinition m in definitions)
            {
                if(m.Match(_method, reqLine[1]))
                {
                    method = m;
                    break;
                }
            }
            if (method != default(RESTMethodDefinition))
            {
                query.URLParameters = method.GetURLParameters(reqLine[1]);
                RESTStatus ret = null;
                try { ret = method.handler(query, method); }
                catch (Exception err) { ret = new RESTStatus() { Status = "Error", StatusMessage = "An exception has occurred!", Data = err }; }
                //all return types should be classes
                SendResponse(connection, 200, JSONSerializer.Serialize(ret), "application/json");
            }
            else //SEND 400
                SendResponse(connection, 400, JSONSerializer.Serialize(new RESTStatus() { Status = "Failure", StatusMessage = "Unknown API Endpoint" }), "application/json");
            connection.Dispose();
        }
        public void Start()
        {
            restthread = new Thread(new ThreadStart(RESTThread));
            restthread.Start();
        }
        public void Stop()
        {
            restthread.Abort();
        }
        public void Register(RESTMethodDefinition definition)
        {
            //registers a REST method using the "request type" "url params" "query params" "header params" and "body params"
            definitions.Add(definition);
        }
        public string DecodeURIComponent(string tmp)
        {
            return Uri.EscapeDataString(tmp);
        }
        private void SendResponse(PortConnection conn, short responseCode, string responseData, string contentType)
        {
            conn.WriteLine("HTTP/1.1 " + responseCode + " " + GetResponseCode(responseCode));
            conn.WriteLine("Server: NULL");
            conn.WriteLine("Content-Length: " + responseData.Length);
            conn.WriteLine("Connection: close");
            conn.WriteLine("Content-Type: " + contentType);
            conn.WriteLine();
            conn.Write(responseData);
        }
        private void SendResponse(PortConnection conn, short responseCode, byte[] responseData, string contentType)
        {
            conn.WriteLine("HTTP/1.1 " + responseCode + " " + GetResponseCode(responseCode));
            conn.WriteLine("Server: NULL");
            conn.WriteLine("Content-Length: " + responseData.Length);
            conn.WriteLine("Connection: close");
            conn.WriteLine("Content-Type: " + contentType);
            conn.WriteLine();
            conn.Write(responseData);
        }
        private void SendResponse(PortConnection conn, short responseCode)
        {
            conn.WriteLine("HTTP/1.1 " + responseCode + " " + GetResponseCode(responseCode));
            conn.WriteLine("Server: NULL");
            conn.WriteLine("Content-Length: 0");
            conn.WriteLine("Connection: close");
            conn.WriteLine("Content-Type: text/plain");
            conn.WriteLine();
        }
        private string GetResponseCode(short numeric)
        {
            switch (numeric)
            {
                case 100:return "CONTINUE";
                case 101:return "SWITCHING PROTOCOLS";
                case 102:return "PROCESSING";
                case 200:return "OK";
                case 201:return "CREATED";
                case 202:return "ACCEPTED";
                case 203:return "NON-AUTHORITATIVE INFORMATION";
                case 204:return "NO CONTENT";
                case 205:return "RESET CONTENT";
                case 206:return "PARTIAL CONTENT";
                case 207:return "MULTI-STATUS";
                case 208:return "ALREADY REPORTED";
                case 226:return "IM USED";
                case 300:return "MULTIPLE CHOICES";
                case 301:return "MOVED PERMANENTLY";
                case 302:return "FOUND";
                case 303:return "SEE OTHER";
                case 304:return "NOT MODIFIED";
                case 305:return "USE PROXY";
                case 306:return "SWITCH PROXY";
                case 307:return "TEMPORARY REDIRECT";
                case 308:return "PERMANENT REDIRECT";
                case 400:return "BAD REQUEST";
                case 401:return "UNAUTHORIZED";
                case 402:return "PAYMENT REQUIRED";
                case 403:return "FORBIDDEN";
                case 404:return "NOT FOUND";
                case 405:return "METHOD NOT ALLOWED";
                case 406:return "NOT ACCEPTABLE";
                case 407:return "PROXY AUTHENTICATION REQUIRED";
                case 408:return "REQUEST TIMEOUT";
                case 409:return "CONFLICT";
                case 410:return "GONE";
                case 411:return "LENGTH REQUIRED";
                case 412:return "PRECONDITION FAILED";
                case 413:return "PAYLOAD TOO LARGE";
                case 414:return "URI TOO LONG";
                case 415:return "UNSUPPORTED MEDIA TYPE";
                case 416:return "RANGE NOT SATISFIABLE";
                case 417:return "EXPECTATION FAILED";
                case 418:return "I'M A TEAPOT";
                case 421:return "MISDIRECTED REQUEST";
                case 422:return "UNPROCESSABLE ENTITY";
                case 423:return "LOCKED";
                case 424:return "FAILED DEPENDENCY";
                case 426:return "UPGRADE REQUIRED";
                case 428:return "PRECONDITION REQUIRED";
                case 429:return "TOO MANY REQUESTS";
                case 431:return "REQUEST HEADER FIELDS TOO LARGE";
                case 451:return "UNAVAILABLE FOR LEGAL REASONS";
                default:
                case 500:return "INTERNAL SERVER ERROR";
                case 501:return "NOT IMPLEMENTED";
                case 502:return "BAD GATEWAY";
                case 503:return "SERVICE UNAVAILABLE";
                case 504:return "GATEWAY TIMEOUT";
                case 505:return "HTTP VERSION NOT SUPPORTED";
                case 506:return "VARIANT ALSO NEGOTIATES";
                case 507:return "INSUFFICIENT STORAGE";
                case 508:return "LOOP DETECTED";
                case 510:return "NOT EXTENDED";
                case 511:return "NETWORK AUTHENTICATION REQUIRED";
            }
        }
    }
    public class RESTQuery
    {
        public Dictionary<string,string> Headers { get; set; }
        public Dictionary<string,string> QueryParameters { get; set; }
        public Dictionary<string,string> URLParameters { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
    }
    public struct RESTQueryParameter
    {
        public bool Required { get; set; }
        public string Name { get; set; }

        public static bool operator ==(RESTQueryParameter a, RESTQueryParameter b)
        {
            return (a.Required == b.Required) &&
                (a.Name == b.Name);
        }
        public static bool operator !=(RESTQueryParameter a,RESTQueryParameter b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public struct RESTMethodDefinition
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public RESTQueryParameter[] QueryParameters { get; set; }
        public RESTMethod handler { get; set; }

        public bool Match(string Method,string Request)
        {
            var temp = Request.Split('?');
            string querytemp = (temp.Length == 2) ? temp[1] : "";
            List<string> queryparams = querytemp.ToUpper().Split('&').Select(a => a.Split('=')[0]).ToList();
            string[] urlparts = temp[0].ToLower().Split('/').Select(a => a.Trim()).ToArray();
            string[] pathparts = Path.ToLower().Split('/').Select(a=>a.Trim()).ToArray();
            if (Method.ToUpper() != Method.ToUpper())
                return false;
            if (urlparts.Length != pathparts.Length)
                return false;
            for(int i = 0; i < pathparts.Length; i++)
            {
                if (pathparts[i].StartsWith("{") && pathparts[i].EndsWith("}"))
                    continue;
                if (pathparts[i] != urlparts[i])
                    return false;
            }
            foreach(RESTQueryParameter param in QueryParameters)
            {
                if (!param.Required)
                {
                    if(queryparams.Contains(param.Name.ToUpper()))
                        queryparams.Remove(param.Name.ToUpper());
                    continue;
                }
                if (!queryparams.Contains(param.Name.ToUpper()))
                    return false;
                queryparams.Remove(param.Name.ToUpper());
            }
            if (queryparams.Count > 0 && (!(queryparams.Count == 1 && queryparams[0].Length == 0)))
                return false;
            return true;
        }
        public Dictionary<string,string> GetURLParameters(string Request)
        {
            string[] urlparts = Request.Split('?')[0].ToLower().Split('/').Select(a => a.Trim()).ToArray();
            string[] pathparts = Path.ToLower().Split('/').Select(a => a.Trim()).ToArray();
            Dictionary<string, string> ret = new Dictionary<string, string>();
            for (int i = 0; i < pathparts.Length; i++)
                if (pathparts[i].StartsWith("{") && pathparts[i].EndsWith("}"))
                    ret.Add(pathparts[i].Substring(1, pathparts[i].Length - 2), urlparts[i]);
            return ret;
        }

        public static bool operator ==(RESTMethodDefinition a, RESTMethodDefinition b)
        {

            return (a.Method == b.Method) &&
                (a.Path == b.Path) && (a.QueryParameters == b.QueryParameters);
        }
        public static bool operator !=(RESTMethodDefinition a,RESTMethodDefinition b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public delegate RESTStatus RESTMethod(RESTQuery query, RESTMethodDefinition definition);
    public class JSONSerializer
    {
        public static T Deserialize<T>(string JSON)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(JSON);
        }
        public static Dictionary<string,object> Deserialize(string JSON)
        {
            return DeserializeObject(JSON);
        }
        public static string Serialize<T>(T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            //return SerializeObject(typeof(T), obj);
        }
        public static string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            //return SerializeObject(obj.GetType(), obj);
        }


        private static string SerializeObject(Type typeref,object obj)
        {
            //base serialize function
            if (obj == null)
                return SerializeNull();
            else if (typeref == typeof(int))
                return SerializeNumber((int)obj);
            else if (typeref == typeof(uint))
                return SerializeNumber((uint)obj);
            else if (typeref == typeof(long))
                return SerializeNumber((long)obj);
            else if (typeref == typeof(ulong))
                return SerializeNumber((ulong)obj);
            else if (typeref == typeof(short))
                return SerializeNumber((short)obj);
            else if (typeref == typeof(ushort))
                return SerializeNumber((ushort)obj);
            else if (typeref == typeof(sbyte))
                return SerializeNumber((sbyte)obj);
            else if (typeref == typeof(byte))
                return SerializeNumber((byte)obj);
            else if (typeref == typeof(float))
                return SerializeNumber((float)obj);
            else if (typeref == typeof(double))
                return SerializeNumber((double)obj);
            else if (typeref == typeof(decimal))
                return SerializeNumber((decimal)obj);
            else if (typeref == typeof(char))
                return SerializeString((char)obj + "");
            else if (typeref == typeof(char[]))
                return SerializeString(new string((char[])obj));
            else if (typeref == typeof(string))
                return SerializeString((string)obj);
            else if (typeref == typeof(bool))
                return SerializeBoolean((bool)obj);
            else if (typeref.IsArray)
                return SerializeArray(typeref, obj);
            else if (typeref == typeof(Dictionary<string, object>))
                return SerializeDictionary((Dictionary<string, object>)obj);
            else
                return SerializeClass(typeref, obj);
        }
        private static string SerializeClass(Type typeref,object obj)
        {
            //base CLASS serializer (uses reflection to serialize a class)
            //generally speaking, the root of the object SHOULD be a class, or an array of CLASS
            if (obj == null)
                return SerializeNull();
            PropertyInfo[] pinfo = typeref.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] finfo = typeref.GetFields(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, object> ret = new Dictionary<string, object>();
            foreach (PropertyInfo p in pinfo)
                ret.Add(p.Name, p.GetValue(obj));
            foreach (FieldInfo f in finfo)
                ret.Add(f.Name, f.GetValue(obj));
            return SerializeDictionary(ret);
        }
        private static string SerializeDictionary(Dictionary<string,object> obj)
        {
            //base DICTIONARY<string,object> serializer, serializes the same way as class, except no reflection
            if (obj == null)
                return SerializeNull();
            List<string> sets = new List<string>();
            foreach(string key in obj.Keys)
            {
                if (obj[key] == null)
                    sets.Add(SerializeString(key) + ":" + SerializeNull());
                else
                    sets.Add(SerializeString(key) + ":" + SerializeObject(obj[key].GetType(), obj[key]));
            }
                
            return "{" + string.Join(",", sets.ToArray()) + "}";
        }
        private static string SerializeArray(Type typeref, object obj)
        {
            //base ARRAY serializer
            if (obj == null)
                return SerializeNull();
            dynamic dyn = (dynamic)obj;
            List<string> ele = new List<string>();
            foreach (object x in dyn)
            {
                if (x == null)
                    ele.Add(SerializeNull());
                else
                    ele.Add(SerializeObject(x.GetType(), x));
            }
            return "[" + string.Join(",", ele.ToArray()) + "]";
        }
        private static string SerializeBoolean(bool obj)
        {
            return (obj) ? "true" : "false";
        }
        private static string SerializeNumber(long obj)
        {
            return obj.ToString();
        }
        private static string SerializeNumber(ulong obj)
        {
            return obj.ToString();
        }
        private static string SerializeNumber(float obj)
        {
            return obj.ToString();//may bug
        }
        private static string SerializeNumber(double obj)
        {
            return obj.ToString();//may bug
        }
        private static string SerializeNumber(decimal obj)
        {
            return obj.ToString();//may bug
        }
        private static string SerializeNull()
        {
            return "null";
        }
        private static string SerializeString(string obj)
        {
            return "\"" + obj.Replace("'", "\\'").Replace("\"", "\\\"")
                .Replace("\\", "\\\\").Replace("\b", "\\b").Replace("\r", "\\r")
                .Replace("\f", "\\f").Replace("\t", "\\t").Replace("\v", "\\v") + "\"";
        }

        private static Dictionary<string,object> DeserializeObject(string JSON)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(JSON);
        }
    }
    public class RESTStatus
    {
        public string Status { get; set;}
        public string StatusMessage { get; set; }
        public object Data { get; set; }
    }
}
