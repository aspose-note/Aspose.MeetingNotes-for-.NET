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
        Task<AnalyzedContent> AnalyzeAsync(TranscriptionResult transcription, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Generate a meeting summary
        /// </summary>
        Task<string> GenerateSummaryAsync(AnalyzedContent content, CancellationToken cancellationToken = default);
    }
} 