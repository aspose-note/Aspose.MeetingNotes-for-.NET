namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the result of exporting meeting content to a specific format
    /// </summary>
    public class ExportResult
    {
        /// <summary>
        /// Gets or sets the format to which the content was exported
        /// </summary>
        public ExportFormat Format { get; set; }

        /// <summary>
        /// Gets or sets text content of the export (for text-based formats like Markdown or HTML)
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets binary content of the export (for binary formats like PDF or OneNote)
        /// </summary>
        public byte[]? Data { get; set; }
    }
}
