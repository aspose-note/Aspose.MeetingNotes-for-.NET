namespace Aspose.MeetingNotes.Progress
{
    /// <summary>
    /// Represents progress information for long-running operations in the MeetingNotes SDK
    /// </summary>
    public class ProcessingProgress
    {
        /// <summary>
        /// Current processing stage of the operation
        /// </summary>
        public ProcessingStage Stage { get; set; }

        /// <summary>
        /// Progress percentage of the current stage (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Detailed status message describing the current operation
        /// </summary>
        public string? StatusMessage { get; set; }
    }

    /// <summary>
    /// Represents different stages of the meeting notes processing pipeline
    /// </summary>
    public enum ProcessingStage
    {
        /// <summary>
        /// Initial audio processing stage
        /// </summary>
        AudioProcessing,

        /// <summary>
        /// Speech-to-text transcription stage
        /// </summary>
        Transcription,

        /// <summary>
        /// Content analysis and structuring stage
        /// </summary>
        ContentAnalysis,

        /// <summary>
        /// Action item extraction stage
        /// </summary>
        ActionItemExtraction,

        /// <summary>
        /// Final export stage
        /// </summary>
        Export
    }
} 