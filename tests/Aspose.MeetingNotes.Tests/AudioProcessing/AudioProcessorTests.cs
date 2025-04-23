using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Aspose.MeetingNotes.AudioProcessing;

namespace Aspose.MeetingNotes.Tests.AudioProcessing
{
    public class AudioProcessorTests
    {
        private readonly ILogger<AudioProcessor> _logger;
        private readonly AudioProcessor _processor;
        private readonly List<IAudioFormatHandler> _handlers;

        public AudioProcessorTests()
        {
            _logger = Mock.Of<ILogger<AudioProcessor>>();
            _handlers =
            [
                new TestAudioHandler(Mock.Of<ILogger>())
            ];
            _processor = new AudioProcessor(_logger, _handlers);
        }

        [Theory]
        [InlineData(".test", true)]
        [InlineData(".txt", false)]
        public void IsFormatSupported_ReturnsExpectedResult(string extension, bool expected)
        {
            var result = _processor.IsFormatSupported(extension);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ProcessAsync_WithValidAudio_ReturnsProcessedAudio()
        {
            // Arrange
            var audioData = new byte[] { 1, 2, 3, 4 };
            var stream = new MemoryStream(audioData);

            // Act
            var result = await _processor.ConvertToWavAsync(stream, ".test");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AudioStream);
        }
    }
} 