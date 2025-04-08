namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the result of AI analysis of meeting content
    /// </summary>
    public class AIAnalysisResult
    {
        /// <summary>
        /// Gets or sets brief summary of the meeting (max 200 words)
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets list of key points discussed during the meeting
        /// </summary>
        public List<string> KeyPoints { get; set; } = new ();

        /// <summary>
        /// Gets or sets main topics covered in the meeting
        /// </summary>
        public List<string> Topics { get; set; } = new ();

        /// <summary>
        /// Gets or sets questions and their answers from the meeting
        /// </summary>
        public List<QASegment> QASegments { get; set; } = new ();
    }
}
