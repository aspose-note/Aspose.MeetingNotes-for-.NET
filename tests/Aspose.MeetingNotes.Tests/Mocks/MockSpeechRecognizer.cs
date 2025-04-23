using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of ISpeechRecognizer for testing purposes
    /// </summary>
    public class MockSpeechRecognizer : ISpeechRecognizer
    {
        private readonly ILogger<MockSpeechRecognizer> logger;

        public MockSpeechRecognizer(ILogger<MockSpeechRecognizer> logger)
        {
            this.logger = logger;
        }

        public async Task<TranscriptionResult> TranscribeAsync(ProcessedAudio audio, string language, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Transcribing audio with mock speech recognizer");

            // Simulate some processing time
            await Task.Delay(100, cancellationToken);

            return new TranscriptionResult
            {
                Segments =
                [
                    new TranscriptionSegment
                    {
                        Speaker = "Speaker 1",
                        Text = "This is a test transcription for the custom AI model test.",
                        StartTime = TimeSpan.Zero,
                        EndTime = TimeSpan.FromSeconds(5)
                    }
                ],
                Language = language,
                Success = true,
                FullText = "This is a test transcription for the custom AI model test."
            };
        }
    }
} 