using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Processes audio input and converts it to WAV format
    /// </summary>
    public class AudioProcessor : IAudioProcessor
    {
        private readonly ILogger<AudioProcessor> logger;
        private readonly Dictionary<string, IAudioFormatHandler> formatHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioProcessor"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging audio processing operations.</param>
        /// <param name="handlers">Collection of audio format handlers.</param>
        public AudioProcessor(
            ILogger<AudioProcessor> logger,
            IEnumerable<IAudioFormatHandler> handlers)
        {
            this.logger = logger;
            formatHandlers = handlers.ToDictionary(h => h.SupportedExtension, h => h);
        }

        /// <inheritdoc/>
        public async Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, string fileExtension, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Processing audio with extension: {Extension}", fileExtension);

                if (!IsFormatSupported(fileExtension))
                {
                    throw new AudioProcessingException($"Unsupported audio format: {fileExtension}");
                }

                var handler = formatHandlers[fileExtension];
                return await handler.ConvertToWavAsync(audioStream, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing audio");
                throw new AudioProcessingException("Failed to process audio", ex);
            }
        }

        /// <inheritdoc/>
        public bool IsFormatSupported(string fileExtension)
        {
            return formatHandlers.ContainsKey(fileExtension);
        }

        /// <inheritdoc/>
        public async Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream, string fileExtension)
        {
            try
            {
                logger.LogInformation("Getting format information for extension: {Extension}", fileExtension);

                if (!IsFormatSupported(fileExtension))
                {
                    throw new AudioProcessingException($"Unsupported audio format: {fileExtension}");
                }

                var handler = formatHandlers[fileExtension];
                return await handler.GetFormatInfoAsync(audioStream);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting format information");
                throw new AudioProcessingException("Failed to get format information", ex);
            }
        }
    }
}
