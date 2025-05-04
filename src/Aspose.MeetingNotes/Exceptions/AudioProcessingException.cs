namespace Aspose.MeetingNotes.Exceptions;

/// <summary>
/// Represents errors that occur during audio processing, such as format conversion failures
/// or issues running external tools like FFmpeg.
/// Inherits from <see cref="MeetingNotesException"/>.
/// </summary>
public class AudioProcessingException : MeetingNotesException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioProcessingException"/> class.
    /// </summary>
    public AudioProcessingException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioProcessingException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error during audio processing.</param>
    public AudioProcessingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioProcessingException"/> class
    /// with a specified error message and a reference to the inner exception that is
    /// the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the audio processing exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception (e.g., exception from FFmpeg library).</param>
    public AudioProcessingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
