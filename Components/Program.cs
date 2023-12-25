using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bots.Http;
using VoiceToTextTgBot.Components.TgBot;

namespace VoiceToTextTgBot.Components
{
    class Program
    {
        static void Main()
        {
            TelegramBot tgBot = null;
            var cts = new CancellationTokenSource();

            while (true)
            {
                Console.WriteLine("Type Telegram Bot Token:");
                var token = Console.ReadLine();                

                tgBot = new TelegramBot(token);                
                
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

                Console.WriteLine("Started bot " + tgBot.botClient.GetMeAsync().Result.FirstName);

                Console.WriteLine("Press 'Enter' to change the token or 'Ctrl+C' to exit.");
                Console.ReadLine();

                if (tgBot != null)
                {
                    // Stop the previous bot by canceling the token
                    cts.Cancel();

                    // Optionally wait for a short time to ensure the bot is stopped
                    Thread.Sleep(1000); // 1 second

                    // Dispose of the previous TelegramBot instance
                    tgBot.Dispose();
                }
            }
        }
    }
}
