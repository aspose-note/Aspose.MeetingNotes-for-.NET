using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AIIntegration;

/// <summary>
/// Defines the contract for AI models capable of analyzing meeting content
/// and extracting relevant information like summaries and action items.
/// </summary>
public interface IAIModel
{
    /// <summary>
    /// Analyzes the provided text content using the AI model.
    /// </summary>
    /// <param name="text">The text content (e.g., meeting transcript) to analyze.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains the <see cref="AnalyzedContent"/> with structured analysis results
    /// (summary, key points, topics, decisions, Q&A).
    /// </returns>
    /// <exception cref="System.ArgumentException">Thrown if <paramref name="text"/> is null or empty.</exception>
    /// <exception cref="Aspose.MeetingNotes.Exceptions.AIModelException">Thrown if the AI model fails to process the request or returns an invalid response.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts action items from the provided text using the AI model.
    /// </summary>
    /// <param name="text">The text content (e.g., meeting transcript) to extract action items from.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a list (<see cref="List{ActionItem}"/>) of extracted action items.
    /// Returns an empty list if no action items are found or extracted.
    /// </returns>
    /// <exception cref="System.ArgumentException">Thrown if <paramref name="text"/> is null or empty.</exception>
    /// <exception cref="Aspose.MeetingNotes.Exceptions.AIModelException">Thrown if the AI model fails to process the request or returns an invalid response.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default);
}
