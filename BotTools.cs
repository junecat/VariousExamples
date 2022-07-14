using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TradingG0.BL;

namespace TradingG0.Tools {
    public class BotTools {
        TelegramBotClient bot;
        long chatId = 0;
        StrategyPlayer sp;
        const string crlf = "\r\n";
        public BotTools(TelegramBotClient botClient) {
            bot = botClient;
        }

        // отсылка сообщения от бота в чат
        public void SendMessage(string msg) {
            if (bot != null && chatId > 0 && !string.IsNullOrEmpty(msg)) {
                bot.SendTextMessageAsync(chatId, msg);
            }
        }

        // Этот метод вызывается при приходе сообщения "снаружи"  - когда бот должен его проанализировать
        public void ProcessLine(string msg) {
            if ( bot==null ) return;

            if (msg == "?") {
                SendMessage("Я - телеграмм-бот торгового робота.\r\nДля подробностей спросите меня: help?\r\n");
                return;
            }

            if (msg.ToLower() == "help?") {
                string cr = "\r\n";
                string helpMsg = "Мои команды: \r\n" + cr +
                                 "ПОЗиции? - список открытых позиций," + cr +
                                 "СТРатегии? - список всех стратегий," + cr +
                                 "СТАтус? - список портфелей и время работы робота," + cr +
                                 "STArt название стратегии - для запуска стратегии," + cr +
                                 "STOp название стратегии - для остановки стратегии," + cr +
                                 "и еще:" + cr +
                                 "1) робот присылает уведомления о сделках (и ошибках)" + cr +
                                 "2) части команд, написанные маленькими буквами, можно не писать";
                SendMessage(helpMsg);
                return;
            }

            // ПОЗиции?
            if (msg.Length >= 4 && msg.ToLower().IndexOf("поз") == 0 && msg.Contains("?")) {
                
                if (sp.KnownPositions.Count == 0) {
                    SendMessage("Открытых позиций нет");
                    return;
                }
                string r = string.Empty;
                foreach (KnownPosition kp in sp.KnownPositions.Values) {
                    r += kp.Tool + ": " + kp.Position + crlf;
                }
                SendMessage(r);
                return;
            }

            // СТРатегии?
            if (msg.Length >= 4 && msg.ToLower().IndexOf("стр") == 0 && msg.Contains("?")) {

                if ( sp.Rurs.Count==0 )

                if (sp.KnownPositions.Count == 0) {
                    SendMessage("Список стратегий - пустой");
                    return;
                }
                string r = string.Empty;
                foreach (Rur rur in sp.Rurs.Values) {
                    r += rur.Name + " ("+rur.Tool+")" + crlf;
                }
                SendMessage(r);
                return;
            }



            SendMessage("непонятная команда. попробуйте прислать мне просто знак вопроса");

        }

        public void SetChatId(long chId) {
            chatId = chId;
        }

        public void SetSp(StrategyPlayer strategyPlayer) {
            sp = strategyPlayer;
        }


    }
}
