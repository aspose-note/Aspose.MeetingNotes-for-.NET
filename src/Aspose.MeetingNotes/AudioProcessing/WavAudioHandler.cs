using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Handler for WAV audio format
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="WavAudioHandler"/> class.
    /// </remarks>
    /// <param name="logger"></param>
    public class WavAudioHandler(ILogger<WavAudioHandler> logger) : BaseAudioFormatHandler(logger)
    {
        public override string SupportedExtension => ".wav";

        public override async Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Processing WAV audio stream");

                // Create a WaveStream from the input stream
                using var waveStream = new WaveFileReader(audioStream);

                // Create a memory stream for the processed audio
                var memoryStream = new MemoryStream();

                // Copy the wave data to memory stream
                await waveStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                return CreateProcessedAudio(
                    memoryStream,
                    waveStream.WaveFormat.SampleRate,
                    waveStream.WaveFormat.Channels,
                    waveStream.TotalTime);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing WAV audio");
                throw new AudioProcessingException("Failed to process WAV audio", ex);
            }
        }

        public override async Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream)
        {
            try
            {
                logger.LogInformation("Getting WAV format information");

                using var waveStream = new WaveFileReader(audioStream);

                return new AudioFormatInfo
                {
                    SampleRate = waveStream.WaveFormat.SampleRate,
                    Channels = waveStream.WaveFormat.Channels,
                    BitsPerSample = waveStream.WaveFormat.BitsPerSample,
                    Duration = waveStream.TotalTime,
                    OriginalFormat = "WAV"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting WAV format information");
                throw new AudioProcessingException("Failed to get WAV format information", ex);
            }
        }
    }
}
