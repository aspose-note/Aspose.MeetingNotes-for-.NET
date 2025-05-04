namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Defines the supported export file formats for the generated meeting notes.
/// </summary>
public enum ExportFormat
{
    /// <summary>
    /// Microsoft OneNote format (.one). Generation typically requires Aspose.Note.
    /// </summary>
    OneNote,

    /// <summary>
    /// Markdown text format (.md). Plain text with simple formatting syntax.
    /// </summary>
    Markdown,

    /// <summary>
    /// Portable Document Format (.pdf). Generation requires a PDF library (e.g., Aspose.Pdf).
    /// </summary>
    PDF,

    /// <summary>
    /// HyperText Markup Language format (.html). Generation requires an HTML library or conversion.
    /// </summary>
    HTML
}
