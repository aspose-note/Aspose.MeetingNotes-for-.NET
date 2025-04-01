namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the result of speech-to-text transcription with speaker diarization
    /// </summary>
    public class TranscriptionResult
    {
        /// <summary>
        /// Gets or sets list of transcribed segments with speaker identification
        /// </summary>
        public List<TranscriptionSegment> Segments { get; set; } = new ();

        /// <summary>
        /// Gets or sets language code of the transcribed content (e.g., "en", "ru")
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether the transcription was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets error message if the transcription failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the complete transcribed text without segmentation
        /// </summary>
        public string FullText { get; set; } = string.Empty;
    }
}