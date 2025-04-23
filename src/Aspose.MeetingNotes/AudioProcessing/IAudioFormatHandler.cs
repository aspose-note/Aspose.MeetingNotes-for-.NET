using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Interface for handling specific audio formats
    /// </summary>
    public interface IAudioFormatHandler
    {
        /// <summary>
        /// Gets the file extension that this handler supports
        /// </summary>
        string SupportedExtension { get; }

        /// <summary>
        /// Processes the audio stream and converts it to WAV format
        /// </summary>
        /// <param name="audioStream">The input audio stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Processed audio data in WAV format</returns>
        Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the audio format information from the stream
        /// </summary>
        /// <param name="audioStream">The input audio stream</param>
        /// <returns>Audio format information</returns>
        Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream);
    }
}
