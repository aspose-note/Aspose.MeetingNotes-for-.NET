using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.ContentAnalysis
{
    /// <summary>
    /// Implementation of content analysis operations
    /// </summary>
    public class ContentAnalyzer : IContentAnalyzer
    {
        private readonly IAIModel aiModel;
        private readonly ILogger<ContentAnalyzer> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAnalyzer"/> class.
        /// </summary>
        /// <param name="aiModel">The AI model to use for content analysis.</param>
        /// <param name="logger">The logger instance for logging analysis operations.</param>
        public ContentAnalyzer(IAIModel aiModel, ILogger<ContentAnalyzer> logger)
        {
            this.aiModel = aiModel;
            this.logger = logger;
        }

        /// <summary>
        /// Analyzes the transcribed content using the configured AI model.
        /// </summary>
        /// <param name="transcription">The transcription result to analyze.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the analyzed content.</returns>
        public async Task<AnalyzedContent> AnalyzeAsync(TranscriptionResult transcription, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Starting content analysis");

            var fullText = string.Join("\n", transcription.Segments.Select(s => s.Text));
            var aiResult = await aiModel.AnalyzeContentAsync(fullText, cancellationToken);

            logger.LogInformation("Content analysis completed successfully");
            return aiResult;
        }

        /// <summary>
        /// Generates a concise summary of the analyzed content.
        /// </summary>
        /// <param name="content">The analyzed content to summarize.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the generated summary.</returns>
        public async Task<string> GenerateSummaryAsync(AnalyzedContent content, CancellationToken cancellationToken = default)
        {
            // Generate a concise summary (max 200 words)
            return content.Summary;
        }
    }
}
