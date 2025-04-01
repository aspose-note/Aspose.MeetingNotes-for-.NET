namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the complete result of meeting analysis
    /// </summary>
    public class MeetingAnalysisResult
    {
        /// <summary>
        /// Gets or sets the analyzed content of the meeting
        /// </summary>
        public AnalyzedContent Content { get; set; } = new ();

        /// <summary>
        /// Gets or sets the list of action items extracted from the meeting
        /// </summary>
        public List<ActionItem> ActionItems { get; set; } = new ();

        /// <summary>
        /// Gets or sets the detected language of the meeting
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the complete transcribed text without any analysis or structuring
        /// </summary>
        public string TranscribedText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether the analysis was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets error message if the analysis failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
