using Telegram.Bot;
using Telegram.Bot.Polling;

namespace VoiceToTextTgBot
{
    class Program
    {
        static void Main()
        {
            var tgBot = new TelegramBot();
            Console.WriteLine("Started bot " + tgBot.botClient.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            tgBot.botClient.StartReceiving(
                tgBot.HandleUpdateAsync,
                tgBot.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.ReadLine();
        }
    }
}
