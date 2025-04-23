using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Processes audio input and converts it to WAV format
    /// </summary>
    public interface IAudioProcessor
    {
        /// <summary>
        /// Processes the audio input and converts it to WAV format
        /// </summary>
        /// <param name="audioStream">The audio stream to process</param>
        /// <param name="fileExtension">The file extension of the audio file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Processed audio data in WAV format</returns>
        Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, string fileExtension, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the audio format is supported
        /// </summary>
        /// <param name="fileExtension">The file extension to check</param>
        /// <returns>True if the format is supported</returns>
        bool IsFormatSupported(string fileExtension);

        /// <summary>
        /// Gets the audio format information from the stream
        /// </summary>
        /// <param name="audioStream">The input audio stream</param>
        /// <param name="fileExtension">The file extension of the audio file</param>
        /// <returns>Audio format information</returns>
        Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream, string fileExtension);
    }
}
