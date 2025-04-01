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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<ExportResult> ExportAsync(
            AnalyzedContent content,
            List<ActionItem> actionItems,
            ExportFormat format,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to OneNote format
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<byte[]> ExportToOneNoteAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to Markdown format
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> ExportToMarkdownAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to PDF format
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<byte[]> ExportToPdfAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports content to HTML format
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> ExportToHtmlAsync(
            AnalyzedContent content,
            CancellationToken cancellationToken = default);
    }
}
