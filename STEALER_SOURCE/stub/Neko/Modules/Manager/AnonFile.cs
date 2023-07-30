using System;

namespace deepside
{
    internal sealed class AnonFiles
    {
        public static string Upload(string file, bool api = false)
        {
            try
            {
                return AnonFile.Api.Upload(file, api);
            }
            catch (Exception error)
            {
                //Logging.Log("AnonFile Upload : Connection error\n" + error);
                //System.Console.WriteLine("Connection error");
                System.Console.ReadLine();
            }
            return null;
        }
    }
}
