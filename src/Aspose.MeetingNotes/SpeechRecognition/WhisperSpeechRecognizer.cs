using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Configuration;
using Whisper.net;
using Whisper.net.Ggml;
using Aspose.MeetingNotes.Metrics;
using Aspose.MeetingNotes.Exceptions;

namespace Aspose.MeetingNotes.SpeechRecognition
{
    /// <summary>
    /// Speech recognition implementation using Whisper.NET
    /// </summary>
    public class WhisperSpeechRecognizer : ISpeechRecognizer, IDisposable
    {
        private readonly ILogger<WhisperSpeechRecognizer> _logger;
        private readonly MeetingNotesOptions _options;
        private readonly IMetricsCollector _metrics;
        private readonly string _modelPath;
        private WhisperFactory? _whisperFactory;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the WhisperSpeechRecognizer class
        /// </summary>
        /// <param name="logger">The logger instance for logging transcription events</param>
        /// <param name="options">The configuration options for the speech recognizer</param>
        /// <param name="metrics">The metrics collector for recording transcription metrics</param>
        public WhisperSpeechRecognizer(
            ILogger<WhisperSpeechRecognizer> logger,
            IOptions<MeetingNotesOptions> options,
            IMetricsCollector metrics)
        {
            _logger = logger;
            _options = options.Value;
            _metrics = metrics;
            _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "ggml-base.bin");
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
                _logger.LogInformation("Starting transcription with language: {Language}", language);

                // Ensure model exists
                if (!File.Exists(_modelPath))
                {
                    _logger.LogWarning("Whisper model not found at {ModelPath}, downloading...", _modelPath);
                    await DownloadModelAsync(cancellationToken);
                }

                // Initialize Whisper factory if needed
                await InitializeWhisperFactoryAsync(cancellationToken);

                if (_whisperFactory == null)
                {
                    throw new TranscriptionException("Failed to initialize Whisper factory");
                }

                // Create processor with specified language
                using var processor = _whisperFactory.CreateBuilder()
                    .WithLanguage("auto")
                    .Build();

                // Process audio
                _logger.LogInformation("Processing audio stream...");
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

                // Record metrics
                _metrics.RecordMetric("transcription.segments", segments.Count);
                _metrics.RecordTiming("transcription.duration", 
                    (segments.LastOrDefault()?.EndTime ?? TimeSpan.Zero).TotalMilliseconds);

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
                _logger.LogError(ex, "Error during transcription");
                return new TranscriptionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task InitializeWhisperFactoryAsync(CancellationToken cancellationToken)
        {
            if (_whisperFactory != null) return;

            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_whisperFactory != null) return;

                _logger.LogInformation("Initializing Whisper factory with model: {ModelPath}", _modelPath);
                _whisperFactory = WhisperFactory.FromPath(_modelPath);
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task DownloadModelAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Downloading Whisper model...");

                // Ensure models directory exists
                var modelsDir = Path.GetDirectoryName(_modelPath);
                if (!Directory.Exists(modelsDir))
                {
                    Directory.CreateDirectory(modelsDir!);
                }

                // Download model using WhisperGgmlDownloader
                using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.Base, cancellationToken: cancellationToken);
                
                // Save model to file
                using var fileStream = File.Create(_modelPath);
                await modelStream.CopyToAsync(fileStream, cancellationToken);

                _logger.LogInformation("Whisper model downloaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading Whisper model");
                throw new TranscriptionException("Failed to download Whisper model", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _whisperFactory?.Dispose();
                _lock.Dispose();
            }

            _disposed = true;
        }

        ~WhisperSpeechRecognizer()
        {
            Dispose(false);
        }
    }
} 