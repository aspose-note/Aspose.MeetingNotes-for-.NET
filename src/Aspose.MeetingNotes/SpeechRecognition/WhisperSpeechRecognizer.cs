using System.Text;

using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Whisper.net;
using Whisper.net.Ggml;

namespace Aspose.MeetingNotes.SpeechRecognition;

/// <summary>
/// Implements the <see cref="ISpeechRecognizer"/> interface using the Whisper.NET library
/// for performing speech-to-text transcription. Handles model downloading and management.
/// </summary>
public sealed class WhisperSpeechRecognizer : ISpeechRecognizer, IDisposable // Sealed class, implements IDisposable
{
    private readonly ILogger<WhisperSpeechRecognizer> logger;
    private readonly MeetingNotesOptions options;
    private readonly SemaphoreSlim lockObj = new (1, 1);
    private readonly string modelDirectory;

    private WhisperFactory? whisperFactory;
    private string? resolvedModelPath;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhisperSpeechRecognizer"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The MeetingNotes options containing speech recognition settings.</param>
    /// <exception cref="ArgumentNullException">Thrown if logger or options is null.</exception>
    public WhisperSpeechRecognizer(
        ILogger<WhisperSpeechRecognizer> logger,
        IOptions<MeetingNotesOptions> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        this.logger = logger;
        this.options = options.Value;

        // Determine model directory (e.g., relative to base directory or configurable)
        this.modelDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");
        this.logger.LogDebug("Whisper model directory set to: {ModelDirectory}", this.modelDirectory);
    }

    /// <inheritdoc/>
    public async Task<TranscriptionResult> TranscribeAsync(
        Stream audioStream,
        string language,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioStream);
        if (!audioStream.CanRead)
        {
            throw new ArgumentException("Input audio stream must be readable", nameof(audioStream));
        }

        this.logger.LogInformation("Starting transcription. Language: {Language}", language);

        try
        {
            // 1. Ensure model exists locally (downloads if necessary)
            await this.EnsureModelExistsAsync(cancellationToken);
            if (this.resolvedModelPath == null)
            {
                throw new TranscriptionException("Could not resolve or obtain the Whisper model file path.");
            }

            // 2. Initialize WhisperFactory if needed (thread-safe)
            await this.InitializeWhisperFactoryAsync(cancellationToken);
            if (this.whisperFactory == null)
            {
                throw new TranscriptionException("Failed to initialize Whisper factory after ensuring model exists.");
            }

            // 3. Create processor and transcribe
            this.logger.LogDebug("Creating Whisper processor for language: {Language}", language);
            using (var processor = this.whisperFactory.CreateBuilder()
                .WithLanguage(language)
                .Build())
            {
                this.logger.LogInformation("Processing audio stream with Whisper...");
                var segments = new List<TranscriptionSegment>();
                var fullTextBuilder = new StringBuilder();

                await foreach (var segmentData in processor.ProcessAsync(audioStream, cancellationToken))
                {
                    string trimmedText = segmentData.Text.Trim();
                    if (fullTextBuilder.Length > 0)
                    {
                        fullTextBuilder.Append(' ');
                    }
                    fullTextBuilder.Append(trimmedText);

                    segments.Add(new TranscriptionSegment
                    {
                        Speaker = "Unknown",
                        Text = trimmedText,
                        StartTime = segmentData.Start,
                        EndTime = segmentData.End
                    });
                    this.logger.LogTrace("Segment [{StartTime} -> {EndTime}]: {Text}", segmentData.Start, segmentData.End, trimmedText);
                }

                string finalFullText = fullTextBuilder.ToString();
                this.logger.LogInformation("Transcription processing complete. Found {SegmentCount} segments", segments.Count);

                return new TranscriptionResult {
                    Segments = segments,
                    Language = language,
                    Success = true,
                    FullText = finalFullText
                };
            }
        }
        catch (OperationCanceledException)
        {
            this.logger.LogWarning("Transcription was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred during transcription");
            throw new TranscriptionException($"Transcription failed: {ex.Message}", ex);
            // Alternatively:
            // return new TranscriptionResult
            // {
            //     Success = false,
            //     ErrorMessage = ex.Message,
            //     Language = language,
            //     Segments = [],
            //     FullText = string.Empty
            // };
        }
    }

    /// <summary>
    /// Ensures the required Whisper model file exists locally, downloading it if necessary.
    /// Sets the <see cref="resolvedModelPath"/> field upon success.
    /// </summary>
    /// <exception cref="TranscriptionException">Thrown if the model cannot be resolved or downloaded.</exception>
    private async Task EnsureModelExistsAsync(CancellationToken cancellationToken)
    {
        // If already resolved, skip
        if (this.resolvedModelPath != null && File.Exists(this.resolvedModelPath))
        {
            return;
        }

        // Determine path from options or construct default
        string modelFileName;
        string? specificPath = this.options.SpeechRecognition?.ModelPath;
        GgmlType modelType = GgmlType.Base;

        if (!string.IsNullOrWhiteSpace(specificPath))
        {
            this.logger.LogInformation("Using specific Whisper model path from options: {ModelPath}", specificPath);
            if (!File.Exists(specificPath))
            {
                this.logger.LogError("Specified Whisper model file not found at: {ModelPath}", specificPath);
                throw new FileNotFoundException("Specified Whisper model file not found.", specificPath);
            }

            this.resolvedModelPath = specificPath;
            return;
        }
        else
        {
            // Construct filename based on ModelSize
            string modelSize = this.options.SpeechRecognition?.ModelSize?.ToLowerInvariant() ?? "base";
            if (!Enum.TryParse<GgmlType>(modelSize, true, out modelType))
            {
                this.logger.LogWarning("Invalid Whisper model size specified: {ModelSize}. Defaulting to 'Base'", modelSize);
                modelType = GgmlType.Base;
                modelSize = "base";
            }

            // Standard Whisper.NET naming convention
            modelFileName = $"ggml-{modelSize}.bin";
            // If targeting specific quantizations like q5_0 etc., adjust naming logic here
            // modelFileName = $"ggml-{modelSize}-q5_0.bin"; // Example

            string potentialPath = Path.Combine(this.modelDirectory, modelFileName);
            this.logger.LogInformation("Checking for Whisper model '{ModelFileName}' in directory {ModelDirectory}", modelFileName, this.modelDirectory);

            if (File.Exists(potentialPath))
            {
                this.logger.LogInformation("Found existing Whisper model file: {ModelPath}", potentialPath);
                this.resolvedModelPath = potentialPath;
                return;
            }
            else
            {
                this.logger.LogInformation("Whisper model '{ModelFileName}' not found locally, attempting download...", modelFileName);
                this.resolvedModelPath = await this.DownloadModelAsync(modelType, potentialPath, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Downloads the specified Whisper Ggml model type to the target path.
    /// </summary>
    /// <returns>The path where the model was saved.</returns>
    /// <exception cref="TranscriptionException">Thrown if download fails.</exception>
    private async Task<string> DownloadModelAsync(GgmlType modelType, string targetPath, CancellationToken cancellationToken)
    {
        try
        {
            // Ensure target directory exists
            string? directory = Path.GetDirectoryName(targetPath);
            if (directory != null && !Directory.Exists(directory))
            {
                this.logger.LogDebug("Creating model directory: {Directory}", directory);
                Directory.CreateDirectory(directory);
            }

            this.logger.LogInformation("Downloading Whisper model type {ModelType} to {TargetPath}...", modelType, targetPath);

            // Download model using WhisperGgmlDownloader
            await using (var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(modelType, cancellationToken: cancellationToken))
            {
                await using (var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await modelStream.CopyToAsync(fileStream, cancellationToken);
                }
            }

            this.logger.LogInformation("Whisper model downloaded successfully to {TargetPath}", targetPath);
            return targetPath;
        }
        catch (HttpRequestException httpEx)
        {
            this.logger.LogError(httpEx, "Failed to download Whisper model {ModelType} due to network error", modelType);
            throw new TranscriptionException($"Failed to download Whisper model {modelType}. Network error: {httpEx.Message}", httpEx);
        }
        catch (IOException ioEx)
        {
            this.logger.LogError(ioEx, "Failed to save downloaded Whisper model to {TargetPath}", targetPath);
            throw new TranscriptionException($"Failed to save downloaded Whisper model to '{targetPath}'. IO error: {ioEx.Message}", ioEx);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            this.logger.LogError(ex, "An unexpected error occurred downloading Whisper model {ModelType}", modelType);
            try { if (File.Exists(targetPath)) File.Delete(targetPath); } catch { /* Ignore delete error */ }
            throw new TranscriptionException($"Failed to download Whisper model {modelType}. Error: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// Lazily initializes the WhisperFactory instance in a thread-safe manner.
    /// Assumes the model file path (<see cref="resolvedModelPath"/>) has been set by <see cref="EnsureModelExistsAsync"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if model path is not resolved before calling.</exception>
    /// <exception cref="TranscriptionException">Thrown if factory initialization fails.</exception>
    private async Task InitializeWhisperFactoryAsync(CancellationToken cancellationToken)
    {
        // Double-check locking pattern
        if (this.whisperFactory != null)
        {
            return;
        }

        await this.lockObj.WaitAsync(cancellationToken);
        try
        {
            // Check again inside the lock
            if (this.whisperFactory != null)
            {
                return;
            }

            if (this.resolvedModelPath == null || !File.Exists(this.resolvedModelPath))
            {
                // This should not happen if EnsureModelExistsAsync was called first
                this.logger.LogError("Cannot initialize WhisperFactory: Resolved model path is null or file does not exist ({ModelPath})", this.resolvedModelPath ?? "NULL");
                throw new InvalidOperationException("Whisper model path was not correctly resolved before factory initialization.");
            }


            this.logger.LogInformation("Initializing Whisper factory with model: {ModelPath}", this.resolvedModelPath);
            try
            {
                // Consider adding .WithUseGpu(true/false) based on configuration if needed
                this.whisperFactory = WhisperFactory.FromPath(this.resolvedModelPath);
                this.logger.LogInformation("Whisper factory initialized successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "Failed to load the Whisper model and initialize factory from path: {ModelPath}", this.resolvedModelPath);
                throw new TranscriptionException($"Failed to initialize Whisper factory from model '{this.resolvedModelPath}'. Error: {ex.Message}", ex);
            }
        }
        finally
        {
            this.lockObj.Release();
        }
    }

    /// <summary>
    /// Releases the managed and unmanaged resources used by the <see cref="WhisperSpeechRecognizer"/>.
    /// </summary>
    private void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects)
                this.logger.LogDebug("Disposing WhisperFactory");
                this.whisperFactory?.Dispose();
                this.logger.LogDebug("Disposing SemaphoreSlim");
                this.lockObj.Dispose();
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer (none here).
            // Set large fields to null (whisperFactory is already nullable).
            this.whisperFactory = null;
            this.disposedValue = true;
            this.logger.LogInformation("WhisperSpeechRecognizer disposed");
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
