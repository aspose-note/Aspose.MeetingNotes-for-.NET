namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents the result of an export operation, containing the exported data
/// either as text or a byte array, along with the format.
/// </summary>
public record ExportResult
{
    /// <summary>
    /// Gets the format to which the content was exported.
    /// </summary>
    public ExportFormat Format { get; init; }

    /// <summary>
    /// Gets the exported content as a string, used for text-based formats like Markdown or HTML.
    /// Will be null if the export format is binary (<see cref="Data"/> will be populated instead).
    /// </summary>
    public string? Text { get; init; } = null;

    /// <summary>
    /// Gets the exported content as a byte array, used for binary formats like PDF or OneNote.
    /// Will be null if the export format is text-based (<see cref="Text"/> will be populated instead).
    /// </summary>
    public byte[]? Data { get; init; } = null;
}
