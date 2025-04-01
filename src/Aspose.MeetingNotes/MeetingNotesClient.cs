using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.SpeechRecognition;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Monitoring;
using Aspose.MeetingNotes.Progress;
using Aspose.MeetingNotes.Exceptions;

namespace Aspose.MeetingNotes
{
    /// <summary>
    /// Main entry point for the MeetingNotes SDK that orchestrates the processing of meeting recordings
    /// </summary>
    public class MeetingNotesClient
    {
        private readonly IAudioProcessor _audioProcessor;
        private readonly ISpeechRecognizer _speechRecognizer;
        private readonly IContentAnalyzer _contentAnalyzer;
        private readonly IActionItemExtractor _actionItemExtractor;
        private readonly IContentExporter _contentExporter;
        private readonly MeetingNotesOptions _options;
        private readonly ILogger<MeetingNotesClient> _logger;
        private readonly PerformanceMetrics _metrics;

        /// <summary>
        /// Initializes a new instance of the MeetingNotesClient class
        /// </summary>
        /// <param name="audioProcessor">Service for processing audio input</param>
        /// <param name="speechRecognizer">Service for converting speech to text</param>
        /// <param name="contentAnalyzer">Service for analyzing and structuring content</param>
        /// <param name="actionItemExtractor">Service for extracting action items</param>
        /// <param name="contentExporter">Service for exporting content to various formats</param>
        /// <param name="options">Configuration options for the client</param>
        /// <param name="logger">Logger instance for logging operations</param>
        /// <param name="metrics">Service for tracking performance metrics</param>
        public MeetingNotesClient(
            IAudioProcessor audioProcessor,
            ISpeechRecognizer speechRecognizer,
            IContentAnalyzer contentAnalyzer,
            IActionItemExtractor actionItemExtractor,
            IContentExporter contentExporter,
            IOptions<MeetingNotesOptions> options,
            ILogger<MeetingNotesClient> logger,
            PerformanceMetrics metrics)
        {
            _audioProcessor = audioProcessor;
            _speechRecognizer = speechRecognizer;
            _contentAnalyzer = contentAnalyzer;
            _actionItemExtractor = actionItemExtractor;
            _contentExporter = contentExporter;
            _options = options.Value;
            _logger = logger;
            _metrics = metrics;
        }

        /// <summary>
        /// Process meeting audio file and generate structured notes
        /// </summary>
        public async Task<MeetingAnalysisResult> ProcessMeetingAsync(
            Stream audioStream,
            string fileExtension,
            IProgress<ProcessingProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            using var _ = _metrics.TrackOperation("ProcessMeeting");

            try
            {
                _logger.LogInformation("Starting meeting processing");

                // Validate audio format
                if (!_audioProcessor.IsFormatSupported(fileExtension))
                {
                    throw new ArgumentException($"Unsupported audio format: {fileExtension}");
                }

                // Process audio
                progress?.Report(new ProcessingProgress 
                { 
                    Stage = ProcessingStage.AudioProcessing,
                    ProgressPercentage = 0,
                    StatusMessage = "Processing audio file..."
                });

                var processedAudio = await _audioProcessor.ProcessAsync(audioStream, cancellationToken);

                // Transcribe audio
                var transcription = await _speechRecognizer.TranscribeAsync(
                    processedAudio,
                    _options.Language,
                    cancellationToken);

                if (!transcription.Success)
                {
                    throw new Exception($"Transcription failed: {transcription.ErrorMessage}");
                }

                // Analyze content
                var analyzedContent = await _contentAnalyzer.AnalyzeAsync(transcription, cancellationToken);

                // Extract action items
                var actionItems = await _actionItemExtractor.ExtractActionItemsAsync(analyzedContent, cancellationToken);

                return new MeetingAnalysisResult
                {
                    Content = analyzedContent,
                    ActionItems = actionItems,
                    Transcription = transcription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing meeting");
                throw new MeetingNotesException("Failed to process meeting", ex);
            }
        }

        /// <summary>
        /// Export meeting notes to the specified format
        /// </summary>
        public async Task<ExportResult> ExportAsync(
            MeetingAnalysisResult analysis,
            ExportFormat format,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"Exporting meeting notes to {format}");

                var result = new ExportResult { Format = format };

                switch (format)
                {
                    case ExportFormat.OneNote:
                        result.Data = await _contentExporter.ExportToOneNoteAsync(analysis.Content, cancellationToken);
                        break;
                    case ExportFormat.Markdown:
                        result.Text = await _contentExporter.ExportToMarkdownAsync(analysis.Content, cancellationToken);
                        break;
                    case ExportFormat.PDF:
                        result.Data = await _contentExporter.ExportToPdfAsync(analysis.Content, cancellationToken);
                        break;
                    case ExportFormat.HTML:
                        result.Text = await _contentExporter.ExportToHtmlAsync(analysis.Content, cancellationToken);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported export format: {format}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting meeting notes");
                throw;
            }
        }
    }
} 