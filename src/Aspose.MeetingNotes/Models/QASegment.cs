namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents a question and its corresponding answer identified during meeting analysis.
/// </summary>
public record QASegment
{
    /// <summary>
    /// Gets the text of the question asked.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Question { get; init; } = string.Empty;

    /// <summary>
    /// Gets the text of the answer provided to the question.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Answer { get; init; } = string.Empty;
}
