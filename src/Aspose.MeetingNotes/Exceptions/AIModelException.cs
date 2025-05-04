namespace Aspose.MeetingNotes.Exceptions;

/// <summary>
/// Represents errors that occur during interactions with an AI model,
/// such as request failures, response parsing errors, or unexpected model behavior.
/// Inherits from <see cref="MeetingNotesException"/>.
/// </summary>
public class AIModelException : MeetingNotesException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIModelException"/> class.
    /// </summary>
    public AIModelException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AIModelException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error related to the AI model.</param>
    public AIModelException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AIModelException"/> class
    /// with a specified error message and a reference to the inner exception that is
    /// the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the AI model exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception (e.g., HttpRequestException, JsonException).</param>
    public AIModelException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
