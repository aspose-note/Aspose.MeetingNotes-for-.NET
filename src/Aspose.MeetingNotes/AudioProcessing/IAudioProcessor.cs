using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Processes audio input for transcription
    /// </summary>
    public interface IAudioProcessor
    {
        /// <summary>
        /// Processes the audio input for transcription
        /// </summary>
        /// <param name="audioStream">The audio stream to process</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Processed audio data</returns>
        Task<ProcessedAudio> ProcessAsync(Stream audioStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the audio format is supported
        /// </summary>
        /// <param name="fileExtension">The file extension to check</param>
        /// <returns>True if the format is supported</returns>
        bool IsFormatSupported(string fileExtension);
    }
} 