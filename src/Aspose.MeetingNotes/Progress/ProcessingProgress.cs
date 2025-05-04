namespace Aspose.MeetingNotes.Progress;

/// <summary>
/// Represents the progress status of a meeting processing operation at a specific point in time.
/// Instances of this record are typically reported via an <see cref="System.IProgress{T}"/> implementation.
/// </summary>
public record ProcessingProgress
{
    /// <summary>
    /// Gets the current stage of processing.
    /// </summary>
    /// <value>Defaults to <see cref="ProcessingStage.Initial"/>.</value>
    public ProcessingStage Stage { get; init; } = ProcessingStage.Initial;

    /// <summary>
    /// Gets the overall progress percentage, typically ranging from 0 to 100.
    /// </summary>
    /// <value>Defaults to 0.</value>
    public int ProgressPercentage { get; init; } = 0;

    /// <summary>
    /// Gets a user-friendly message describing the current activity or status within the stage.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string StatusMessage { get; init; } = string.Empty;
}
