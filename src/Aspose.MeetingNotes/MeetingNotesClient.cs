using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Progress;
using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aspose.MeetingNotes
{
    /// <summary>
    /// Main client class for processing meeting audio and generating structured notes.
    /// </summary>
    public class MeetingNotesClient
    {
        private readonly IAudioProcessor audioProcessor;
        private readonly ISpeechRecognizer speechRecognizer;
        private readonly IContentAnalyzer contentAnalyzer;
        private readonly IActionItemExtractor actionItemExtractor;
        private readonly IContentExporter contentExporter;
        private readonly MeetingNotesOptions options;
        private readonly ILogger<MeetingNotesClient> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingNotesClient"/> class.
        /// </summary>
        /// <param name="audioProcessor">Service for processing audio input.</param>
        /// <param name="speechRecognizer">Service for converting speech to text.</param>
        /// <param name="contentAnalyzer">Service for analyzing and structuring content.</param>
        /// <param name="actionItemExtractor">Service for extracting action items.</param>
        /// <param name="contentExporter">Service for exporting content to various formats.</param>
        /// <param name="options">Configuration options for the client.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        public MeetingNotesClient(
            IAudioProcessor audioProcessor,
            ISpeechRecognizer speechRecognizer,
            IContentAnalyzer contentAnalyzer,
            IActionItemExtractor actionItemExtractor,
            IContentExporter contentExporter,
            IOptions<MeetingNotesOptions> options,
            ILogger<MeetingNotesClient> logger)
        {
            this.audioProcessor = audioProcessor;
            this.speechRecognizer = speechRecognizer;
            this.contentAnalyzer = contentAnalyzer;
            this.actionItemExtractor = actionItemExtractor;
            this.contentExporter = contentExporter;
            this.options = options.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Process meeting audio file and generate structured notes
        /// </summary>
        /// <param name="audioStream">The audio stream to process.</param>
        /// <param name="fileExtension">The file extension of the audio file.</param>
        /// <param name="progress">Optional progress reporter for processing updates.</param>
        /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
        /// <returns>A <see cref="MeetingAnalysisResult"/> containing the processed meeting notes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when audioStream is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the audio format is unsupported.</exception>
        public async Task<MeetingAnalysisResult> ProcessMeetingAsync(
            Stream audioStream,
            string fileExtension,
            IProgress<ProcessingProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (audioStream == null)
            {
                throw new ArgumentNullException(nameof(audioStream));
            }

            try
            {
                logger.LogInformation("Starting meeting processing");

                // Validate audio format
                if (!audioProcessor.IsFormatSupported(fileExtension))
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
                    processedAudio = await audioProcessor.ProcessAsync(audioStream, cancellationToken);
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
                    transcription = await speechRecognizer.TranscribeAsync(processedAudio, options.Language, cancellationToken);
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
                    analyzedContent = await contentAnalyzer.AnalyzeAsync(transcription, cancellationToken);
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
                    actionItems = await actionItemExtractor.ExtractActionItemsAsync(analyzedContent, cancellationToken);
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
                logger.LogError(ex, "Error processing meeting");
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
        /// <param name="content">The analyzed content to export.</param>
        /// <param name="actionItems">The list of action items to include in the export.</param>
        /// <param name="format">The format to export the meeting notes to.</param>
        /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
        /// <returns>An <see cref="ExportResult"/> containing the result of the export operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when content or actionItems is null.</exception>
        public async Task<ExportResult> ExportAsync(
            AnalyzedContent content,
            List<ActionItem> actionItems,
            ExportFormat format,
            CancellationToken cancellationToken = default)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (actionItems == null)
            {
                throw new ArgumentNullException(nameof(actionItems));
            }

            try
            {
                logger.LogInformation($"Exporting meeting notes to {format}");

                return await contentExporter.ExportAsync(
                    content,
                    actionItems,
                    format,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error exporting meeting notes");
                throw;
            }
        }
    }
}
