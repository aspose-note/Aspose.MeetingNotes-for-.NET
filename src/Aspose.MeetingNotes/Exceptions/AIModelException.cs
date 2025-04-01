namespace Aspose.MeetingNotes.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an error during AI model operations
    /// </summary>
    public class AIModelException : MeetingNotesException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AIModelException"/> class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public AIModelException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AIModelException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception</param>
        public AIModelException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
