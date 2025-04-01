using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.Exporters
{
    /// <summary>
    /// Interface for exporting content to various formats
    /// </summary>
    public interface IContentExporter
    {
        /// <summary>
        /// Exports content to the specified format
        /// </summary>
        Task<ExportResult> ExportAsync(
            AnalyzedContent content,
            List<ActionItem> actionItems,
            ExportFormat format,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to OneNote format
        /// </summary>
        Task<byte[]> ExportToOneNoteAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to Markdown format
        /// </summary>
        Task<string> ExportToMarkdownAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to PDF format
        /// </summary>
        Task<byte[]> ExportToPdfAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to HTML format
        /// </summary>
        Task<string> ExportToHtmlAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);
    }
} 