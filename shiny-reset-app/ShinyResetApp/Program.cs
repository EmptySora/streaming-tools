using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

namespace ShinyResetApp {

    static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            /*ResourceSet? set = Resources.ResourceManager.GetResourceSet(new CultureInfo("en-GB"), true, true);
            Console.WriteLine($"Text: {Resources.ResourceManager.GetString("MainFormTitleText", new CultureInfo("en-GB"))}");
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().FullName);
            Console.WriteLine($"Culture: {CultureInfo.CurrentCulture}");
            Resources.Culture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;
            Console.WriteLine($"Culture: {CultureInfo.CurrentCulture}");
            Console.WriteLine($"Text: {Resources.ResourceManager.GetString("cvc", CultureInfo.CurrentCulture)}");*/
            _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
