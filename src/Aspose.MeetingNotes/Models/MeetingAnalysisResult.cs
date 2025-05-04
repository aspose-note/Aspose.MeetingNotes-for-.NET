namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents the complete result of processing and analyzing a meeting recording.
/// Includes the analyzed content, extracted action items, status, and detected language.
/// </summary>
public record MeetingAnalysisResult
{
    /// <summary>
    /// Gets the analyzed content of the meeting, including summary, sections, etc.
    /// </summary>
    /// <value>Defaults to a new instance of <see cref="AnalyzedContent"/>.</value>
    public AnalyzedContent Content { get; init; } = new ();

    /// <summary>
    /// Gets the list of action items extracted from the meeting.
    /// </summary>
    /// <value>Defaults to an empty list.</value>
    public List<ActionItem> ActionItems { get; init; } = [];

    /// <summary>
    /// Gets the detected or specified language of the meeting audio (e.g., "en", "ru").
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the complete original transcribed text without analysis or structuring.
    /// Included for reference or alternative processing.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string TranscribedText { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the overall analysis process was successful.
    /// If false, check the <see cref="ErrorMessage"/> property.
    /// </summary>
    public bool Success { get; init; } = false;

    /// <summary>
    /// Gets the error message if the analysis process failed (<see cref="Success"/> is false).
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string ErrorMessage { get; init; } = string.Empty;
}
