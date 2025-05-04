namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents the structured results of analyzing meeting content,
/// including summaries, key points, decisions, topics, Q&A, and section details.
/// </summary>
public record AnalyzedContent
{
    /// <summary>
    /// Gets a brief summary of the meeting content.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Gets the structured sections identified within the meeting content.
    /// </summary>
    /// <value>Defaults to an empty list.</value>
    public List<ContentSection> Sections { get; init; } = [];

    /// <summary>
    /// Gets the key discussion points extracted from the meeting.
    /// </summary>
    /// <value>Defaults to an empty list.</value>
    public List<string> KeyPoints { get; init; } = [];

    /// <summary>
    /// Gets the main decisions recorded during the meeting.
    /// </summary>
    /// <value>Defaults to an empty list.</value>
    public List<string> Decisions { get; init; } = [];

    /// <summary>
    /// Gets the primary topics discussed during the meeting.
    /// </summary>
    /// <value>Defaults to an empty list.</value>
    public List<string> Topics { get; init; } = [];

    /// <summary>
    /// Gets the questions and corresponding answers identified in the meeting.
    /// </summary>
    /// <value>Defaults to an empty list.</value>
    public List<QASegment> QASegments { get; init; } = [];

    /// <summary>
    /// Gets the complete original transcribed text that was analyzed.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string TranscribedText { get; init; } = string.Empty;
}
