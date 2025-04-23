using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.DependencyInjection;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Aspose.MeetingNotes.Tests.AIIntegration
{
    public class CustomAIModelTests
    {
        [Fact]
        public async Task ContentAnalyzer_Should_Use_Custom_AIModel()
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

            mockAIModel.Setup(m => m.AnalyzeContentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var contentAnalyzer = new ContentAnalyzer(mockAIModel.Object, mockLogger.Object);
            var transcription = new TranscriptionResult
            {
                Segments =
                [
                    new TranscriptionSegment { Text = "Test transcription" }
                ]
            };

            // Act
            var result = await contentAnalyzer.AnalyzeAsync(transcription);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Summary, result.Summary);
            Assert.Equal(expectedResult.KeyPoints, result.KeyPoints);
            //Assert.Equal(expectedResult.Topics, result.Topics);
            
            mockAIModel.Verify(m => m.AnalyzeContentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ActionItemExtractor_Should_Use_Custom_AIModel()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ActionItemExtractor>>();
            var mockAIModel = new Mock<IAIModel>();
            
            var expectedActionItems = new List<ActionItem>
            {
                new() {
                    Description = "Test action item",
                    Assignee = "Test User",
                    DueDate = DateTime.Now.AddDays(7).ToString(),
                    Status = "New"
                }
            };

            mockAIModel.Setup(m => m.ExtractActionItemsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedActionItems);

            var actionItemExtractor = new ActionItemExtractor(mockAIModel.Object, mockLogger.Object);
            var content = new AnalyzedContent
            {
                Sections = new List<ContentSection>
                {
                    new ContentSection { Content = "Test content" }
                }
            };

            // Act
            var result = await actionItemExtractor.ExtractActionItemsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedActionItems[0].Description, result[0].Description);
            Assert.Equal(expectedActionItems[0].Assignee, result[0].Assignee);
            Assert.Equal(expectedActionItems[0].DueDate, result[0].DueDate);
            Assert.Equal(expectedActionItems[0].Status, result[0].Status);
            
            mockAIModel.Verify(m => m.ExtractActionItemsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MeetingNotesClient_Should_Use_Custom_AIModel()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockLogger = new Mock<ILogger<MockAIModel>>();
            var mockSpeechLogger = new Mock<ILogger<MockSpeechRecognizer>>();
            var mockAudioLogger = new Mock<ILogger<MockAudioProcessor>>();
            var customModel = new MockAIModel(mockLogger.Object);
            var mockSpeechRecognizer = new MockSpeechRecognizer(mockSpeechLogger.Object);
            var mockAudioProcessor = new MockAudioProcessor(mockAudioLogger.Object);

            services.AddLogging();
            services.AddMeetingNotes(options =>
            {
                options.CustomAIModel = customModel;
                options.CustomSpeechRecognizer = mockSpeechRecognizer;
                options.Language = "en";
                options.ExportFormat = ExportFormat.Markdown;
            });

            // Register mock audio processor
            services.AddSingleton<IAudioProcessor>(mockAudioProcessor);

            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetRequiredService<MeetingNotesClient>();

            // Create a mock audio stream
            using var audioStream = new MemoryStream();
            using var writer = new StreamWriter(audioStream);
            writer.Write("Test audio content");
            writer.Flush();
            audioStream.Position = 0;

            // Act
            var result = await client.ProcessMeetingAsync(audioStream, ".wav");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("This is a mock summary generated by the custom AI model.", result.Content.Summary);
            Assert.Equal(2, result.ActionItems.Count);
            Assert.Equal("Mock action item 1", result.ActionItems[0].Description);
            Assert.Equal("John Doe", result.ActionItems[0].Assignee);
            Assert.Equal("Mock action item 2", result.ActionItems[1].Description);
            Assert.Equal("Jane Smith", result.ActionItems[1].Assignee);
        }
    }
}
