namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the complete result of meeting analysis
    /// </summary>
    public class MeetingAnalysisResult
    {
        /// <summary>
        /// The analyzed content of the meeting
        /// </summary>
        public AnalyzedContent Content { get; set; } = new();

        /// <summary>
        /// The list of action items extracted from the meeting
        /// </summary>
        public List<ActionItem> ActionItems { get; set; } = new();

        /// <summary>
        /// The detected language of the meeting
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// The complete transcribed text without any analysis or structuring
        /// </summary>
        public string TranscribedText { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the analysis was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if the analysis failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
} 