namespace Aspose.MeetingNotes.Progress
{
    /// <summary>
    /// Represents the current progress of a processing operation
    /// </summary>
    public class ProcessingProgress
    {
        /// <summary>
        /// Gets or sets the current stage of processing
        /// </summary>
        public ProcessingStage Stage { get; set; }

        /// <summary>
        /// Gets or sets the progress percentage (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Gets or sets a descriptive message about the current stage
        /// </summary>
        public string StatusMessage { get; set; } = string.Empty;
    }
}
