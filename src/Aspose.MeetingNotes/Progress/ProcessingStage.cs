namespace Aspose.MeetingNotes.Progress;

/// <summary>
/// Defines the distinct stages involved in processing a meeting recording.
/// Used to report progress status via <see cref="ProcessingProgress"/>.
/// </summary>
public enum ProcessingStage
{
    /// <summary>
    /// The initial state before processing begins.
    /// </summary>
    Initial,

    /// <summary>
    /// The stage where the input audio file is being processed or converted (e.g., to WAV).
    /// </summary>
    AudioProcessing,

    /// <summary>
    /// The stage where the processed audio is being transcribed into text.
    /// </summary>
    Transcription,

    /// <summary>
    /// The stage where the transcribed text is being analyzed by an AI model
    /// to extract summaries, key points, etc.
    /// </summary>
    ContentAnalysis,

    /// <summary>
    /// The stage where action items are being extracted from the analyzed content.
    /// </summary>
    ActionItems,

    /// <summary>
    /// The stage where the final results (notes, action items) are being exported
    /// to a specific format.
    /// </summary>
    Export,

    /// <summary>
    /// The final state indicating that all processing stages have completed successfully.
    /// </summary>
    Complete
}
