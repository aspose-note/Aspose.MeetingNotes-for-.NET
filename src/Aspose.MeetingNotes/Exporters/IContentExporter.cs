using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.Exporters;

/// <summary>
/// Defines the contract for services that export analyzed meeting content and action items
/// into various output formats.
/// </summary>
public interface IContentExporter
{
    /// <summary>
    /// Asynchronously exports the provided meeting content and action items to the specified format.
    /// This method typically delegates to format-specific export methods.
    /// </summary>
    /// <param name="content">The analyzed meeting content to export.</param>
    /// <param name="actionItems">The list of action items extracted from the meeting.</param>
    /// <param name="format">The target export format.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains an <see cref="ExportResult"/> object with the exported data
    /// (either as text or byte array depending on the format).
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="content"/> or <paramref name="actionItems"/> is null.</exception>
    /// <exception cref="System.ArgumentException">Thrown if the specified <paramref name="format"/> is invalid or unsupported.</exception>
    /// <exception cref="System.NotImplementedException">Thrown if the export logic for the specified <paramref name="format"/> is not yet implemented.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    /// <exception cref="System.Exception">Other exceptions may be thrown depending on the specific export implementation.</exception>
    Task<ExportResult> ExportAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        ExportFormat format,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously exports the analyzed content to Microsoft OneNote format (.one).
    /// </summary>
    /// <param name="content">The analyzed meeting content.</param>
    /// <param name="actionItems">The list of action items.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a byte array representing the OneNote file content.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="content"/> or <paramref name="actionItems"/> is null.</exception>
    /// <exception cref="System.NotImplementedException">Thrown if OneNote export is not implemented.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled.</exception>
    /// <exception cref="System.Exception">Exceptions related to OneNote document generation may be thrown.</exception>
    Task<byte[]> ExportToOneNoteAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously exports the analyzed content to Markdown format (.md).
    /// </summary>
    /// <param name="content">The analyzed meeting content.</param>
    /// <param name="actionItems">The list of action items.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a string with the meeting notes formatted as Markdown.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="content"/> or <paramref name="actionItems"/> is null.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task<string> ExportToMarkdownAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously exports the analyzed content to Portable Document Format (.pdf).
    /// </summary>
    /// <param name="content">The analyzed meeting content.</param>
    /// <param name="actionItems">The list of action items.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a byte array representing the PDF file content.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="content"/> or <paramref name="actionItems"/> is null.</exception>
    /// <exception cref="System.NotImplementedException">Thrown if PDF export is not implemented.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled.</exception>
    /// <exception cref="System.Exception">Exceptions related to PDF document generation may be thrown.</exception>
    Task<byte[]> ExportToPdfAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously exports the analyzed content to HTML format (.html).
    /// </summary>
    /// <param name="content">The analyzed meeting content.</param>
    /// <param name="actionItems">The list of action items.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a string with the meeting notes formatted as HTML.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="content"/> or <paramref name="actionItems"/> is null.</exception>
    /// <exception cref="System.NotImplementedException">Thrown if HTML export is not implemented.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled.</exception>
    /// <exception cref="System.Exception">Exceptions related to HTML generation may be thrown.</exception>
    Task<string> ExportToHtmlAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default);
}
