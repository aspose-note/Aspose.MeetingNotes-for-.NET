using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;

using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aspose.MeetingNotes.Tests.SpeechRecognition;

public class WhisperSpeechRecognizerTests : IDisposable
{
    private readonly string _testAudioPath = Path.Combine("test-data", "audio", "kennedy.wav");

    // Helper method to create recognizer for tests
    private WhisperSpeechRecognizer CreateRecognizer(MeetingNotesOptions? options = null)
    {
        var mockLogger = new Mock<ILogger<WhisperSpeechRecognizer>>();
        options ??= new MeetingNotesOptions
        {
            Language = "en",
            FfMpegPath = "dummy",
            SpeechRecognition = new SpeechRecognitionOptions
            {
                ModelSize = "tiny" // Use tiny for faster tests if downloading
            }
        };
        var optionsWrapper = Options.Create(options);
        return new WhisperSpeechRecognizer(mockLogger.Object, optionsWrapper);
    }

    [Fact]
    public async Task Should_Transcribe_Valid_Wav_File_Stream()
    {
        // Arrange
        if (!File.Exists(_testAudioPath))
        {
            Assert.Fail($"Test audio file not found: {Path.GetFullPath(_testAudioPath)}. Ensure 'test-data' folder is copied to output.");
            return;
        }

        var recognizer = CreateRecognizer();

        // Act & Assert
        TranscriptionResult result;
        try
        {
            await using var audioStream = File.OpenRead(_testAudioPath);
            result = await recognizer.TranscribeAsync(audioStream, "en");
        }
        finally
        {
            recognizer.Dispose();
        }

        // Assert results
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Segments);
        Assert.NotEmpty(result.Segments);
        Assert.NotNull(result.FullText);
        Assert.NotEmpty(result.FullText);
        var expectedText = "I believe that this nation should commit itself to achieving the goal before this decade is out of landing a man on the moon and returning him safely to the earth. No single space project in this period will be more impressive to mankind or more important for the long range exploration of space.";
        Assert.Contains(expectedText.ToLowerInvariant().Replace(" ", ""), result.FullText.ToLowerInvariant().Replace(" ", ""));
        Assert.Equal("en", result.Language);
    }

    [Fact]
    public async Task Should_Throw_Exception_For_Invalid_Audio_Stream()
    {
        // Arrange
        var recognizer = CreateRecognizer();

        // Act & Assert
        try
        {
            // Create an invalid audio stream (e.g., empty or non-WAV data)
            await using var invalidStream = new MemoryStream([0, 1, 2, 3, 4, 5]); // Not a WAV

            // Ensure TranscribeAsync throws the expected exception
            await Assert.ThrowsAsync<TranscriptionException>(async () =>
            {
                // Pass the invalid Stream directly
                await recognizer.TranscribeAsync(invalidStream, "en");
            });
        }
        finally
        {
            recognizer.Dispose();
        }
    }

    public void Dispose()
    {
        // Cleanup if needed, e.g., delete downloaded models if tests manage them
        GC.SuppressFinalize(this);
    }
}