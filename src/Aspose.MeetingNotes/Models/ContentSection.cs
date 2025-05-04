namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents a distinct section identified within the analyzed meeting content,
/// typically having a title and associated text.
/// </summary>
public record ContentSection
{
    /// <summary>
    /// Gets the title of the content section (e.g., "Introduction", "Topic Discussion").
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the text content belonging to this section.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Content { get; init; } = string.Empty;
}
