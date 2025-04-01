using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.ContentAnalysis
{
    /// <summary>
    /// Interface for content analysis operations
    /// </summary>
    public interface IContentAnalyzer
    {
        /// <summary>
        /// Analyze transcribed content and structure it into sections
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<AnalyzedContent> AnalyzeAsync(TranscriptionResult transcription, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generate a meeting summary
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GenerateSummaryAsync(AnalyzedContent content, CancellationToken cancellationToken = default);
    }
}
