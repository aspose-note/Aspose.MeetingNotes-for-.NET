namespace Aspose.MeetingNotes.Progress
{
    /// <summary>
    /// Represents the current progress of a processing operation
    /// </summary>
    public class ProcessingProgress
    {
        /// <summary>
        /// The current stage of processing
        /// </summary>
        public ProcessingStage Stage { get; set; }

        /// <summary>
        /// The progress percentage (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// A descriptive message about the current stage
        /// </summary>
        public string StatusMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the different stages of processing a meeting recording
    /// </summary>
    public enum ProcessingStage
    {
        /// <summary>
        /// Initial stage
        /// </summary>
        Initial,

        /// <summary>
        /// Processing the audio file
        /// </summary>
        AudioProcessing,

        /// <summary>
        /// Converting speech to text
        /// </summary>
        Transcription,

        /// <summary>
        /// Analyzing the content
        /// </summary>
        ContentAnalysis,

        /// <summary>
        /// Extracting action items
        /// </summary>
        ActionItems,

        /// <summary>
        /// Exporting the results
        /// </summary>
        Export,

        /// <summary>
        /// Processing completed
        /// </summary>
        Complete
    }
} 