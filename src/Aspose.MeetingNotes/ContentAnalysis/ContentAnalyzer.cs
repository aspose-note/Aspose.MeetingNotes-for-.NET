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

            var content = new AnalyzedContent
            {
                Summary = aiResult.Summary,
                Sections = new List<ContentSection>(),
                KeyPoints = aiResult.KeyPoints,
                QASegments = ExtractQASegments(transcription.Segments),
                TranscribedText = fullText
            };

            return content;
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

        private List<QASegment> ExtractQASegments(List<TranscriptionSegment> segments)
        {
            var qaSegments = new List<QASegment>();
            var questionKeywords = new[] { "what", "how", "why", "when", "where", "who" };

            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                if (questionKeywords.Any(keyword => segment.Text.ToLower().Contains(keyword)))
                {
                    var qaSegment = new QASegment
                    {
                        Question = segment.Text,
                        Answer = i + 1 < segments.Count ? segments[i + 1].Text : "No answer provided"
                    };
                    qaSegments.Add(qaSegment);
                }
            }

            return qaSegments;
        }
    }
}
