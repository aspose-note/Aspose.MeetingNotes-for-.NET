using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.Exporters
{
    /// <summary>
    /// Interface for exporting meeting content to various formats
    /// </summary>
    public interface IContentExporter
    {
        /// <summary>
        /// Export content to OneNote format
        /// </summary>
        Task<byte[]> ExportToOneNoteAsync(AnalyzedContent content, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Export content to Markdown format
        /// </summary>
        Task<string> ExportToMarkdownAsync(AnalyzedContent content, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Export content to PDF format
        /// </summary>
        Task<byte[]> ExportToPdfAsync(AnalyzedContent content, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Export content to HTML format
        /// </summary>
        Task<string> ExportToHtmlAsync(AnalyzedContent content, CancellationToken cancellationToken = default);
    }
} 