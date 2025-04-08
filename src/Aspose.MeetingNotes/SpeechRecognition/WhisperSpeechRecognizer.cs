using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Whisper.net;
using Whisper.net.Ggml;

namespace Aspose.MeetingNotes.SpeechRecognition
{
    /// <summary>
    /// Speech recognition implementation using Whisper.NET
    /// </summary>
    public class WhisperSpeechRecognizer : ISpeechRecognizer, IDisposable
    {
        private readonly ILogger<WhisperSpeechRecognizer> logger;
        private readonly MeetingNotesOptions options;
        private readonly SemaphoreSlim lockObj = new (1, 1);
        private readonly string modelPath;
        private WhisperFactory? whisperFactory;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhisperSpeechRecognizer"/> class
        /// </summary>
        /// <param name="logger">The logger instance for logging transcription events</param>
        /// <param name="options">The configuration options for the speech recognizer</param>
        public WhisperSpeechRecognizer(
            ILogger<WhisperSpeechRecognizer> logger,
            IOptions<MeetingNotesOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;
            this.modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "ggml-base-gguf.bin");
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="WhisperSpeechRecognizer"/> class.
        /// Finalizer for WhisperSpeechRecognizer.
        /// </summary>
        ~WhisperSpeechRecognizer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Transcribes audio using Whisper
        /// </summary>
        /// <param name="audio">The processed audio to transcribe</param>
        /// <param name="language">The language code of the audio (e.g., "en", "ru")</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation</param>
        /// <returns>A TranscriptionResult containing the transcribed text segments and metadata</returns>
        public async Task<TranscriptionResult> TranscribeAsync(
            ProcessedAudio audio,
            string language,
            CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Starting transcription with language: {Language}", language);

                // Ensure model exists
                if (!File.Exists(modelPath))
                {
                    logger.LogWarning("Whisper model not found at {ModelPath}, downloading...", modelPath);
                    await DownloadModelAsync(cancellationToken);
                }

                // Initialize Whisper factory if needed
                await InitializeWhisperFactoryAsync(cancellationToken);

                if (whisperFactory == null)
                {
                    throw new TranscriptionException("Failed to initialize Whisper factory");
                }

                // Create processor with specified language
                using var processor = whisperFactory.CreateBuilder()
                    .WithLanguage(options.Language)
                    .Build();

                // Process audio
                logger.LogInformation("Processing audio stream...");
                var segments = new List<TranscriptionSegment>();

                await foreach (var segment in processor.ProcessAsync(audio.AudioStream, cancellationToken))
                {
                    segments.Add(new TranscriptionSegment
                    {
                        Speaker = "Unknown", // TODO: Implement speaker diarization
                        Text = segment.Text,
                        StartTime = segment.Start,
                        EndTime = segment.End
                    });
                }

                return new TranscriptionResult
                {
                    Segments = segments,
                    Language = language,
                    Success = true,
                    FullText = string.Join(" ", segments.Select(s => s.Text))
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during transcription");
                return new TranscriptionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task InitializeWhisperFactoryAsync(CancellationToken cancellationToken)
        {
            if (whisperFactory != null)
            {
                return;
            }

            await lockObj.WaitAsync(cancellationToken);
            try
            {
                if (whisperFactory != null)
                {
                    return;
                }

                logger.LogInformation("Initializing Whisper factory with model: {ModelPath}", modelPath);

                try
                {
                    whisperFactory = WhisperFactory.FromPath(modelPath);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to load the Whisper model from path: {ModelPath}", modelPath);
                    throw;
                }
            }
            finally
            {
                lockObj.Release();
            }
        }

        private async Task DownloadModelAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Downloading Whisper model...");

                // Ensure models directory exists
                var modelsDir = Path.GetDirectoryName(modelPath);
                if (!Directory.Exists(modelsDir))
                {
                    Directory.CreateDirectory(modelsDir!);
                }

                // Download model using WhisperGgmlDownloader
                using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.Base, cancellationToken: cancellationToken);

                // Save model to file
                using var fileStream = File.Create(modelPath);
                await modelStream.CopyToAsync(fileStream, cancellationToken);

                logger.LogInformation("Whisper model downloaded successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error downloading Whisper model");
                throw new TranscriptionException("Failed to download Whisper model", ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the WhisperSpeechRecognizer and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                whisperFactory?.Dispose();
                lockObj.Dispose();
            }

            disposed = true;
        }
    }
}
