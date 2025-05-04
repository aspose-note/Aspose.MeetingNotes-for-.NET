using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aspose.MeetingNotes.Tests.AIIntegration;

public class MeetingNotesClientUnitTests
{
    [Fact]
    public async Task ProcessMeetingAsync_OrchestratesDependencies_OnSuccessPath()
    {
        // Arrange
        var (client, mocks) = CreateClientWithMocks();

        var testLanguage = "test-lang";
        mocks.Options.Object.Value.Language = testLanguage;
        var mockOutputStream = new MemoryStream([1, 2, 3, 4]);
        var expectedTranscriptionResult = new TranscriptionResult { Success = true, Language = testLanguage, FullText = "Specific Mock Text", Segments = [] };
        var expectedAnalyzedContent = new AnalyzedContent { Summary = "Specific Mock Summary", KeyPoints = ["Specific Point"] };
        var expectedActionItems = new List<ActionItem> { new() { Description = "Specific Action" } };

        // Override default mock setups with specific data and verification expectations
        mocks.AudioProcessor.Setup(p => p.ConvertToWavAsync(It.IsAny<FileInfo>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockOutputStream);
        mocks.SpeechRecognizer.Setup(r => r.TranscribeAsync(mockOutputStream, testLanguage, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(expectedTranscriptionResult);
        mocks.ContentAnalyzer.Setup(c => c.AnalyzeAsync(It.Is<TranscriptionResult>(t => t.Equals(expectedTranscriptionResult)), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(expectedAnalyzedContent);
        mocks.ActionItemExtractor.Setup(e => e.ExtractActionItemsAsync(It.Is<AnalyzedContent>(a => a.Equals(expectedAnalyzedContent)), It.IsAny<CancellationToken>()))
                               .ReturnsAsync(expectedActionItems);

        var tempFilePath = Path.GetTempFileName();
        var fileToProcess = new FileInfo(tempFilePath);

        try
        {
            // Act
            var actualResult = await client.ProcessMeetingAsync(fileToProcess);

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.Success);
            Assert.Empty(actualResult.ErrorMessage);
            Assert.Equal(testLanguage, actualResult.Language);
            Assert.Equal(expectedTranscriptionResult.FullText, actualResult.TranscribedText);
            Assert.Equal(expectedAnalyzedContent, actualResult.Content);
            Assert.Equal(expectedActionItems, actualResult.ActionItems);

            // Verify
            mocks.AudioProcessor.Verify(p => p.IsFormatSupported(fileToProcess.Extension), Times.Once);
            mocks.AudioProcessor.Verify(p => p.ConvertToWavAsync(fileToProcess, It.IsAny<CancellationToken>()), Times.Once);
            mocks.SpeechRecognizer.Verify(r => r.TranscribeAsync(mockOutputStream, testLanguage, It.IsAny<CancellationToken>()), Times.Once);
            mocks.ContentAnalyzer.Verify(c => c.AnalyzeAsync(It.Is<TranscriptionResult>(t => t.Equals(expectedTranscriptionResult)), It.IsAny<CancellationToken>()), Times.Once);
            mocks.ActionItemExtractor.Verify(e => e.ExtractActionItemsAsync(It.Is<AnalyzedContent>(a => a.Equals(expectedAnalyzedContent)), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            await mockOutputStream.DisposeAsync();
        }
    }

    [Fact]
    public async Task ProcessMeetingAsync_ThrowsAudioProcessingException_When_ProcessorFails()
    {
        // Arrange
        var (client, mocks) = CreateClientWithMocks();
        var exceptionToThrow = new AudioProcessingException("Mock Audio Conversion Failed");

        // Setup the audio processor mock to throw an exception
        mocks.AudioProcessor
            .Setup(p => p.ConvertToWavAsync(It.IsAny<FileInfo>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exceptionToThrow);

        var tempFilePath = Path.GetTempFileName();
        var fileToProcess = new FileInfo(tempFilePath);

        try
        {
            // Act & Assert
            var actualException = await Assert.ThrowsAsync<AudioProcessingException>(
                () => client.ProcessMeetingAsync(fileToProcess)
            );

            // Optional: Assert on exception details if needed
            Assert.Equal(exceptionToThrow.Message, actualException.Message);

            // Verify that subsequent steps were NOT called
            mocks.SpeechRecognizer.Verify(r => r.TranscribeAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
        }
    }

    [Fact]
    public async Task ProcessMeetingAsync_ThrowsTranscriptionException_When_RecognizerFails()
    {
        // Arrange
        var (client, mocks) = CreateClientWithMocks();
        var exceptionToThrow = new TranscriptionException("Mock Transcription Failed");

        // Setup the recognizer mock to throw an exception
        mocks.SpeechRecognizer
            .Setup(r => r.TranscribeAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exceptionToThrow);

        var tempFilePath = Path.GetTempFileName();
        var fileToProcess = new FileInfo(tempFilePath);

        try
        {
            // Act & Assert
            var actualException = await Assert.ThrowsAsync<TranscriptionException>(
                () => client.ProcessMeetingAsync(fileToProcess)
            );

            Assert.Equal(exceptionToThrow.Message, actualException.Message);

            // Verify that subsequent steps were NOT called
            mocks.ContentAnalyzer.Verify(c => c.AnalyzeAsync(It.IsAny<TranscriptionResult>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
        }
    }

    [Fact]
    public async Task ProcessMeetingAsync_ThrowsAIModelException_When_AnalyzerFails()
    {
        // Arrange
        var (client, mocks) = CreateClientWithMocks();
        var exceptionToThrow = new AIModelException("Mock Analysis Failed");

        // Setup the analyzer mock to throw an exception
        mocks.ContentAnalyzer
            .Setup(c => c.AnalyzeAsync(It.IsAny<TranscriptionResult>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exceptionToThrow);

        var tempFilePath = Path.GetTempFileName();
        var fileToProcess = new FileInfo(tempFilePath);

        try
        {
            // Act & Assert
            var actualException = await Assert.ThrowsAsync<AIModelException>(
                () => client.ProcessMeetingAsync(fileToProcess)
            );

            Assert.Equal(exceptionToThrow.Message, actualException.Message);

            // Verify that subsequent steps were NOT called
            mocks.ActionItemExtractor.Verify(e => e.ExtractActionItemsAsync(It.IsAny<AnalyzedContent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
        }
    }

    [Fact]
    public async Task ProcessMeetingAsync_ThrowsAIModelException_When_ExtractorFails()
    {
        // Arrange
        var (client, mocks) = CreateClientWithMocks();
        var exceptionToThrow = new AIModelException("Mock Extraction Failed");

        // Setup the extractor mock to throw an exception
        mocks.ActionItemExtractor
            .Setup(e => e.ExtractActionItemsAsync(It.IsAny<AnalyzedContent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exceptionToThrow);

        var tempFilePath = Path.GetTempFileName();
        var fileToProcess = new FileInfo(tempFilePath);

        try
        {
            // Act & Assert
            var actualException = await Assert.ThrowsAsync<AIModelException>(
                () => client.ProcessMeetingAsync(fileToProcess)
            );

            Assert.Equal(exceptionToThrow.Message, actualException.Message);
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
        }
    }

    [Fact]
    public async Task ProcessMeetingAsync_ThrowsOperationCanceledException_When_TokenCancelled()
    {
        // Arrange
        var (client, mocks) = CreateClientWithMocks();
        var cts = new CancellationTokenSource();

        // Setup a mock to throw OperationCanceledException when the token is cancelled
        // We can simulate this on any of the async calls, e.g., ConvertToWavAsync
        mocks.AudioProcessor
            .Setup(p => p.ConvertToWavAsync(It.IsAny<FileInfo>(), cts.Token)) // Ensure the mock checks the specific token
            .Returns(async (FileInfo _, CancellationToken ct) => {
                await Task.Delay(50, ct); // Simulate some work before checking cancellation
                ct.ThrowIfCancellationRequested(); // Standard way to react to cancellation
                return new MemoryStream();
            });

        var tempFilePath = Path.GetTempFileName();
        var fileToProcess = new FileInfo(tempFilePath);

        try
        {
            // Act
            // Cancel the token *before* calling the method or during its execution
            cts.Cancel();

            // Assert that the correct exception is thrown
            await Assert.ThrowsAsync<TaskCanceledException>(
                () => client.ProcessMeetingAsync(fileToProcess, cancellationToken: cts.Token)
            );
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            cts.Dispose();
        }
    }
    private static (MeetingNotesClient client, MocksContainer mocks) CreateClientWithMocks(MeetingNotesOptions? options = null)
    {
        var mockAudioProcessor = new Mock<IAudioProcessor>();
        var mockSpeechRecognizer = new Mock<ISpeechRecognizer>();
        var mockContentAnalyzer = new Mock<IContentAnalyzer>();
        var mockActionItemExtractor = new Mock<IActionItemExtractor>();
        var mockContentExporter = new Mock<IContentExporter>();
        var mockOptions = new Mock<IOptions<MeetingNotesOptions>>();
        var mockLogger = new Mock<ILogger<MeetingNotesClient>>();

        options ??= new MeetingNotesOptions
        {
            Language = "en",
            FfMpegPath = "required_but_unused_in_this_test"
        };
        mockOptions.Setup(o => o.Value).Returns(options);

        // Basic success setup for mocks (can be overridden in specific tests)
        mockAudioProcessor.Setup(p => p.IsFormatSupported(It.IsAny<string>())).Returns(true);
        mockAudioProcessor.Setup(p => p.ConvertToWavAsync(It.IsAny<FileInfo>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new MemoryStream([1, 2, 3])); // Default success stream

        mockSpeechRecognizer.Setup(r => r.TranscribeAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new TranscriptionResult { Success = true, Language = options.Language, FullText = "Default Text", Segments = [] });

        mockContentAnalyzer.Setup(c => c.AnalyzeAsync(It.IsAny<TranscriptionResult>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new AnalyzedContent { Summary = "Default Summary" });

        mockActionItemExtractor.Setup(e => e.ExtractActionItemsAsync(It.IsAny<AnalyzedContent>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync([]); // Default empty list

        var client = new MeetingNotesClient(
            mockAudioProcessor.Object,
            mockSpeechRecognizer.Object,
            mockContentAnalyzer.Object,
            mockActionItemExtractor.Object,
            mockContentExporter.Object,
            mockOptions.Object,
            mockLogger.Object);

        var mocks = new MocksContainer(
            mockAudioProcessor,
            mockSpeechRecognizer,
            mockContentAnalyzer,
            mockActionItemExtractor,
            mockContentExporter,
            mockOptions,
            mockLogger);

        return (client, mocks);
    }

    // Helper record to hold all mocks together
    private record MocksContainer(
        Mock<IAudioProcessor> AudioProcessor,
        Mock<ISpeechRecognizer> SpeechRecognizer,
        Mock<IContentAnalyzer> ContentAnalyzer,
        Mock<IActionItemExtractor> ActionItemExtractor,
        Mock<IContentExporter> ContentExporter,
        Mock<IOptions<MeetingNotesOptions>> Options,
        Mock<ILogger<MeetingNotesClient>> Logger);
}