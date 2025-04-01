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
using Aspose.MeetingNotes.Metrics;

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
        private readonly IMetricsCollector _metrics;

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
            IMetricsCollector metrics)
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
                ProcessedAudio processedAudio;
                try
                {
                    processedAudio = await _audioProcessor.ProcessAsync(audioStream, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to process audio", ex);
                }

                // Transcribe audio
                progress?.Report(new ProcessingProgress 
                { 
                    Stage = ProcessingStage.Transcription,
                    ProgressPercentage = 25,
                    StatusMessage = "Converting speech to text..."
                });
                TranscriptionResult transcription;
                try
                {
                    transcription = await _speechRecognizer.TranscribeAsync(processedAudio, _options.Language, cancellationToken);
                    if (!transcription.Success)
                    {
                        throw new Exception($"Transcription failed: {transcription.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Transcription failed", ex);
                }
                finally
                {
                    processedAudio.Dispose();
                }

                // Analyze content
                progress?.Report(new ProcessingProgress 
                { 
                    Stage = ProcessingStage.ContentAnalysis,
                    ProgressPercentage = 50,
                    StatusMessage = "Analyzing content..."
                });
                AnalyzedContent analyzedContent;
                try
                {
                    analyzedContent = await _contentAnalyzer.AnalyzeAsync(transcription, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Content analysis failed", ex);
                }

                // Extract action items
                progress?.Report(new ProcessingProgress 
                { 
                    Stage = ProcessingStage.ActionItems,
                    ProgressPercentage = 75,
                    StatusMessage = "Extracting action items..."
                });
                List<ActionItem> actionItems;
                try
                {
                    actionItems = await _actionItemExtractor.ExtractActionItemsAsync(analyzedContent, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Action item extraction failed", ex);
                }

                progress?.Report(new ProcessingProgress 
                { 
                    Stage = ProcessingStage.Complete,
                    ProgressPercentage = 100,
                    StatusMessage = "Processing completed successfully"
                });

                return new MeetingAnalysisResult
                {
                    Content = analyzedContent,
                    ActionItems = actionItems,
                    Language = transcription.Language,
                    TranscribedText = transcription.FullText,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing meeting");
                return new MeetingAnalysisResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export meeting notes to the specified format
        /// </summary>
        public async Task<ExportResult> ExportAsync(
            AnalyzedContent content,
            List<ActionItem> actionItems,
            ExportFormat format,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"Exporting meeting notes to {format}");

                return await _contentExporter.ExportAsync(
                    content,
                    actionItems,
                    format,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting meeting notes");
                throw;
            }
        }
    }
} 