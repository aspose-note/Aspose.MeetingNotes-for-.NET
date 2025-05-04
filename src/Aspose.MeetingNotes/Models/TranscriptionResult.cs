namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents the result of a speech-to-text transcription operation,
/// including segmented text, detected language, and status.
/// </summary>
public record TranscriptionResult
{
    /// <summary>
    /// Gets the list of transcribed text segments, potentially including speaker and timing information.
    /// </summary>
    /// <value>Defaults to an empty list.</value>
    public List<TranscriptionSegment> Segments { get; init; } = [];

    /// <summary>
    /// Gets the language code detected or used during transcription (e.g., "en", "ru").
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the transcription process completed successfully.
    /// If false, check the <see cref="ErrorMessage"/> property.
    /// </summary>
    public bool Success { get; init; } = false;

    /// <summary>
    /// Gets the error message if the transcription failed (<see cref="Success"/> is false).
    /// </summary>
    /// <value>Defaults to null.</value>
    public string? ErrorMessage { get; init; } = null;

    /// <summary>
    /// Gets the complete transcribed text concatenated from all segments, without detailed timing or speaker info.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string FullText { get; init; } = string.Empty;
}
