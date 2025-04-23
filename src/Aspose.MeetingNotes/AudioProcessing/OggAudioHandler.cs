using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;
using Concentus.Oggfile;
using Concentus.Structs;
using Microsoft.Extensions.Logging;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NVorbis;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Handler for OGG audio format
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="OggAudioHandler"/> class.
    /// </remarks>
    /// <param name="logger"></param>
    public class OggAudioHandler(ILogger<OggAudioHandler> logger) : BaseAudioFormatHandler(logger)
    {
        private const int TargetSampleRate = 16000; // Whisper requires 16kHz
        private const int TargetChannels = 1;       // Whisper works better with mono

        public override string SupportedExtension => ".ogg";

        public override async Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Processing OGG audio stream");

                // Try to detect the codec
                var buffer = new byte[4];
                await audioStream.ReadAsync(buffer, 0, 4, cancellationToken);
                audioStream.Position = 0;

                return await ProcessOpusStreamAsync(audioStream, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing OGG audio");
                throw new AudioProcessingException("Failed to process OGG audio", ex);
            }
        }

        public override async Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream)
        {
            try
            {
                logger.LogInformation("Getting OGG format information");

                // Try to detect the codec
                var buffer = new byte[4];
                await audioStream.ReadAsync(buffer, 0, 4);
                audioStream.Position = 0;

                return new AudioFormatInfo
                {
                    SampleRate = TargetSampleRate,
                    Channels = TargetChannels,
                    BitsPerSample = 16,
                    OriginalFormat = "OGG (Opus)"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting OGG format information");
                throw new AudioProcessingException("Failed to get OGG format information", ex);
            }
        }

        private async Task<ProcessedAudio> ProcessVorbisStreamAsync(Stream audioStream, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing Vorbis stream");

            // Create a VorbisReader from the input stream
            using var vorbisReader = new VorbisReader(audioStream, true);

            // Create a memory stream for the processed audio
            var memoryStream = new MemoryStream();

            // Convert Vorbis to WAV format
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(vorbisReader.SampleRate, vorbisReader.Channels);
            using var waveStream = new WaveFileWriter(memoryStream, waveFormat);

            // Read and write audio data
            var buffer = new float[4096];
            int read;
            while ((read = vorbisReader.ReadSamples(buffer, 0, buffer.Length)) > 0)
            {
                waveStream.WriteSamples(buffer, 0, read);
            }

            memoryStream.Position = 0;

            return CreateProcessedAudio(
                memoryStream,
                vorbisReader.SampleRate,
                vorbisReader.Channels,
                TimeSpan.FromSeconds(vorbisReader.TotalTime.TotalSeconds));
        }

        private async Task<ProcessedAudio> ProcessOpusStreamAsync(Stream audioStream, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing Opus stream");

            try
            {
                // Create a memory stream for PCM data
                using var pcmStream = new MemoryStream();

                // Create Opus decoder and OGG reader
                var decoder = OpusDecoder.Create(48000, 2); // Opus typically uses 48kHz and stereo
                var oggIn = new OpusOggReadStream(decoder, audioStream);

                // Decode all packets
                while (oggIn.HasNextPacket)
                {
                    var packet = oggIn.DecodeNextPacket();
                    if (packet != null)
                    {
                        // Convert short[] to bytes and write to PCM stream
                        for (int i = 0; i < packet.Length; i++)
                        {
                            var bytes = BitConverter.GetBytes(packet[i]);
                            await pcmStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                        }
                    }
                }

                pcmStream.Position = 0;

                // Create initial WAV format from PCM data (48kHz, stereo)
                var sourceFormat = new WaveFormat(48000, 2);
                using var sourceStream = new RawSourceWaveStream(pcmStream, sourceFormat);

                // Convert to mono if needed
                var monoStream = sourceStream.ToSampleProvider().ToMono();

                // Resample to 16kHz
                var resampler = new WdlResampler();
                resampler.SetMode(true, 2, false);
                resampler.SetFilterParms();
                resampler.SetFeedMode(true);

                var resampledProvider = new WdlResamplingSampleProvider(monoStream, TargetSampleRate);

                // Create final WAV stream
                var wavMemoryStream = new MemoryStream();
                WaveFileWriter.WriteWavFileToStream(wavMemoryStream, resampledProvider.ToWaveProvider16());
                wavMemoryStream.Position = 0;

                return CreateProcessedAudio(
                    wavMemoryStream,
                    TargetSampleRate,
                    TargetChannels,
                    TimeSpan.FromSeconds(wavMemoryStream.Length / (TargetSampleRate * TargetChannels * 2.0))); // 16-bit samples
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing Opus stream");
                throw new AudioProcessingException("Failed to process Opus stream", ex);
            }
        }
    }
}
