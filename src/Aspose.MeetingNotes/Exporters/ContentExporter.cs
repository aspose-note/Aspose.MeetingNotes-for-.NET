using System.Drawing;
using System.Text;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Models;
using Aspose.Note;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aspose.MeetingNotes.Exporters;

/// <summary>
/// Default implementation of <see cref="IContentExporter"/> that handles exporting
/// analyzed meeting content and action items to various formats.
/// Currently, only Markdown export is implemented.
/// </summary>
public class ContentExporter : IContentExporter
{
    private static readonly ParagraphStyle DefaultTextStyle = new ParagraphStyle { FontColor = Color.Black, FontName = "Arial", FontSize = 10 };
    private static readonly ParagraphStyle Heading1Style = new ParagraphStyle { FontColor = Color.Black, FontName = "Arial", FontSize = 16, IsBold = true };
    private static readonly ParagraphStyle Heading2Style = new ParagraphStyle { FontColor = Color.Black, FontName = "Arial", FontSize = 14, IsBold = true };
    private static readonly ParagraphStyle Heading3Style = new ParagraphStyle { FontColor = Color.Black, FontName = "Arial", FontSize = 12, IsBold = true };

    private readonly ILogger<ContentExporter> logger;
    private readonly MeetingNotesOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentExporter"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging export operations.</param>
    /// <param name="options">The configuration options for the exporter, including the Aspose license path.</param>
    /// <exception cref="ArgumentNullException">Thrown if logger or options is null.</exception>
    public ContentExporter(ILogger<ContentExporter> logger, IOptions<MeetingNotesOptions> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);
        this.logger = logger;
        this.options = options.Value ?? throw new ArgumentNullException(nameof(options), "MeetingNotesOptions cannot be null.");
    }

    /// <inheritdoc/>
    public async Task<ExportResult> ExportAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        ExportFormat format,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(actionItems);

        this.logger.LogInformation("Initiating export of meeting content to {Format} format", format);

        try
        {
            // Activate Aspose licenses if required for the format
            if (format != ExportFormat.Markdown)
            {
                if (string.IsNullOrWhiteSpace(this.options.AsposeLicensePath))
                {
                    this.logger.LogWarning("AsposeLicensePath is not configured. Aspose components will run in evaluation mode for formats: {Format}.", format);
                }
                else if (!File.Exists(this.options.AsposeLicensePath))
                {
                    this.logger.LogError("Aspose license file not found at the specified path: {AsposeLicensePath}", this.options.AsposeLicensePath);
                    throw new FileNotFoundException("Aspose license file not found.", this.options.AsposeLicensePath);
                }
                else
                {
                    this.logger.LogInformation("Attempting to set Aspose licenses from path: {AsposeLicensePath}", this.options.AsposeLicensePath);

                    // Set Aspose.Note License
                    try
                    {
                        var noteLicense = new Aspose.Note.License();
                        noteLicense.SetLicense(options.AsposeLicensePath);
                        this.logger.LogDebug("Aspose.Note license set successfully.");
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Failed to set Aspose.Note license from path: {AsposeLicensePath}. Ensure the license is valid for Aspose.Note.", this.options.AsposeLicensePath);
                        // Optionally, rethrow a more specific exception or a general one indicating which component failed
                        throw new InvalidOperationException($"Failed to set Aspose.Note license. Please ensure the license file is valid for Aspose.Note. Path: {this.options.AsposeLicensePath}", ex);
                    }
                }
            }

            // Declare variables to hold the result data before creating ExportResult
            string? resultText = null;
            byte[]? resultData = null;

            // Determine the content based on the format by calling the specific methods
            switch (format)
            {
                case ExportFormat.Markdown:
                    resultText = await this.ExportToMarkdownAsync(content, actionItems, cancellationToken);
                    break;
                case ExportFormat.OneNote:
                    resultData = await this.ExportToOneNoteAsync(content, actionItems, cancellationToken);
                    break;
                case ExportFormat.PDF:
                    resultData = await this.ExportToPdfAsync(content, actionItems, cancellationToken);
                    break;
                case ExportFormat.HTML:
                    resultData = await this.ExportToHtmlAsync(content, actionItems, cancellationToken);
                    break;
                default:
                    this.logger.LogError("Unsupported export format specified: {Format}", format);
                    throw new ArgumentException($"Unsupported export format: {format}", nameof(format));
            }

            var exportResult = new ExportResult {
                Format = format,
                Text = resultText,
                Data = resultData
            };

            this.logger.LogInformation("Successfully prepared export result for {Format}", format);
            return exportResult;
        }
        catch (NotImplementedException)
        {
            this.logger.LogError("Export to {Format} is not implemented", format);
            throw;
        }
        catch (OperationCanceledException)
        {
            this.logger.LogWarning("Export operation to {Format} was cancelled", format);
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred during export to {Format}", format);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task<string> ExportToMarkdownAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(actionItems);

        this.logger.LogDebug("Generating Markdown content");

        var sb = new StringBuilder();

        // Title
        sb.AppendLine("# Meeting Notes");
        sb.AppendLine();

        // Summary
        if (!string.IsNullOrWhiteSpace(content.Summary))
        {
            sb.AppendLine("## Summary");
            sb.AppendLine(content.Summary.Trim());
            sb.AppendLine();
        }

        // Action Items
        if (actionItems.Any())
        {
            sb.AppendLine("## Action Items");
            foreach (var item in actionItems)
            {
                cancellationToken.ThrowIfCancellationRequested();
                sb.Append($"- **{item.Description?.Trim()}**");
                if (!string.IsNullOrWhiteSpace(item.Assignee) && !item.Assignee.Equals("Unassigned", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append($" (Assignee: {item.Assignee.Trim()})");
                }

                if (!string.IsNullOrWhiteSpace(item.DueDate) && !item.DueDate.Equals("Not specified", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append($" (Due: {item.DueDate.Trim()})");
                }

                if (!string.IsNullOrWhiteSpace(item.Priority) && !item.Priority.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append($" [Priority: {item.Priority}]");
                }

                sb.AppendLine();
            }

            sb.AppendLine();
        }

        // Key Points
        if (content.KeyPoints?.Any() ?? false)
        {
            sb.AppendLine("## Key Points");
            foreach (var point in content.KeyPoints)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrWhiteSpace(point))
                {
                    sb.AppendLine($"- {point.Trim()}");
                }
            }

            sb.AppendLine();
        }

        // Decisions
        if (content.Decisions?.Any() ?? false)
        {
            sb.AppendLine("## Decisions");
            foreach (var decision in content.Decisions)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrWhiteSpace(decision))
                {
                    sb.AppendLine($"- {decision.Trim()}");
                }
            }

            sb.AppendLine();
        }

        // Topics Discussed
        if (content.Topics?.Any() ?? false)
        {
            sb.AppendLine("## Topics Discussed");
            foreach (var topic in content.Topics)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrWhiteSpace(topic))
                {
                    sb.AppendLine($"- {topic.Trim()}");
                }
            }

            sb.AppendLine();
        }

        // Q&A Section
        if (content.QASegments?.Any() ?? false)
        {
            sb.AppendLine("## Questions and Answers");
            foreach (var qa in content.QASegments)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (qa != null && !string.IsNullOrWhiteSpace(qa.Question))
                {
                    sb.AppendLine($"- **Q:** {qa.Question.Trim()}");
                    if (!string.IsNullOrWhiteSpace(qa.Answer))
                    {
                        sb.AppendLine($"  **A:** {qa.Answer.Trim()}");
                    }
                }
            }

            sb.AppendLine();
        }

        // Detailed Sections
        if (content.Sections?.Any() ?? false)
        {
            sb.AppendLine("## Meeting Content Sections");
            foreach (var section in content.Sections)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (section != null && !string.IsNullOrWhiteSpace(section.Title))
                {
                    sb.AppendLine($"### {section.Title.Trim()}");
                    if (!string.IsNullOrWhiteSpace(section.Content))
                    {
                        sb.AppendLine(section.Content.Trim());
                    }

                    sb.AppendLine();
                }
            }
        }

        // Full Transcript
        if (!string.IsNullOrWhiteSpace(content.TranscribedText))
        {
            sb.AppendLine("<details>");
            sb.AppendLine("<summary>## Full Transcript (Click to expand)</summary>");
            sb.AppendLine();
            sb.AppendLine("```");
            sb.AppendLine(content.TranscribedText.Trim());
            sb.AppendLine("```");
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        this.logger.LogDebug("Markdown content generated successfully");
        return Task.FromResult(sb.ToString());
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportToOneNoteAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        this.logger.LogInformation("Exporting to OneNote directly using Aspose.Note API");

        var document = GetOneNoteDocument(content, actionItems, cancellationToken);

        using var stream = new MemoryStream();
        document.Save(stream, SaveFormat.One);
        return stream.ToArray();
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportToPdfAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        this.logger.LogInformation("Exporting to PDF directly using Aspose.Note API");

        var document = GetOneNoteDocument(content, actionItems, cancellationToken);

        using var stream = new MemoryStream();
        document.Save(stream, SaveFormat.Pdf);
        return stream.ToArray();
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportToHtmlAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        this.logger.LogInformation("Exporting to HTML directly using Aspose.Note API");

        var document = GetOneNoteDocument(content, actionItems, cancellationToken);

        using var stream = new MemoryStream();
        document.Save(stream, SaveFormat.Html);
        return stream.ToArray();
    }

    private static Document GetOneNoteDocument(AnalyzedContent content, List<ActionItem> actionItems, CancellationToken cancellationToken)
    {
        var pageTitleStyle = CloneStyle(Heading1Style);
        pageTitleStyle.FontSize = 18;
        var pageDateStyle = CloneStyle(DefaultTextStyle);
        pageDateStyle.FontSize = 10;

        var title = new Title {
            TitleText = new RichText { Text = "Meeting Notes", ParagraphStyle = pageTitleStyle },
            TitleDate = new RichText { Text = DateTime.Now.ToString("D"), ParagraphStyle = pageDateStyle },
            TitleTime = new RichText { Text = DateTime.Now.ToString("t"), ParagraphStyle = pageDateStyle }
        };
        var page = new Page { Title = title };

        float currentVerticalOffset = 70.0f; // Initial Y offset for the first Outline
        const float horizontalOffset = 40.0f;
        const float outlineMaxWidth = 680.0f;
        const float spacingBetweenOutlines = 15.0f; // Spacing between top-level Outline blocks

        var summaryOutline = BuildTextBlock(content.Summary, "Summary", Heading1Style, DefaultTextStyle, horizontalOffset, ref currentVerticalOffset, outlineMaxWidth);
        if (summaryOutline != null)
        {
            page.AppendChildLast(summaryOutline);
        }

        currentVerticalOffset += spacingBetweenOutlines;

        if (content.KeyPoints.Any())
        {
            var keyPointsOutline = BuildListBlock(content.KeyPoints, "Key Points", Heading1Style, DefaultTextStyle, false, horizontalOffset, ref currentVerticalOffset, outlineMaxWidth);
            if (keyPointsOutline != null)
            {
                page.AppendChildLast(keyPointsOutline);
            }

            currentVerticalOffset += spacingBetweenOutlines;
        }

        if (content.Sections.Any())
        {
            var sectionsOutline = BuildSectionsBlock(content.Sections, "Meeting Content", Heading1Style, Heading2Style, DefaultTextStyle, horizontalOffset, ref currentVerticalOffset, outlineMaxWidth);
            if (sectionsOutline != null)
            {
                page.AppendChildLast(sectionsOutline);
            }

            currentVerticalOffset += spacingBetweenOutlines;
        }

        if (content.Decisions.Any())
        {
            var decisionsOutline = BuildListBlock(content.Decisions, "Decisions", Heading1Style, DefaultTextStyle, false, horizontalOffset, ref currentVerticalOffset, outlineMaxWidth);
            if (decisionsOutline != null)
            {
                page.AppendChildLast(decisionsOutline);
            }

            currentVerticalOffset += spacingBetweenOutlines;
        }

        if (content.Topics.Any())
        {
            var topicsOutline = BuildListBlock(content.Topics, "Topics Discussed", Heading1Style, DefaultTextStyle, false, horizontalOffset, ref currentVerticalOffset, outlineMaxWidth);
            if (topicsOutline != null)
            {
                page.AppendChildLast(topicsOutline);
            }

            currentVerticalOffset += spacingBetweenOutlines;
        }

        if (content.QASegments.Any())
        {
            var qaOutline = BuildQABlock(content.QASegments, "Questions and Answers", Heading1Style, Heading3Style, DefaultTextStyle, horizontalOffset, ref currentVerticalOffset, outlineMaxWidth);
            if (qaOutline != null)
            {
                page.AppendChildLast(qaOutline);
            }

            currentVerticalOffset += spacingBetweenOutlines;
        }

        if (actionItems.Any())
        {
            var actionsOutline = BuildListBlock(
                actionItems.Select(item => $"{item.Description} (Assignee: {item.Assignee}, Due: {item.DueDate}, Priority: {item.Priority})"),
                "Action Items",
                Heading1Style,
                DefaultTextStyle,
                false,
                horizontalOffset,
                ref currentVerticalOffset,
                outlineMaxWidth
            );

            if (actionsOutline != null)
            {
                page.AppendChildLast(actionsOutline);
            }

            currentVerticalOffset += spacingBetweenOutlines;
        }

        var transcriptOutline = BuildTextBlock(content.TranscribedText, "Full Transcript", Heading1Style, DefaultTextStyle, horizontalOffset, ref currentVerticalOffset, outlineMaxWidth);
        if (transcriptOutline != null)
        {
            page.AppendChildLast(transcriptOutline);
        }

        var oneNoteDocument = new Document();
        oneNoteDocument.AppendChildLast(page);
        return oneNoteDocument;
    }

    private static Outline? BuildTextBlock(string? text, string title, ParagraphStyle titleStyle, ParagraphStyle contentStyle, float hOffset, ref float vOffset, float maxWidth)
    {
        if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        var outline = new Outline { VerticalOffset = vOffset, HorizontalOffset = hOffset, MaxWidth = maxWidth };
        float internalVOffset = 0f;

        // Title
        if (!string.IsNullOrWhiteSpace(title))
        {
            var titleOe = new OutlineElement { }; // Padding after the title OE
            titleOe.AppendChildLast(new RichText { SpaceAfter = 5f, Text = title, ParagraphStyle = CloneStyle(titleStyle) });
            outline.AppendChildLast(titleOe);
            internalVOffset += 20f; // Approximate height of the title + spacing
        }

        // Text (each paragraph in its own OE)
        if (!string.IsNullOrWhiteSpace(text))
        {
            var paragraphs = text.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
            foreach (var para in paragraphs)
            {
                if (string.IsNullOrWhiteSpace(para) && paragraphs.Length > 1 && para != paragraphs.Last())
                {
                    continue;
                }

                var contentOe = new OutlineElement { };
                contentOe.AppendChildLast(new RichText { SpaceBefore = (title != null && para == paragraphs.First()) ? 0f : 2f, Text = para, ParagraphStyle = CloneStyle(contentStyle) });
                outline.AppendChildLast(contentOe);
            }

            // Estimate number of visual lines and adjust height accordingly
            double avgCharsPerLine = 115.0; // Empirical estimate for Arial 10pt and width ≈ 680pt
            int estimatedLineCount = (int) Math.Ceiling(text.Length / avgCharsPerLine);
            internalVOffset += estimatedLineCount * 14f; // 14f per line including padding
        }

        vOffset += internalVOffset; // Update the global vOffset based on the content height of this Outline
        return outline;
    }

    private static Outline? BuildListBlock(IEnumerable<string>? items, string title, ParagraphStyle titleStyle, ParagraphStyle itemStyle, bool isNumbered, float hOffset, ref float vOffset, float maxWidth)
    {
        if ((items == null || !items.Any(i => !string.IsNullOrWhiteSpace(i))) && string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        var outline = new Outline { VerticalOffset = vOffset, HorizontalOffset = hOffset, MaxWidth = maxWidth };
        float internalVOffset = 0f;

        if (!string.IsNullOrWhiteSpace(title))
        {
            var titleOe = new OutlineElement { };
            titleOe.AppendChildLast(new RichText { SpaceAfter = 5f, Text = title, ParagraphStyle = CloneStyle(titleStyle) });
            outline.AppendChildLast(titleOe);
            internalVOffset += 20f;
        }

        if (items != null && items.Any(i => !string.IsNullOrWhiteSpace(i)))
        {
            int listMarkerFontSize = itemStyle.FontSize ?? 10;
            string listFontName = itemStyle.FontName ?? "Arial";
            NumberList listSettings = isNumbered
                ? new NumberList("{0}.", NumberFormat.DecimalNumbers, listFontName, listMarkerFontSize)
                : new NumberList("\u2022", listFontName, listMarkerFontSize);

            bool firstItem = true;
            foreach (var itemText in items.Where(i => !string.IsNullOrWhiteSpace(i)))
            {
                var itemOe = new OutlineElement
                {
                    NumberList = listSettings
                };
                var richText = new RichText
                {
                    SpaceBefore = firstItem ? 2f : 1f,
                    SpaceAfter = 1f,
                    Text = itemText!.Trim(),
                    ParagraphStyle = CloneStyle(itemStyle)
                };

                itemOe.AppendChildLast(richText);
                outline.AppendChildLast(itemOe);

                firstItem = false;
            }

            // Fixed vertical offset per item regardless of text length
            int itemCount = items.Count(i => !string.IsNullOrWhiteSpace(i));
            internalVOffset += itemCount * 15f;
        }

        vOffset += internalVOffset;
        return outline;
    }

    private static Outline? BuildSectionsBlock(List<ContentSection>? sections, string mainTitle, ParagraphStyle mainTitleStyle, ParagraphStyle sectionTitleStyle, ParagraphStyle contentStyle, float hOffset, ref float vOffset, float maxWidth)
    {
        if ((sections == null || !sections.Any()) && string.IsNullOrWhiteSpace(mainTitle))
        {
            return null;
        }

        var outline = new Outline { VerticalOffset = vOffset, HorizontalOffset = hOffset, MaxWidth = maxWidth };
        float internalVOffset = 0f;

        if (!string.IsNullOrWhiteSpace(mainTitle))
        {
            var mainTitleOe = new OutlineElement { };
            mainTitleOe.AppendChildLast(new RichText { SpaceAfter = 8f, Text = mainTitle, ParagraphStyle = CloneStyle(mainTitleStyle) });
            outline.AppendChildLast(mainTitleOe);
            internalVOffset += 25f;
        }

        if (sections != null)
        {
            bool firstSection = true;
            foreach (var section in sections)
            {
                if (!string.IsNullOrWhiteSpace(section.Title))
                {
                    var sectionTitleOe = new OutlineElement { };
                    sectionTitleOe.AppendChildLast(new RichText { SpaceBefore = firstSection ? 2f : 8f, SpaceAfter = 3f, Text = section.Title, ParagraphStyle = CloneStyle(sectionTitleStyle) });
                    outline.AppendChildLast(sectionTitleOe);
                    internalVOffset += 20f;
                }
                if (!string.IsNullOrWhiteSpace(section.Content))
                {
                    var paragraphs = section.Content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
                    foreach (var para in paragraphs)
                    {
                        if (string.IsNullOrWhiteSpace(para) && paragraphs.Length > 1 && para != paragraphs.Last())
                        {
                            continue;
                        }

                        var contentOe = new OutlineElement { };
                        contentOe.AppendChildLast(new RichText { SpaceBefore = (para == paragraphs.First() && !string.IsNullOrWhiteSpace(section.Title)) ? 0f : 2f, Text = para, ParagraphStyle = CloneStyle(contentStyle) });
                        outline.AppendChildLast(contentOe);
                        internalVOffset += 15f;
                    }
                }
                firstSection = false;
            }
        }

        vOffset += internalVOffset;
        return outline;
    }

    private static Outline? BuildQABlock(List<QASegment>? qas, string mainTitle, ParagraphStyle mainTitleStyle, ParagraphStyle questionStyle, ParagraphStyle answerStyle, float hOffset, ref float vOffset, float maxWidth)
    {
        if ((qas == null || !qas.Any()) && string.IsNullOrWhiteSpace(mainTitle))
        {
            return null;
        }

        var outline = new Outline { VerticalOffset = vOffset, HorizontalOffset = hOffset, MaxWidth = maxWidth };
        float internalVOffset = 0f;

        if (!string.IsNullOrWhiteSpace(mainTitle))
        {
            var mainTitleOe = new OutlineElement { };
            mainTitleOe.AppendChildLast(new RichText { SpaceAfter = 8f, Text = mainTitle, ParagraphStyle = CloneStyle(mainTitleStyle) });
            outline.AppendChildLast(mainTitleOe);
            internalVOffset += 25f;
        }

        if (qas != null)
        {
            bool firstQA = true;
            foreach (var qa in qas)
            {
                if (!string.IsNullOrWhiteSpace(qa.Question))
                {
                    var questionOe = new OutlineElement { };
                    questionOe.AppendChildLast(new RichText { SpaceBefore = firstQA ? 2f : 8f, SpaceAfter = 3f, Text = "Q: " + qa.Question, ParagraphStyle = CloneStyle(questionStyle) });
                    outline.AppendChildLast(questionOe);
                    internalVOffset += 20f;
                }
                if (!string.IsNullOrWhiteSpace(qa.Answer))
                {
                    var paragraphs = qa.Answer.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
                    bool firstParaInAnswer = true;
                    foreach (var para in paragraphs)
                    {
                        if (string.IsNullOrWhiteSpace(para) && paragraphs.Length > 1 && para != paragraphs.Last())
                        {
                            continue;
                        }

                        var answerOe = new OutlineElement { };
                        answerOe.AppendChildLast(new RichText { SpaceBefore = (para == paragraphs.First() && !string.IsNullOrWhiteSpace(qa.Question)) ? 0f : 1f, Text = (firstParaInAnswer ? "A: " : string.Empty) + para, ParagraphStyle = CloneStyle(answerStyle) });
                        outline.AppendChildLast(answerOe);
                        internalVOffset += 15f;
                        firstParaInAnswer = false;
                    }
                }

                firstQA = false;
            }
        }

        vOffset += internalVOffset;
        return outline;
    }

    private static ParagraphStyle CloneStyle(ParagraphStyle original)
    {
        return new ParagraphStyle {
            FontColor = original.FontColor,
            FontName = original.FontName,
            FontSize = original.FontSize,
            IsBold = original.IsBold,
            IsItalic = original.IsItalic,
            IsUnderline = original.IsUnderline
        };
    }
}
