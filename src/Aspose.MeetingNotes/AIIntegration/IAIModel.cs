using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AIIntegration
{
    /// <summary>
    /// Defines the interface for AI model integration.
    /// </summary>
    public interface IAIModel
    {
        /// <summary>
        /// Analyzes the provided text content using the AI model.
        /// </summary>
        /// <param name="text">The text content to analyze.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the AI analysis result.</returns>
        Task<AIAnalysisResult> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extracts action items from the provided text using the AI model.
        /// </summary>
        /// <param name="text">The text content to extract action items from.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the list of extracted action items.</returns>
        Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default);
    }
}