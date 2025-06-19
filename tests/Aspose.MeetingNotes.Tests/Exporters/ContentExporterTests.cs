using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.Tests.Exporters
{
    public class ContentExporterTests : IDisposable
    {
        private readonly Mock<ILogger<ContentExporter>> _mockLogger;
        private readonly AnalyzedContent _sampleAnalyzedContent;
        private readonly List<ActionItem> _sampleActionItems;
        private readonly string _tempDirectory;

        public ContentExporterTests()
        {
            _mockLogger = new Mock<ILogger<ContentExporter>>();
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);

            _sampleAnalyzedContent = new AnalyzedContent
            {
                Summary = "This is a meeting summary.",
                KeyPoints = ["Point 1", "Point 2"],
                Decisions = ["Decision A", "Decision B"],
                Topics = ["Topic X", "Topic Y"],
                QASegments =
                [
                    new QASegment { Question = "Q1?", Answer = "A1." },
                    new QASegment { Question = "Q2?", Answer = "A2." }
                ],
                Sections =
                [
                    new ContentSection { Title = "Section Alpha", Content = "Content of Alpha." },
                    new ContentSection { Title = "Section Beta", Content = "Content of Beta." }
                ],
                TranscribedText = "Full meeting transcript goes here."
            };

            _sampleActionItems =
            [
                new ActionItem { Description = "Do task 1", Assignee = "Alice", DueDate = "Tomorrow", Priority = "High" },
                new ActionItem { Description = "Review item 2", Assignee = "Bob" }
            ];
        }

        private ContentExporter CreateExporter(string? asposeLicensePath = null)
        {
            var options = new MeetingNotesOptions { AsposeLicensePath = asposeLicensePath, FfMpegPath = "dummy_ffmpeg_path_not_used_in_exporter" };
            var mockOptions = new Mock<IOptions<MeetingNotesOptions>>();
            mockOptions.Setup(o => o.Value).Returns(options);
            return new ContentExporter(_mockLogger.Object, mockOptions.Object);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task ExportAsync_ToMarkdown_GeneratesCorrectContent()
        {
            // Arrange
            var exporter = CreateExporter();

            // Act
            var result = await exporter.ExportAsync(_sampleAnalyzedContent, _sampleActionItems, ExportFormat.Markdown);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ExportFormat.Markdown, result.Format);
            Assert.NotNull(result.Text);
            Assert.Null(result.Data);

            Assert.Contains("# Meeting Notes", result.Text);
            Assert.Contains("## Summary", result.Text);
            Assert.Contains(_sampleAnalyzedContent.Summary, result.Text);

            Assert.Contains("## Action Items", result.Text);
            Assert.Contains(_sampleActionItems[0].Description, result.Text);
            Assert.Contains(_sampleActionItems[0].Assignee, result.Text);
            Assert.Contains(_sampleActionItems[1].Description, result.Text);

            Assert.Contains("## Key Points", result.Text);
            Assert.Contains(_sampleAnalyzedContent.KeyPoints[0], result.Text);

            Assert.Contains("## Decisions", result.Text);
            Assert.Contains(_sampleAnalyzedContent.Decisions[0], result.Text);

            Assert.Contains("## Topics Discussed", result.Text);
            Assert.Contains(_sampleAnalyzedContent.Topics[0], result.Text);

            Assert.Contains("## Questions and Answers", result.Text);
            Assert.Contains(_sampleAnalyzedContent.QASegments[0].Question, result.Text);
            Assert.Contains(_sampleAnalyzedContent.QASegments[0].Answer, result.Text);

            Assert.Contains("## Meeting Content Sections", result.Text);
            Assert.Contains($"### {_sampleAnalyzedContent.Sections[0].Title}", result.Text);
            Assert.Contains(_sampleAnalyzedContent.Sections[0].Content, result.Text);

            Assert.Contains("<summary>## Full Transcript (Click to expand)</summary>", result.Text);
            Assert.Contains(_sampleAnalyzedContent.TranscribedText, result.Text);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("AsposeLicensePath is not configured")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        [Theory]
        [InlineData(ExportFormat.PDF)]
        [InlineData(ExportFormat.HTML)]
        [InlineData(ExportFormat.OneNote)]
        public async Task ExportAsync_ToLicensedFormat_LicensePathNotConfigured_LogsWarningAndAttemptsExport(ExportFormat format)
        {
            // Arrange
            var exporter = CreateExporter(asposeLicensePath: null);

            // Act
            var result = await exporter.ExportAsync(_sampleAnalyzedContent, _sampleActionItems, format);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("AsposeLicensePath is not configured. Aspose components will run in evaluation mode")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0, $"{format} output data should not be empty.");
        }

        [Theory]
        [InlineData(ExportFormat.PDF)]
        [InlineData(ExportFormat.HTML)]
        [InlineData(ExportFormat.OneNote)]
        public async Task ExportAsync_ToLicensedFormat_LicenseFileDoesNotExist_ThrowsFileNotFoundException(ExportFormat format)
        {
            // Arrange
            string nonExistentLicenseFile = Path.Combine(_tempDirectory, "non_existent_license.lic");

            if (File.Exists(nonExistentLicenseFile)) File.Delete(nonExistentLicenseFile);

            var exporter = CreateExporter(asposeLicensePath: nonExistentLicenseFile);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() =>
                exporter.ExportAsync(_sampleAnalyzedContent, _sampleActionItems, format)
            );

            Assert.Contains("Aspose license file not found", exception.Message);
            Assert.Equal(nonExistentLicenseFile, exception.FileName);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Aspose license file not found at the specified path")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExportAsync_UnsupportedFormat_ThrowsArgumentException()
        {
            // Arrange
            var exporter = CreateExporter();
            var unsupportedFormat = (ExportFormat)999;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                exporter.ExportAsync(_sampleAnalyzedContent, _sampleActionItems, unsupportedFormat)
            );
        }
    }
}