using System.Text;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Tests.Mocks;

/// <summary>
/// Mock implementation of the refactored ISpeechRecognizer for testing purposes.
/// </summary>
public class MockSpeechRecognizer : ISpeechRecognizer
{
    private readonly ILogger<MockSpeechRecognizer> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockSpeechRecognizer"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public MockSpeechRecognizer(ILogger<MockSpeechRecognizer> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        this.logger = logger;
    }

    /// <summary>
    /// Simulates transcription, returning predefined mock segments.
    /// Reads the stream to simulate processing but ignores content.
    /// </summary>
    /// <param name="audioStream">The input audio stream (content ignored in mock).</param>
    /// <param name="language">The requested language.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task containing a mock TranscriptionResult.</returns>
    public async Task<TranscriptionResult> TranscribeAsync(
        Stream audioStream,
        string language,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioStream);
        this.logger.LogInformation("MockSpeechRecognizer: Simulating transcription for language {Language}", language);

        // Simulate reading the stream
        using var reader = new StreamReader(audioStream, Encoding.UTF8, leaveOpen: true);
        await reader.ReadToEndAsync(cancellationToken);

        // Simulate processing time
        await Task.Delay(50, cancellationToken);

        // Return mock result
        var mockText = $"This is a test transcription for language {language}.";
        return new TranscriptionResult
        {
            Segments = new List<TranscriptionSegment>
            {
                new() // Using record initialization syntax
                {
                    Speaker = "Mock Speaker 1",
                    Text = mockText,
                    StartTime = TimeSpan.Zero,
                    EndTime = TimeSpan.FromSeconds(5)
                }
            },
            Language = language,
            Success = true,
            FullText = mockText
        };
    }
}