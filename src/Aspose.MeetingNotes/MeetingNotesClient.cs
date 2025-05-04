using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Progress;
using Aspose.MeetingNotes.SpeechRecognition;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aspose.MeetingNotes;

/// <summary>
/// Main client class for processing meeting audio and generating structured notes.
/// Orchestrates the workflow involving audio processing, speech recognition,
/// content analysis, action item extraction, and exporting.
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
    /// <exception cref="ArgumentNullException">Thrown if any injected dependency is null.</exception>
    public MeetingNotesClient(
        IAudioProcessor audioProcessor,
        ISpeechRecognizer speechRecognizer,
        IContentAnalyzer contentAnalyzer,
        IActionItemExtractor actionItemExtractor,
        IContentExporter contentExporter,
        IOptions<MeetingNotesOptions> options,
        ILogger<MeetingNotesClient> logger)
    {
        ArgumentNullException.ThrowIfNull(audioProcessor);
        ArgumentNullException.ThrowIfNull(speechRecognizer);
        ArgumentNullException.ThrowIfNull(contentAnalyzer);
        ArgumentNullException.ThrowIfNull(actionItemExtractor);
        ArgumentNullException.ThrowIfNull(contentExporter);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        this.audioProcessor = audioProcessor;
        this.speechRecognizer = speechRecognizer;
        this.contentAnalyzer = contentAnalyzer;
        this.actionItemExtractor = actionItemExtractor;
        this.contentExporter = contentExporter;
        this.options = options.Value;
        this.logger = logger;
    }

    /// <summary>
    /// Processes a meeting audio file asynchronously, generating structured notes and action items.
    /// </summary>
    /// <param name="audioFileInfo">The audio file information to process.</param>
    /// <param name="progress">Optional progress reporter for processing updates.</param>
    /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
    /// <returns>A <see cref="Task{MeetingAnalysisResult}"/> containing the processed meeting notes or error information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="audioFileInfo"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the audio format is unsupported or other argument issues arise.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the audio file specified by <paramref name="audioFileInfo"/> cannot be found.</exception>
    /// <exception cref="AudioProcessingException">Thrown if audio conversion fails.</exception>
    /// <exception cref="TranscriptionException">Thrown if speech-to-text transcription fails.</exception>
    /// <exception cref="AIModelException">Thrown if content analysis or action item extraction fails.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the cancellation token.</exception>
    public async Task<MeetingAnalysisResult> ProcessMeetingAsync(
        FileInfo audioFileInfo,
        IProgress<ProcessingProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioFileInfo);

        if (!audioFileInfo.Exists)
        {
            this.logger.LogError("Audio file not found: {FilePath}", audioFileInfo.FullName);
            throw new FileNotFoundException("Audio file not found.", audioFileInfo.FullName);
        }

        try
        {
            this.logger.LogInformation("Starting meeting processing for file: {FileName}", audioFileInfo.Name);

            // 1. Validate audio format
            if (!this.audioProcessor.IsFormatSupported(audioFileInfo.Extension))
            {
                this.logger.LogError("Unsupported audio format: {Extension}", audioFileInfo.Extension);
                throw new ArgumentException($"Unsupported audio format: {audioFileInfo.Extension}", nameof(audioFileInfo));
            }
            this.logger.LogDebug("Audio format {Extension} supported", audioFileInfo.Extension);

            // 2. Process audio (Convert to WAV)
            this.ReportProgress(progress, ProcessingStage.AudioProcessing, 0, "Processing audio file...");
            Stream audioStream;
            try
            {
                audioStream = await this.audioProcessor.ConvertToWavAsync(audioFileInfo, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                this.logger.LogInformation("Audio processing completed");
            }
            catch (Exception ex) when (ex is not OperationCanceledException and not AudioProcessingException and not FileNotFoundException) // Avoid re-wrapping specific exceptions
            {
                this.logger.LogError(ex, "Audio processing failed unexpectedly for {FileName}", audioFileInfo.Name);
                throw new AudioProcessingException($"Failed to process audio: {ex.Message}", ex);
            }

            TranscriptionResult transcription;
            AnalyzedContent analyzedContent;
            List<ActionItem> actionItems;

            // Use await using for the stream to ensure disposal
            await using (audioStream)
            {
                // 3. Transcribe audio
                this.ReportProgress(progress, ProcessingStage.Transcription, 25, "Converting speech to text...");
                try
                {
                    transcription = await this.speechRecognizer.TranscribeAsync(audioStream, this.options.Language, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!transcription.Success)
                    {
                        this.logger.LogError("Transcription failed: {ErrorMessage}", transcription.ErrorMessage);
                        throw new TranscriptionException($"Transcription failed: {transcription.ErrorMessage}");
                    }

                    this.logger.LogInformation("Transcription completed. Language: {Language}", transcription.Language);
                }
                catch (Exception ex) when (ex is not OperationCanceledException and not TranscriptionException)
                {
                    this.logger.LogError(ex, "Transcription failed unexpectedly");
                    throw new TranscriptionException($"Transcription failed: {ex.Message}", ex);
                }

                // 4. Analyze content
                this.ReportProgress(progress, ProcessingStage.ContentAnalysis, 50, "Analyzing content...");
                try
                {
                    analyzedContent = await this.contentAnalyzer.AnalyzeAsync(transcription, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    this.logger.LogInformation("Content analysis completed");
                }
                catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
                {
                    this.logger.LogError(ex, "Content analysis failed unexpectedly");
                    throw new AIModelException($"Content analysis failed: {ex.Message}", ex);
                }

                // 5. Extract action items
                this.ReportProgress(progress, ProcessingStage.ActionItems, 75, "Extracting action items...");
                try
                {
                    actionItems = await this.actionItemExtractor.ExtractActionItemsAsync(analyzedContent, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    this.logger.LogInformation("Action item extraction completed. Found {Count} items", actionItems.Count);
                }
                catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
                {
                    this.logger.LogError(ex, "Action item extraction failed unexpectedly");
                    throw new AIModelException($"Action item extraction failed: {ex.Message}", ex);
                }
            }

            this.ReportProgress(progress, ProcessingStage.Complete, 100, "Processing completed successfully");

            // 6. Return successful result
            return new MeetingAnalysisResult {
                Content = analyzedContent,
                ActionItems = actionItems,
                Language = transcription.Language,
                TranscribedText = transcription.FullText,
                Success = true
            };
        }
        catch (OperationCanceledException)
        {
            this.logger.LogWarning("Meeting processing was cancelled for file: {FileName}", audioFileInfo.Name);
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Meeting processing failed for file {FileName} due to: {ExceptionType}", audioFileInfo.Name, ex.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Exports the analyzed meeting content and action items to the specified format asynchronously.
    /// </summary>
    /// <param name="content">The analyzed content to export.</param>
    /// <param name="actionItems">The list of action items to include in the export.</param>
    /// <param name="format">The desired export format.</param>
    /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
    /// <returns>An <see cref="ExportResult"/> containing the exported content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> or <paramref name="actionItems"/> is null.</exception>
    /// <exception cref="NotSupportedException">Thrown if the export format is not supported or implemented by the exporter.</exception>
    /// <exception cref="Exception">Rethrows exceptions that occur during the export process.</exception>
    public async Task<ExportResult> ExportAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        ExportFormat format,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(actionItems);

        try
        {
            this.logger.LogInformation("Exporting meeting notes to {Format}", format);
            ExportResult exportResult = await this.contentExporter.ExportAsync(
                content,
                actionItems,
                format,
                cancellationToken);

            this.logger.LogInformation("Export to {Format} completed successfully", format);
            return exportResult;
        }
        catch (NotImplementedException ex)
        {
            this.logger.LogError(ex, "Export format {Format} is not implemented", format);
            throw new NotSupportedException($"Export format {format} is not currently implemented", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            this.logger.LogError(ex, "Error exporting meeting notes to {Format}", format);
            throw;
        }
    }

    // Helper method for progress reporting (private, no 'this.' needed for static context, but instance access requires 'this.')
    private void ReportProgress(IProgress<ProcessingProgress>? progress, ProcessingStage stage, int percentage, string message)
    {
        progress?.Report(new ProcessingProgress
        {
            Stage = stage,
            ProgressPercentage = percentage,
            StatusMessage = message
        });

        this.logger.LogDebug("Progress: {Stage} - {Percentage}% - {Message}", stage, percentage, message);
    }
}
