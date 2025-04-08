using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aspose.MeetingNotes.Tests.SpeechRecognition
{
    public class WhisperSpeechRecognizerTests
    {
        [Fact]
        public async Task Should_Transcribe_Audio_File()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<WhisperSpeechRecognizer>>();
            var options = new MeetingNotesOptions
            {
                Language = "en"
            };
            var optionsWrapper = Options.Create(options);

            var recognizer = new WhisperSpeechRecognizer(mockLogger.Object, optionsWrapper);

            var audioFilePath = Path.Combine("test-data", "audio", "kennedy.wav");
            using var audioStream = File.OpenRead(audioFilePath);
            var processedAudio = new ProcessedAudio { AudioStream = audioStream };

            // Act
            var result = await recognizer.TranscribeAsync(processedAudio, "en");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Segments);
            Assert.NotEmpty(result.Segments);
            Assert.NotNull(result.FullText);
            Assert.NotEmpty(result.FullText);
            Assert.Equal(" I believe that this nation should commit itself to achieving the goal before this decade is out  of landing a man on the moon and returning him safely to the earth.  No single space project in this period will be more impressive to mankind or more important for the long range exploration of space.",
                result.FullText);
            Assert.Equal("en", result.Language);
        }

        [Fact]
        public async Task Should_Handle_Invalid_Audio_File()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<WhisperSpeechRecognizer>>();
            var options = new MeetingNotesOptions
            {
                Language = "en"
            };
            var optionsWrapper = Options.Create(options);

            var recognizer = new WhisperSpeechRecognizer(mockLogger.Object, optionsWrapper);

            // Create an invalid audio stream
            using var invalidStream = new MemoryStream(new byte[100]);

            var processedAudio = new ProcessedAudio { AudioStream = invalidStream };

            // Act
            var result = await recognizer.TranscribeAsync(processedAudio, "en");

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal("Invalid wave file RIFF header.", result.ErrorMessage);
        }
    }
} 