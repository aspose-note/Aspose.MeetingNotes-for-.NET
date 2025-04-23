using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AIIntegration
{
    /// <summary>
    /// Interface for AI models that can analyze meeting content
    /// </summary>
    public interface IAIModel
    {
        /// <summary>
        /// Analyzes the provided text content and returns structured analysis results
        /// </summary>
        /// <param name="text">The text content to analyze</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>An AnalyzedContent containing the analysis results</returns>
        Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extracts action items from the provided text using the AI model.
        /// </summary>
        /// <param name="text">The text content to extract action items from.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the list of extracted action items.</returns>
        Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default);
    }
}