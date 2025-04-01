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
        /// Gets or sets identified decisions made during the meeting
        /// </summary>
        public List<string> Decisions { get; set; } = new ();

        /// <summary>
        /// Gets or sets questions and their answers from the meeting
        /// </summary>
        public List<QASegment> QASegments { get; set; } = new ();

        /// <summary>
        /// Gets or sets sentiment analysis of the meeting
        /// </summary>
        public SentimentAnalysis Sentiment { get; set; } = new ();

        /// <summary>
        /// Gets or sets participants mentioned in the meeting
        /// </summary>
        public List<ParticipantMention> Participants { get; set; } = new ();

        /// <summary>
        /// Gets or sets any follow-up items or next steps identified
        /// </summary>
        public List<string> FollowUps { get; set; } = new ();

        /// <summary>
        /// Gets or sets confidence score of the analysis (0-1)
        /// </summary>
        public double ConfidenceScore { get; set; }
    }
}
