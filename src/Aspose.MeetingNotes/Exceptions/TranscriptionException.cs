namespace Aspose.MeetingNotes.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an error during speech-to-text transcription
    /// </summary>
    public class TranscriptionException : MeetingNotesException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranscriptionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public TranscriptionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranscriptionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception</param>
        public TranscriptionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
