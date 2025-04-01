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
        private readonly IAIModel _aiModel;
        private readonly ILogger<ContentAnalyzer> _logger;

        public ContentAnalyzer(IAIModel aiModel, ILogger<ContentAnalyzer> logger)
        {
            _aiModel = aiModel;
            _logger = logger;
        }

        public async Task<AnalyzedContent> AnalyzeAsync(TranscriptionResult transcription, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting content analysis");

            var fullText = string.Join("\n", transcription.Segments.Select(s => s.Text));
            var aiResult = await _aiModel.AnalyzeContentAsync(fullText, cancellationToken);

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