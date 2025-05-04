using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aspose.MeetingNotes.Tests.AIIntegration;

public class CustomAIModelTests
{
    [Fact]
    public async Task ContentAnalyzer_Should_Use_Provided_AIModel()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ContentAnalyzer>>();
        var mockAIModel = new Mock<IAIModel>();

        var expectedResult = new AnalyzedContent
        {
            Summary = "Test summary from custom model",
            KeyPoints = ["Test key point"],
            Topics = ["Test topic"]
        };

        var transcriptionInputText = "Test transcription";
        mockAIModel.Setup(m => m.AnalyzeContentAsync(transcriptionInputText, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedResult);

        var contentAnalyzer = new ContentAnalyzer(mockAIModel.Object, mockLogger.Object);
        var transcription = new TranscriptionResult
        {
            Success = true,
            FullText = transcriptionInputText,
            Segments = [new TranscriptionSegment { Text = transcriptionInputText }]
        };

        // Act
        var result = await contentAnalyzer.AnalyzeAsync(transcription);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Summary, result.Summary);
        Assert.Equal(expectedResult.KeyPoints, result.KeyPoints);
        Assert.Equal(expectedResult.Topics, result.Topics);
        mockAIModel.Verify(m => m.AnalyzeContentAsync(transcriptionInputText, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActionItemExtractor_Should_Use_Provided_AIModel()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ActionItemExtractor>>();
        var mockAIModel = new Mock<IAIModel>();

        var expectedActionItems = new List<ActionItem>
        {
            new()
            {
                Description = "Test action item",
                Assignee = "Test User"
            }
        };

        var contentInputText = "Test content";
        mockAIModel.Setup(m => m.ExtractActionItemsAsync(contentInputText, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedActionItems);

        var actionItemExtractor = new ActionItemExtractor(mockAIModel.Object, mockLogger.Object);
        var content = new AnalyzedContent
        {
            TranscribedText = contentInputText,
            Sections = [new ContentSection { Content = contentInputText }]
        };

        // Act
        var result = await actionItemExtractor.ExtractActionItemsAsync(content);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedActionItems[0].Description, result[0].Description);
        Assert.Equal(expectedActionItems[0].Assignee, result[0].Assignee);
        Assert.Equal("Medium", result[0].Priority); // Check default
        Assert.Equal("Not specified", result[0].DueDate); // Check default

        // Verify the mock was called correctly
        mockAIModel.Verify(m => m.ExtractActionItemsAsync(contentInputText, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessMeetingAsync_OrchestratesDependencies_OnSuccessPath()
    {
        // --- Arrange ---

        // 1. Mock Dependencies
        var mockAudioProcessor = new Mock<IAudioProcessor>();
        var mockSpeechRecognizer = new Mock<ISpeechRecognizer>();
        var mockContentAnalyzer = new Mock<IContentAnalyzer>();
        var mockActionItemExtractor = new Mock<IActionItemExtractor>();
        var mockContentExporter = new Mock<IContentExporter>();
        var mockOptions = new Mock<IOptions<MeetingNotesOptions>>();
        var mockLogger = new Mock<ILogger<MeetingNotesClient>>();

        // 2. Define Configuration Options
        var testLanguage = "test-lang";
        var testOptions = new MeetingNotesOptions
        {
            Language = testLanguage,
            FfMpegPath = "required_but_unused_in_this_test"
        };
        mockOptions.Setup(o => o.Value).Returns(testOptions);

        // 3. Define Mock Input/Output Data Flow
        var mockInputFileInfo = new FileInfo(Path.GetTempFileName());
        var mockOutputStream = new MemoryStream([1, 2, 3]);
        var expectedTranscriptionResult = new TranscriptionResult
        {
            Success = true,
            Language = testLanguage,
            FullText = "Mock Transcription Text",
            Segments = []
        };
        var expectedAnalyzedContent = new AnalyzedContent
        {
            Summary = "Mock Summary",
            KeyPoints = ["Point 1"],
            TranscribedText = expectedTranscriptionResult.FullText
        };
        var expectedActionItems = new List<ActionItem>
        {
            new() { Description = "Mock Action Item 1" }
        };

        // 4. Setup Mock Behavior
        mockAudioProcessor.Setup(p => p.IsFormatSupported(It.IsAny<string>())).Returns(true);
        mockAudioProcessor.Setup(p => p.ConvertToWavAsync(mockInputFileInfo, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockOutputStream);

        mockSpeechRecognizer.Setup(r => r.TranscribeAsync(mockOutputStream, testLanguage, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(expectedTranscriptionResult);

        // Use It.Is<> with value equality for records to verify correct object passed
        mockContentAnalyzer.Setup(c => c.AnalyzeAsync(It.Is<TranscriptionResult>(t => t.Equals(expectedTranscriptionResult)), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(expectedAnalyzedContent);

        mockActionItemExtractor.Setup(e => e.ExtractActionItemsAsync(It.Is<AnalyzedContent>(a => a.Equals(expectedAnalyzedContent)), It.IsAny<CancellationToken>()))
                               .ReturnsAsync(expectedActionItems);


        // 5. Create the System Under Test (SUT) directly with mocks
        var client = new MeetingNotesClient(
            mockAudioProcessor.Object,
            mockSpeechRecognizer.Object,
            mockContentAnalyzer.Object,
            mockActionItemExtractor.Object,
            mockContentExporter.Object,
            mockOptions.Object,
            mockLogger.Object);

        try
        {
            // --- Act ---
            var actualResult = await client.ProcessMeetingAsync(mockInputFileInfo);

            // --- Assert ---
            Assert.NotNull(actualResult);
            Assert.True(actualResult.Success);
            Assert.Empty(actualResult.ErrorMessage);

            // Assert that the final result contains data returned by mocks
            Assert.Equal(expectedTranscriptionResult.Language, actualResult.Language);
            Assert.Equal(expectedTranscriptionResult.FullText, actualResult.TranscribedText);
            Assert.Equal(expectedAnalyzedContent, actualResult.Content);
            Assert.Equal(expectedActionItems, actualResult.ActionItems);

            // --- Verify ---
            // Verify that dependencies were called exactly once with expected parameters

            mockAudioProcessor.Verify(p => p.IsFormatSupported(mockInputFileInfo.Extension), Times.Once);
            mockAudioProcessor.Verify(p => p.ConvertToWavAsync(mockInputFileInfo, It.IsAny<CancellationToken>()), Times.Once);

            // Verify stream from processor was passed to recognizer
            mockSpeechRecognizer.Verify(r => r.TranscribeAsync(mockOutputStream, testLanguage, It.IsAny<CancellationToken>()), Times.Once);

            // Verify TranscriptionResult from recognizer was passed to analyzer
            mockContentAnalyzer.Verify(c => c.AnalyzeAsync(
                It.Is<TranscriptionResult>(t => t.Equals(expectedTranscriptionResult)),
                It.IsAny<CancellationToken>()), Times.Once);

            // Verify AnalyzedContent from analyzer was passed to extractor
            mockActionItemExtractor.Verify(e => e.ExtractActionItemsAsync(
                It.Is<AnalyzedContent>(a => a.Equals(expectedAnalyzedContent)),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            // Clean up the temporary file
            if (File.Exists(mockInputFileInfo.FullName))
            {
                File.Delete(mockInputFileInfo.FullName);
            }
            // Dispose the stream manually since await using wasn't used in setup
            await mockOutputStream.DisposeAsync();
        }
    }
}