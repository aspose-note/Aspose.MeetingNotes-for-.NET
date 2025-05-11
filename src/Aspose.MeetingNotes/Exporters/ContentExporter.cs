using System.Text;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Models;
using Aspose.Note;
using Aspose.Note.Importing;
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

                    // Set Aspose.Html License (needed for Markdown to HTML conversion which is used by PDF/OneNote)
                    try
                    {
                        var htmlLicense = new Aspose.Html.License();
                        htmlLicense.SetLicense(options.AsposeLicensePath);
                        this.logger.LogDebug("Aspose.Html license set successfully.");
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Failed to set Aspose.Html license from path: {AsposeLicensePath}. Ensure the license is valid for Aspose.Html.", this.options.AsposeLicensePath);
                        throw new InvalidOperationException($"Failed to set Aspose.Html license. Please ensure the license file is valid for Aspose.Html. Path: {this.options.AsposeLicensePath}", ex);
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
                    resultText = await this.ExportToHtmlAsync(content, actionItems, cancellationToken);
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
        ArgumentNullException.ThrowIfNull(actionItems);
        this.logger.LogInformation("Exporting to OneNote via HTML import.");

        string htmlString = await ConvertMarkdownToHtmlAsync(content, actionItems, cancellationToken);
        byte[] htmlBytes = Encoding.UTF8.GetBytes(htmlString);

        using (var htmlMemoryStream = new MemoryStream(htmlBytes))
        {
            // Aspose.Note operations are generally synchronous
            var oneDocument = new Document();
            oneDocument.Import(htmlMemoryStream, new HtmlImportOptions());

            using (var oneStream = new MemoryStream())
            {
                oneDocument.Save(oneStream, SaveFormat.One);
                return oneStream.ToArray();
            }
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportToPdfAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(actionItems);
        this.logger.LogInformation("Exporting to PDF via HTML import.");

        string htmlString = await ConvertMarkdownToHtmlAsync(content, actionItems, cancellationToken);
        byte[] htmlBytes = Encoding.UTF8.GetBytes(htmlString);

        using (var htmlMemoryStream = new MemoryStream(htmlBytes))
        {
            // Aspose.Note operations are generally synchronous
            var oneDocument = new Document();
            oneDocument.Import(htmlMemoryStream, new HtmlImportOptions());

            using (var pdfStream = new MemoryStream())
            {
                oneDocument.Save(pdfStream, SaveFormat.Pdf);
                return pdfStream.ToArray();
            }
        }
    }

    /// <inheritdoc/>
    public async Task<string> ExportToHtmlAsync(
        AnalyzedContent content,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(actionItems);
        this.logger.LogInformation("Exporting to HTML format.");

        // The conversion already produces an HTML string.
        return await ConvertMarkdownToHtmlAsync(content, actionItems, cancellationToken);
    }

    /// <summary>
    /// Converts analyzed content and action items to an HTML string.
    /// This is a helper method used by other export formats.
    /// </summary>
    private async Task<string> ConvertMarkdownToHtmlAsync(
        AnalyzedContent analyzedContent,
        List<ActionItem> actionItems,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Converting content to HTML via Markdown using Aspose.HTML.");
        var markdownText = await ExportToMarkdownAsync(analyzedContent, actionItems, cancellationToken);
        if (string.IsNullOrWhiteSpace(markdownText))
        {
            this.logger.LogWarning("Markdown content is empty. Resulting HTML will be empty.");
            return string.Empty;
        }

        string htmlText;
        // Using temporary files for conversion, as Aspose.Html.Converter.ConvertMarkdown often uses file paths.
        var tempMdPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".md");
        var tempHtmlPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".html");

        try
        {
            // File operations can be wrapped in Task.Run if they become a bottleneck,
            // but for typical scenarios, direct calls within an async method are acceptable for clarity.
            await File.WriteAllTextAsync(tempMdPath, markdownText, Encoding.UTF8, cancellationToken);
            this.logger.LogDebug("Temporary Markdown file created: {TempMdPath}", tempMdPath);

            // Aspose.Html.Converters.Converter.ConvertMarkdown is a synchronous method.
            // If this becomes a performance issue in highly concurrent scenarios,
            // it could be wrapped with Task.Run, but that adds complexity.
            Aspose.Html.Converters.Converter.ConvertMarkdown(tempMdPath, tempHtmlPath);
            this.logger.LogDebug("Markdown file '{TempMdPath}' converted to HTML file '{TempHtmlPath}'.", tempMdPath, tempHtmlPath);

            htmlText = await File.ReadAllTextAsync(tempHtmlPath, Encoding.UTF8, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error during Aspose.HTML Markdown to HTML conversion using temporary files.");
            throw new InvalidOperationException("Failed to convert Markdown to HTML via temporary files.", ex);
        }
        finally
        {
            if (File.Exists(tempMdPath))
            {
                try
                {
                    File.Delete(tempMdPath);
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, "Failed to delete temp MD file: {TempMdPath}", tempMdPath);
                }
            }

            if (File.Exists(tempHtmlPath))
            {
                try
                {
                    File.Delete(tempHtmlPath);
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, "Failed to delete temp HTML file: {TempHtmlPath}", tempHtmlPath);
                }
            }
        }

        this.logger.LogInformation("Markdown to HTML conversion successful.");
        return htmlText;
    }
}
