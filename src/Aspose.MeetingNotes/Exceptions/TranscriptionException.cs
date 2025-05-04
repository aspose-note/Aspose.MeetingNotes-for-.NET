namespace Aspose.MeetingNotes.Exceptions;

/// <summary>
/// Represents errors that occur during the speech-to-text transcription process.
/// Inherits from <see cref="MeetingNotesException"/>.
/// </summary>
public class TranscriptionException : MeetingNotesException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranscriptionException"/> class.
    /// </summary>
    public TranscriptionException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranscriptionException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error during transcription.</param>
    public TranscriptionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranscriptionException"/> class
    /// with a specified error message and a reference to the inner exception that is
    /// the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the transcription exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception (e.g., error from Whisper library).</param>
    public TranscriptionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
