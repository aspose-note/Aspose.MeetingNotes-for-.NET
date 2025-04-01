namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the result of exporting meeting content to a specific format
    /// </summary>
    public class ExportResult
    {
        /// <summary>
        /// The format to which the content was exported
        /// </summary>
        public ExportFormat Format { get; set; }

        /// <summary>
        /// Text content of the export (for text-based formats like Markdown or HTML)
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Binary content of the export (for binary formats like PDF or OneNote)
        /// </summary>
        public byte[]? Data { get; set; }
    }

    /// <summary>
    /// Supported export formats for meeting content
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        /// Microsoft OneNote format (.one)
        /// </summary>
        OneNote,

        /// <summary>
        /// Markdown format (.md)
        /// </summary>
        Markdown,

        /// <summary>
        /// Portable Document Format (.pdf)
        /// </summary>
        PDF,

        /// <summary>
        /// HyperText Markup Language (.html)
        /// </summary>
        HTML
    }
} 