using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.SpeechRecognition;

/// <summary>
/// Defines the contract for services that perform speech-to-text transcription on audio data.
/// </summary>
public interface ISpeechRecognizer
{
    /// <summary>
    /// Asynchronously transcribes the provided audio stream into text.
    /// Implementations may also perform speaker diarization if supported.
    /// </summary>
    /// <param name="audioStream">The audio stream to transcribe (must be readable, typically WAV format expected by many implementations).</param>
    /// <param name="language">The language code of the audio (e.g., "en", "ru"), or "auto" for automatic detection if supported.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a <see cref="TranscriptionResult"/> object with the transcription details
    /// (text segments, detected language, success status, and potentially speaker information).
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="audioStream"/> is null.</exception>
    /// <exception cref="System.ArgumentException">Thrown if <paramref name="audioStream"/> is not readable, or if <paramref name="language"/> is invalid or unsupported.</exception>
    /// <exception cref="TranscriptionException">Thrown if the transcription process fails due to model errors, configuration issues, or processing errors.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    Task<TranscriptionResult> TranscribeAsync(Stream audioStream, string language, CancellationToken cancellationToken = default);
}
