using Aspose.MeetingNotes.Exceptions;

using FFmpeg.NET;
using FFmpeg.NET.Events;

using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.AudioProcessing;

/// <summary>
/// Processes audio input using FFmpeg to convert it into a standard WAV format.
/// Implements the <see cref="IAudioProcessor"/> interface.
/// </summary>
internal class AudioProcessor : IAudioProcessor
{
    private readonly ILogger<AudioProcessor> logger;
    private readonly string ffmpegExecutablePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioProcessor"/> class.
    /// </summary>
    /// <param name="ffmpegPath">The full path to the FFmpeg executable.</param>
    /// <param name="logger">The logger instance for logging audio processing operations.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="ffmpegPath"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the FFmpeg executable specified by <paramref name="ffmpegPath"/> is not found.</exception>
    public AudioProcessor(string ffmpegPath, ILogger<AudioProcessor> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ffmpegPath);
        ArgumentNullException.ThrowIfNull(logger);

        if (!File.Exists(ffmpegPath))
        {
            // Log critical error before throwing
            logger.LogCritical("FFmpeg executable not found at the specified path: {FfmpegPath}", ffmpegPath);
            throw new FileNotFoundException("FFmpeg executable not found.", ffmpegPath);
        }

        this.ffmpegExecutablePath = ffmpegPath;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Stream> ConvertToWavAsync(FileInfo audioFileInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioFileInfo);

        if (!audioFileInfo.Exists)
        {
            this.logger.LogError("Input audio file not found: {FilePath}", audioFileInfo.FullName);
            throw new FileNotFoundException($"The specified audio file was not found: {audioFileInfo.FullName}", audioFileInfo.FullName);
        }

        this.logger.LogInformation("Attempting to convert audio file '{FileName}' to WAV format...", audioFileInfo.Name);
        this.logger.LogDebug("Audio file full path: {FullPath}", audioFileInfo.FullName);
        this.logger.LogDebug("Using FFmpeg from: {FfmpegPath}", this.ffmpegExecutablePath);

        var ffmpeg = new Engine(this.ffmpegExecutablePath);
        ffmpeg.Progress += this.OnFfmpegProgress;
        ffmpeg.Data += this.OnFfmpegData;
        ffmpeg.Error += this.OnFfmpegError;
        ffmpeg.Complete += this.OnFfmpegComplete;

        var inputFile = new InputFile(audioFileInfo);
        var options = new ConversionOptions
        {
            AudioChanel = 1,
            ExtraArguments = "-f wav -acodec pcm_s16le -ar 16000 -ac 1"
        };

        // Using MemoryStream to capture output. Caller must dispose.
        // Initialize outside try block to ensure it's available in catch/finally if needed for disposal on error.
        var outputStream = new MemoryStream();

        try
        {
            this.logger.LogInformation("Starting FFmpeg conversion for {InputFile}", audioFileInfo.Name);

            await ffmpeg.ConvertAsync(inputFile, outputStream, options, cancellationToken);

            // Reset stream position for the consumer
            outputStream.Position = 0;
            this.logger.LogInformation("FFmpeg conversion successful for {InputFile}. Output stream size: {StreamSize} bytes", audioFileInfo.Name, outputStream.Length);

            return outputStream;
        }
        catch (OperationCanceledException)
        {
            this.logger.LogWarning("FFmpeg conversion was cancelled for {InputFile}", audioFileInfo.Name);
            await outputStream.DisposeAsync();
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "FFmpeg conversion failed for file: {FileName}", audioFileInfo.Name);
            await outputStream.DisposeAsync();
            throw new AudioProcessingException($"Audio conversion failed for '{audioFileInfo.Name}'. See inner exception for details", ex);
        }
        finally
        {
            ffmpeg.Progress -= this.OnFfmpegProgress;
            ffmpeg.Data -= this.OnFfmpegData;
            ffmpeg.Error -= this.OnFfmpegError;
            ffmpeg.Complete -= this.OnFfmpegComplete;
        }
    }

    /// <inheritdoc/>
    public bool IsFormatSupported(string fileExtension)
    {
        ArgumentNullException.ThrowIfNull(fileExtension);

        this.logger.LogDebug("Checking format support for extension: {Extension}. Assuming supported by FFmpeg", fileExtension);
        return true;
    }

    /// <summary>
    /// Handles the event raised when FFmpeg reports progress.
    /// </summary>
    private void OnFfmpegProgress(object? sender, ConversionProgressEventArgs e)
    {
        // Log detailed progress information at Trace level to avoid excessive logging
        this.logger.LogTrace(
            "FFmpeg Progress: Input={Input}, Output={Output}, Bitrate={Bitrate}, Fps={Fps}, Frame={Frame}, SizeKb={SizeKb}, Processed={ProcessedDuration}",
            e.Input?.Name ?? "N/A",
            e.Output?.Name ?? "N/A",
            e.Bitrate,
            e.Fps,
            e.Frame,
            e.SizeKb,
            e.ProcessedDuration);
    }

    /// <summary>
    /// Handles the event raised when FFmpeg outputs data (typically to stderr).
    /// </summary>
    private void OnFfmpegData(object? sender, ConversionDataEventArgs e)
    {
        this.logger.LogTrace("FFmpeg Output Data: {Data}", e.Data ?? string.Empty);
    }

    /// <summary>
    /// Handles the event raised when FFmpeg reports an error.
    /// </summary>
    private void OnFfmpegError(object? sender, ConversionErrorEventArgs e)
    {
        this.logger.LogError(
            e.Exception,
            "FFmpeg Error Event: Exception={ExceptionMessage}, Input={Input}, Output={Output}",
            e.Exception.Message,
            e.Input?.Name ?? "N/A",
            e.Output?.Name ?? "N/A");
    }

    /// <summary>
    /// Handles the event raised when the FFmpeg conversion process completes.
    /// </summary>
    private void OnFfmpegComplete(object? sender, ConversionCompleteEventArgs e)
    {
        this.logger.LogDebug(
            "FFmpeg Process Completed: Input={Input}, Output={Output}",
            e.Input?.Name ?? "N/A",
            e.Output?.Name ?? "N/A");
    }
}
