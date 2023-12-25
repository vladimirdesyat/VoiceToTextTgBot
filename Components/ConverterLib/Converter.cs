using NAudio.Wave;
using Concentus.Oggfile;
using Concentus.Structs;
using NAudio.Wave.Compression;

namespace VoiceToTextTgBot.Components.ConverterLib
{
    internal class Converter : IDisposable
    {
        private MemoryStream pcmStream;
        public void Convert()
        {
            var wavFilePath = Path.Combine(AppContext.BaseDirectory, "voice.wav");

            using (var fileIn = new FileStream(Path.Combine(AppContext.BaseDirectory, "voice.ogg"), FileMode.Open))
            {
                var pcmStream = new MemoryStream();
                OpusDecoder decoder = OpusDecoder.Create(16000, 1);
                OpusOggReadStream oggIn = new OpusOggReadStream(decoder, fileIn);
                while (oggIn.HasNextPacket)
                {
                    short[] packet = oggIn.DecodeNextPacket();
                    if (packet != null)
                    {
                        for (int i = 0; i < packet.Length; i++)
                        {
                            var bytes = BitConverter.GetBytes(packet[i]);
                            pcmStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                pcmStream.Position = 0;
                var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(16000, 1));
                var sampleProvider = wavStream.ToSampleProvider();
                WaveFileWriter.CreateWaveFile16(wavFilePath, sampleProvider);
            }
        }

        public void Dispose()
        {
            pcmStream?.Dispose();
        }
    }
}
