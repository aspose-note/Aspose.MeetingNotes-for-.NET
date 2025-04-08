using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Examples
{
    /// <summary>
    /// Example of a custom speech recognizer implementation that returns mock transcription
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
                Segments = new List<TranscriptionSegment>
                {
                    new TranscriptionSegment
                    {
                        Speaker = "Speaker 1",
                        Text = "This is a mock transcription of the first speaker.",
                        StartTime = TimeSpan.Zero,
                        EndTime = TimeSpan.FromSeconds(5)
                    },
                    new TranscriptionSegment
                    {
                        Speaker = "Speaker 2",
                        Text = "And this is a mock transcription of the second speaker.",
                        StartTime = TimeSpan.FromSeconds(5),
                        EndTime = TimeSpan.FromSeconds(10)
                    }
                },
                Language = language,
                Success = true,
                FullText = "This is a mock transcription of the first speaker. And this is a mock transcription of the second speaker."
            };
        }
    }

    /// <summary>
    /// Example demonstrating how to use a custom speech recognizer implementation
    /// </summary>
    public static class CustomSpeechRecognizerExample
    {
        public static async Task RunAsync()
        {
            // Configure services
            var services = new ServiceCollection();
            
            // Configure logging
            services.AddLogging(builder => 
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Configure MeetingNotes with custom speech recognizer
            services.AddMeetingNotes(options =>
            {
                options.CustomSpeechRecognizer = new MockSpeechRecognizer(
                    services.BuildServiceProvider().GetRequiredService<ILogger<MockSpeechRecognizer>>());
                options.Language = "en";
            });

            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetRequiredService<MeetingNotesClient>();

            // Example usage
            var result = await client.ProcessMeetingAsync(
                new FileStream("example.mp3", FileMode.Open),
                new MeetingNotesOptions
                {
                    Language = "en",
                    ExportFormat = ExportFormat.Markdown
                });
        }
    }
} 