namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the result of speech-to-text transcription with speaker diarization
    /// </summary>
    public class TranscriptionResult
    {
        /// <summary>
        /// List of transcribed segments with speaker identification
        /// </summary>
        public List<TranscriptionSegment> Segments { get; set; } = new();

        /// <summary>
        /// Language code of the transcribed content (e.g., "en", "ru")
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the transcription was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if the transcription failed
        /// </summary>
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Represents a segment of transcribed text with speaker and timing information
    /// </summary>
    public class TranscriptionSegment
    {
        /// <summary>
        /// Identifier of the speaker who spoke this segment
        /// </summary>
        public string Speaker { get; set; } = string.Empty;

        /// <summary>
        /// The transcribed text content
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Start time of the segment in the audio
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// End time of the segment in the audio
        /// </summary>
        public TimeSpan EndTime { get; set; }
    }
} 