using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.SpeechRecognition
{
    /// <summary>
    /// Interface for speech recognition operations
    /// </summary>
    public interface ISpeechRecognizer
    {
        /// <summary>
        /// Transcribe audio to text with speaker diarization
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TranscriptionResult> TranscribeAsync(ProcessedAudio audio, string language, CancellationToken cancellationToken = default);
    }
}
