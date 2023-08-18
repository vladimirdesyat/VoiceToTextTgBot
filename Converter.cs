using NAudio.Wave;
using Concentus.Oggfile;
using Concentus.Structs;

namespace VoiceToTextTgBot
{
    internal class Converter
    {
        public Converter() 
        {
            Convert();
        }

        public void Convert()
        {
            var wavFilePath = Path.Combine(AppContext.BaseDirectory, "voice.wav");

            using (var fileIn = new FileStream(Path.Combine(AppContext.BaseDirectory, "voice.ogg"), FileMode.Open))
            using (var pcmStream = new MemoryStream())
            {
                OpusDecoder decoder = OpusDecoder.Create(48000, 1);
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
                var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(48000, 1));
                var sampleProvider = wavStream.ToSampleProvider();
                WaveFileWriter.CreateWaveFile16(wavFilePath, sampleProvider);
            }
        }
    }
}
