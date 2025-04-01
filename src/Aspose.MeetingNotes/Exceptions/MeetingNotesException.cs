namespace Aspose.MeetingNotes.Exceptions
{
    /// <summary>
    /// Base exception for all MeetingNotes errors
    /// </summary>
    public class MeetingNotesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MeetingNotesException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public MeetingNotesException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the MeetingNotesException class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception</param>
        public MeetingNotesException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception thrown when there is an error processing audio input
    /// </summary>
    public class AudioProcessingException : MeetingNotesException
    {
        /// <summary>
        /// Initializes a new instance of the AudioProcessingException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public AudioProcessingException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the AudioProcessingException class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception</param>
        public AudioProcessingException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception thrown when there is an error during speech-to-text transcription
    /// </summary>
    public class TranscriptionException : MeetingNotesException
    {
        /// <summary>
        /// Initializes a new instance of the TranscriptionException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public TranscriptionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the TranscriptionException class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception</param>
        public TranscriptionException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception thrown when there is an error during AI model operations
    /// </summary>
    public class AIModelException : MeetingNotesException
    {
        /// <summary>
        /// Initializes a new instance of the AIModelException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public AIModelException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the AIModelException class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception</param>
        public AIModelException(string message, Exception inner) : base(message, inner) { }
    }
} 