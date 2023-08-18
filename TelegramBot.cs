using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace VoiceToTextTgBot
{
    internal class TelegramBot
    {
        public ITelegramBotClient botClient = new TelegramBotClient("Token");
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                if (message.Voice != null)
                {
                    var voiceFileId = await botClient.GetFileAsync(message.Voice.FileId);

                    var voicePath = voiceFileId.FilePath;

                    using (var saveVoiceStream = new FileStream(AppContext.BaseDirectory + "voice.ogg", FileMode.Create))
                    {
                        await botClient.DownloadFileAsync(voicePath, saveVoiceStream);

                        Console.WriteLine("Download complete.");

                        Console.WriteLine("Download successful.");

                        saveVoiceStream.Close();
                        saveVoiceStream.Dispose();

                        _ = new Converter();

                        string voiceText = Whisper.Start().Result;

                        await botClient.SendTextMessageAsync(message.Chat, voiceText);

                        System.IO.File.Delete(Path.Combine(AppContext.BaseDirectory, "voice.ogg"));
                        System.IO.File.Delete(Path.Combine(AppContext.BaseDirectory, "voice.wav"));
                        System.IO.File.Delete(Path.Combine(AppContext.BaseDirectory, "voice.txt"));

                        return;
                    }
                }
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonSerializer.Serialize(exception));
        }
    }
}
