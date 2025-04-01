namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents analyzed content from a meeting with structured sections and metadata
    /// </summary>
    public class AnalyzedContent
    {
        /// <summary>
        /// Gets or sets brief summary of the meeting content
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets structured sections of the meeting content
        /// </summary>
        public List<ContentSection> Sections { get; set; } = new ();

        /// <summary>
        /// Gets or sets key points extracted from the meeting
        /// </summary>
        public List<string> KeyPoints { get; set; } = new ();

        /// <summary>
        /// Gets or sets questions and answers identified in the meeting
        /// </summary>
        public List<QASegment> QASegments { get; set; } = new ();

        /// <summary>
        /// Gets or sets the complete transcribed text without any analysis or structuring
        /// </summary>
        public string TranscribedText { get; set; } = string.Empty;
    }
}
