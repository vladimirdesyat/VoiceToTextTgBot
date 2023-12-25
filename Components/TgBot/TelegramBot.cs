using System.Text.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using VoiceToTextTgBot.Components.ConverterLib;

namespace VoiceToTextTgBot.Components.TgBot
{
    internal class TelegramBot : IDisposable
    {
        string token;
        public ITelegramBotClient botClient;
        private Converter converter;
        private WhisperNet.Whisper whisper;
        public TelegramBot(string Token)
        {
            token = Token;
            botClient = new TelegramBotClient(token);
            converter = new Converter();
            whisper = new WhisperNet.Whisper();
        }
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
                        try
                        {
                            await botClient.DownloadFileAsync(voicePath, saveVoiceStream);

                            Console.WriteLine("Download complete.");

                            Console.WriteLine("Download successful.");
                        }
                        finally 
                        {
                            saveVoiceStream.Close();

                            converter.Convert();

                            await botClient.SendTextMessageAsync(message.Chat, whisper.Start().Result);
                        }                        

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

        public void Dispose()
        {
            // Dispose of resources held by the classes
            converter.Dispose();
            whisper.Dispose();
        }
    }
}
