using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using Whisper.net;
using Whisper.net.Ggml;

namespace VoiceToTextTgBot
{
    internal class Whisper
    {
        public static async Task<string> Start()
        {
            string output = "";
            var ggmlType = GgmlType.Small;
            var modelName = "ggml-small.bin";
            var wavName = Path.Combine(AppContext.BaseDirectory, "voice.wav");

            if (!File.Exists(modelName))
            {
                await DownloadModel(modelName, ggmlType);
            }

            using var whisperFactory = WhisperFactory.FromPath("ggml-small.bin");

            using var processor = whisperFactory.CreateBuilder()
            .WithLanguage("auto")
            .Build();

            using var fileStream = File.OpenRead(wavName);
            using var wavStream = new MemoryStream();

            using var reader = new WaveFileReader(fileStream);
            var resampler = new WdlResamplingSampleProvider(reader.ToSampleProvider(), 16000);
            WaveFileWriter.WriteWavFileToStream(wavStream, resampler.ToWaveProvider16());

            wavStream.Seek(0, SeekOrigin.Begin);

            await foreach (var result in processor.ProcessAsync(wavStream))
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
    }
}
