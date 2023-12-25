using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using Whisper.net;
using Whisper.net.Ggml;

namespace VoiceToTextTgBot.Components.WhisperNet
{
    internal class Whisper : IDisposable
    {
        private readonly WhisperFactory whisperFactory;
        public Whisper()
        {
            whisperFactory = WhisperFactory.FromPath("ggml-large-v1.bin");
        }
        public async Task<string> Start()
        {
            string output = "";
            var ggmlType = GgmlType.LargeV1;
            var modelName = "ggml-large-v1.bin";
            var wavName = Path.Combine(AppContext.BaseDirectory, "voice.wav");

            if (!File.Exists(modelName))
            {
                await DownloadModel(modelName, ggmlType);
            }            

            using var processor = whisperFactory.CreateBuilder()
            .WithLanguage("auto")
            .WithSpeedUp2x()
            .WithThreads(16)
            .Build();

            using var fileStream = File.OpenRead(wavName);

            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                // Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
                output += result.Text + Environment.NewLine;
            }

            /*
            using (var wr = new StreamWriter(AppContext.BaseDirectory + "voice.txt"))
            {
                wr.WriteLine(output);
                wr.Close();
                wr.Dispose();
                Console.WriteLine("OK");
                return output;
            }
            */

            Console.WriteLine("OK");
            return output;
        }

        public static async Task DownloadModel(string modelName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {modelName}");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(modelName);
            await modelStream.CopyToAsync(fileWriter);
        }

        public void Dispose()
        {
            whisperFactory?.Dispose();
        }
    }
}
