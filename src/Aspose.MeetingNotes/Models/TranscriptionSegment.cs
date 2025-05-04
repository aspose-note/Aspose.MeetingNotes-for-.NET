namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents a segment of transcribed text, potentially associated with a specific speaker
/// and time range within the original audio.
/// </summary>
public record TranscriptionSegment
{
    /// <summary>
    /// Gets the identifier or name of the speaker attributed to this text segment.
    /// May be "Unknown" or a generic ID if speaker diarization is not performed or fails.
    /// </summary>
    /// <value>Defaults to "Unknown".</value>
    public string Speaker { get; init; } = "Unknown";

    /// <summary>
    /// Gets the transcribed text content of this segment.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Gets the start time of this segment within the original audio.
    /// </summary>
    /// <value>Defaults to <see cref="TimeSpan.Zero"/>.</value>
    public TimeSpan StartTime { get; init; } = TimeSpan.Zero;

    /// <summary>
    /// Gets the end time of this segment within the original audio.
    /// </summary>
    /// <value>Defaults to <see cref="TimeSpan.Zero"/>.</value>
    public TimeSpan EndTime { get; init; } = TimeSpan.Zero;
}
