using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AIIntegration
{
    /// <summary>
    /// Interface for AI model integration
    /// </summary>
    public interface IAIModel
    {
        /// <summary>
        /// Generate summary and analysis of the transcribed text
        /// </summary>
        Task<AIAnalysisResult> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Extract action items from the text
        /// </summary>
        Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default);
    }
} 