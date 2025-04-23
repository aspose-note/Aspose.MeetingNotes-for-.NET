using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Base class for audio format handlers
    /// </summary>
    public abstract class BaseAudioFormatHandler : IAudioFormatHandler
    {
        protected readonly ILogger Logger;

        protected BaseAudioFormatHandler(ILogger logger)
        {
            Logger = logger;
        }

        /// <inheritdoc/>
        public abstract string SupportedExtension { get; }

        /// <inheritdoc/>
        public abstract Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream);

        /// <summary>
        /// Creates a processed audio object with the specified parameters.
        /// This method is used to return the processed audio details after conversion.
        /// </summary>
        /// <param name="audioStream">The audio stream of the processed file.</param>
        /// <param name="sampleRate">The sample rate of the processed audio.</param>
        /// <param name="channels">The number of channels of the processed audio.</param>
        /// <param name="duration">The duration of the processed audio.</param>
        /// <returns>A ProcessedAudio object with the provided details.</returns>
        protected ProcessedAudio CreateProcessedAudio(Stream audioStream, int sampleRate, int channels, TimeSpan duration)
        {
            return new ProcessedAudio
            {
                AudioStream = audioStream,
                SampleRate = sampleRate,
                Channels = channels,
                Duration = duration
            };
        }
    }
}
