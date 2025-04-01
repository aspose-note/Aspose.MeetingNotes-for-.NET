namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents analyzed content from a meeting with structured sections and metadata
    /// </summary>
    public class AnalyzedContent
    {
        /// <summary>
        /// Brief summary of the meeting content
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Structured sections of the meeting content
        /// </summary>
        public List<ContentSection> Sections { get; set; } = new();

        /// <summary>
        /// Key points extracted from the meeting
        /// </summary>
        public List<string> KeyPoints { get; set; } = new();

        /// <summary>
        /// Questions and answers identified in the meeting
        /// </summary>
        public List<QASegment> QASegments { get; set; } = new();

        /// <summary>
        /// The complete transcribed text without any analysis or structuring
        /// </summary>
        public string TranscribedText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a section of analyzed content with a title and content
    /// </summary>
    public class ContentSection
    {
        /// <summary>
        /// Title of the content section
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content of the section
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a question and answer segment from the meeting
    /// </summary>
    public class QASegment
    {
        /// <summary>
        /// The question asked during the meeting
        /// </summary>
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// The answer provided during the meeting
        /// </summary>
        public string Answer { get; set; } = string.Empty;
    }
} 