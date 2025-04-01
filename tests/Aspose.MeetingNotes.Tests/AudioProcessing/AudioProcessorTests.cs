using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.Tests.AudioProcessing
{
    public class AudioProcessorTests
    {
        private readonly ILogger<AudioProcessor> _logger;
        private readonly AudioProcessor _processor;

        public AudioProcessorTests()
        {
            _logger = Mock.Of<ILogger<AudioProcessor>>();
            _processor = new AudioProcessor(_logger);
        }

        [Theory]
        [InlineData(".mp3", true)]
        [InlineData(".wav", true)]
        [InlineData(".m4a", true)]
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
            var result = await _processor.ProcessAsync(stream);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AudioStream);
        }
    }
} 