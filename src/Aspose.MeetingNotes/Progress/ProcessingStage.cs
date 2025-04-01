namespace Aspose.MeetingNotes.Progress
{
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
