using System.Text;
using System.Text.Json;

using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;

using LLama;
using LLama.Common;
using LLama.Sampling;

using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.AIIntegration;

/// <summary>
/// Implementation of <see cref="IAIModel"/> using a local LLama model via the LLamaSharp library.
/// </summary>
public class LLamaModel : IAIModel, IDisposable // Implement IDisposable for model resources
{
    // Constants for prompts
    private const string AnalyzeContentPromptFormat = @"Analyze the following meeting transcript and provide:
1. A brief summary (max 200 words)
2. Key discussion points (up to 5)
3. Main decisions made
4. Questions and answers identified
5. Important topics covered

Format the response strictly as a JSON object with the following structure:
{{
    ""summary"": ""string"",
    ""keyPoints"": [""string""],
    ""decisions"": [""string""],
    ""qaSegments"": [{{""question"": ""string"", ""answer"": ""string""}}],
    ""topics"": [""string""]
}}

IMPORTANT: Respond *only* with the valid JSON object, without any surrounding text or formatting.

Meeting transcript:
{0}";

    private const string ExtractActionItemsPromptFormat = @"Extract action items from the following meeting transcript. Look for:
- Tasks that need to be done
- Assignments to specific people
- Deadlines or due dates
- Priority indicators

Format the response strictly as a JSON array of objects with the following structure:
[{{
    ""description"": ""string"",
    ""assignee"": ""string"",
    ""dueDate"": ""string"",
    ""priority"": ""string""
}}]

For each action item:
- description: Clear description of what needs to be done
- assignee: Name of person assigned (or ""Unassigned"")
- dueDate: Due date if mentioned (or ""Not specified"")
- priority: High/Medium/Low based on context (default to ""Medium"")

IMPORTANT: Respond *only* with the valid JSON array, without any surrounding text or formatting.

Meeting transcript:
{0}";

    private readonly LLamaOptions llamaOptions;
    private readonly ILogger<LLamaModel> logger;
    private readonly LLamaWeights model;
    private readonly LLamaContext context;
    private readonly ChatSession session;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="LLamaModel"/> class.
    /// Loads the LLama model and sets up the execution context.
    /// </summary>
    /// <param name="options">The meeting notes options containing configuration for the AI model.</param>
    /// <param name="logger">The logger instance for logging operations.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> or <paramref name="logger"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the AI model options are not <see cref="LLamaOptions"/>, or if the model path is missing.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the LLama model file specified in options cannot be found.</exception>
    /// <exception cref="AIModelException">Thrown if there is an error loading the LLama model or context.</exception>
    public LLamaModel(MeetingNotesOptions options, ILogger<LLamaModel> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        if (options.AIModel is not LLamaOptions specificOptions)
        {
            throw new ArgumentException($"Configuration error: AIModel options must be of type {nameof(LLamaOptions)} for {nameof(LLamaModel)}.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(specificOptions.ModelPath))
        {
            throw new ArgumentException("LLama model path is required in LLamaOptions and cannot be empty.", nameof(options));
        }

        if (!File.Exists(specificOptions.ModelPath))
        {
            throw new FileNotFoundException("LLama model file not found at the specified path.", specificOptions.ModelPath);
        }

        this.llamaOptions = specificOptions;
        this.logger = logger;

        this.logger.LogInformation("Initializing LLama model from path: {ModelPath}", this.llamaOptions.ModelPath);

        try
        {
            var parameters = new ModelParams(this.llamaOptions.ModelPath)
            {
                ContextSize = this.llamaOptions.ContextSize ?? 4096,
                // GpuLayerCount seems obsolete in newer LLamaSharp, check documentation for current GPU offloading options if needed.
                // Example for potential new way (check LLamaSharp docs):
                // MainGpu = 0, // Specify main GPU index
                // TensorSplits = null, // Configure tensor splitting if needed
            };

            this.model = LLamaWeights.LoadFromFile(parameters);
            this.context = this.model.CreateContext(parameters);

            // Use StatefulExecutor or ChatSession based on interaction style
            // ChatSession manages history automatically
            var executor = new StatelessExecutor(this.model, parameters); // Or InteractiveExecutor if preferred
            this.session = new ChatSession(executor); // Use ChatSession for simplicity

            // Configure output filtering (optional, helps clean up model self-correction/role prefixes)
            this.session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(["User:", "Assistant:"], redundancyLength: 8));

            this.logger.LogInformation("LLama model and context initialized successfully");
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ex, "Failed to load LLama model or create context from path: {ModelPath}", this.llamaOptions.ModelPath);
            // Clean up partially initialized resources if possible (model might need disposal)
            this.model?.Dispose(); // Dispose model if it was loaded before context creation failed
            throw new AIModelException($"Failed to initialize LLama model from '{this.llamaOptions.ModelPath}'", ex);
        }

        // Setup JsonSerializerOptions
        this.jsonSerializerOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
        };
    }

    /// <inheritdoc/>
    public async Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        string prompt = string.Format(AnalyzeContentPromptFormat, text);
        var inferenceParams = new InferenceParams()
        {
            MaxTokens = -1,
            AntiPrompts = ["User:", "\n"],
            SamplingPipeline = new DefaultSamplingPipeline { Temperature = this.llamaOptions.Temperature }
        };

        try
        {
            this.logger.LogInformation("Sending content analysis request to LLama model");
            string responseContent = await this.InferAsync(prompt, inferenceParams, cancellationToken);
            this.logger.LogDebug("Raw AI response for analysis: {Response}", responseContent);

            // Attempt to deserialize directly, assuming model follows instructions
            var initialResult = JsonSerializer.Deserialize<AnalyzedContent>(responseContent, this.jsonSerializerOptions);

            if (initialResult == null)
            {
                this.logger.LogError("Failed to deserialize LLama analysis response to AnalyzedContent. Response: {Response}", responseContent);
                throw new AIModelException("Failed to parse AI response for content analysis. Response was empty or invalid JSON");
            }

            this.logger.LogInformation("Successfully parsed LLama analysis response. Post-processing results...");

            // Create a new record instance using a 'with' expression, applying cleaning logic
            AnalyzedContent finalResult = initialResult with {
                Summary = initialResult.Summary?.Trim() ?? string.Empty,
                KeyPoints = initialResult.KeyPoints?
                               .Where(p => !string.IsNullOrWhiteSpace(p))
                               .Select(p => p.Trim())
                               .ToList() ?? [],
                Decisions = initialResult.Decisions?
                               .Where(d => !string.IsNullOrWhiteSpace(d))
                               .Select(d => d.Trim())
                               .ToList() ?? [],
                Topics = initialResult.Topics?
                              .Where(t => !string.IsNullOrWhiteSpace(t))
                              .Select(t => t.Trim())
                              .ToList() ?? [],
                QASegments = initialResult.QASegments?
                                .Where(qa => qa != null && !string.IsNullOrWhiteSpace(qa.Question) && !string.IsNullOrWhiteSpace(qa.Answer))
                                .Select(qa => qa with { Question = qa.Question.Trim(), Answer = qa.Answer.Trim() })
                                .ToList() ?? [],
                TranscribedText = text
            };

            return finalResult;
        }
        catch (JsonException jsonEx)
        {
            this.logger.LogError(jsonEx, "Failed to deserialize JSON response from LLama during content analysis");
            throw new AIModelException("Failed to parse JSON response from LLama for content analysis", jsonEx);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
        {
            this.logger.LogError(ex, "An unexpected error occurred during LLama content analysis");
            throw new AIModelException("An unexpected error occurred during LLama content analysis", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        string prompt = string.Format(ExtractActionItemsPromptFormat, text);
        var inferenceParams = new InferenceParams()
        {
            MaxTokens = -1,
            AntiPrompts = ["User:", "\n"],
            SamplingPipeline = new DefaultSamplingPipeline { Temperature = this.llamaOptions.Temperature }
        };

        try
        {
            this.logger.LogInformation("Sending action item extraction request to LLama model");
            string responseContent = await this.InferAsync(prompt, inferenceParams, cancellationToken);

            this.logger.LogDebug("Raw AI response for action items: {Response}", responseContent);

            // Attempt to deserialize directly
            var result = JsonSerializer.Deserialize<List<ActionItem>>(responseContent, this.jsonSerializerOptions);

            if (result == null)
            {
                this.logger.LogWarning("LLama response for action items was null or empty JSON array. Assuming no action items found. Response: {Response}", responseContent);
                return [];
            }

            this.logger.LogInformation("Successfully parsed LLama action items response. Found initially {Count} items", result.Count);

            // Clean up and validate action items
            return result
                .Where(item => !string.IsNullOrWhiteSpace(item.Description))
                .Select(item => new ActionItem {
                    Description = item.Description.Trim(),
                    Assignee = string.IsNullOrWhiteSpace(item.Assignee) ? "Unassigned" : item.Assignee.Trim(),
                    DueDate = string.IsNullOrWhiteSpace(item.DueDate) ? "Not specified" : item.DueDate.Trim(),
                    Priority = ValidatePriority(item.Priority)
                })
                .ToList();
        }
        catch (JsonException jsonEx)
        {
            this.logger.LogError(jsonEx, "Failed to deserialize JSON response from LLama during action item extraction");
            throw new AIModelException("Failed to parse JSON response from LLama for action item extraction", jsonEx);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
        {
            this.logger.LogError(ex, "An unexpected error occurred during LLama action item extraction");
            throw new AIModelException("An unexpected error occurred during LLama action item extraction", ex);
        }
    }

    /// <summary>
    /// Performs inference using the configured LLama session.
    /// </summary>
    /// <param name="prompt">The input prompt.</param>
    /// <param name="inferenceParams">Inference parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated text response.</returns>
    private async Task<string> InferAsync(string prompt, InferenceParams inferenceParams, CancellationToken cancellationToken)
    {
        var responseBuilder = new StringBuilder();

        // Use ChatAsync which handles history automatically
        await foreach (var textChunk in this.session.ChatAsync(
            new ChatHistory.Message(AuthorRole.User, prompt), // Add prompt as user message
            inferenceParams,
            cancellationToken))
        {
            responseBuilder.Append(textChunk);
        }
        return responseBuilder.ToString().Trim(); // Trim final whitespace
    }

    /// <summary>
    /// Validates and normalizes the priority string.
    /// </summary>
    /// <param name="priority">The input priority string.</param>
    /// <returns>A normalized priority ("High", "Medium", "Low"). Defaults to "Medium".</returns>
    private static string ValidatePriority(string? priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            return "Medium";
        }

        return priority.Trim().ToLowerInvariant() switch {
            "high" => "High",
            "medium" => "Medium",
            "low" => "Low",
            _ => "Medium"
        };
    }

    // --- IDisposable Implementation ---

    /// <summary>
    /// Releases the unmanaged resources used by the LLamaModel, specifically the loaded model weights and context.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                this.logger.LogDebug("Disposing LLamaContext");
                this.context?.Dispose(); // Dispose context first
                this.logger.LogDebug("Disposing LLamaWeights");
                this.model?.Dispose(); // Then dispose model weights
                // Note: ChatSession and Executor might not need explicit disposal if they don't own resources beyond context/model
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer (none here)
            // Set large fields to null
            this.disposedValue = true;
            this.logger.LogInformation("LLamaModel disposed");
        }
    }

    // Uncomment if a finalizer (~LLamaModel()) is needed, which is generally only if you directly handle unmanaged resources.
    // LLamaSharp handles its own unmanaged resources via its Dispose methods.
    // ~LLamaModel()
    // {
    //     Dispose(disposing: false);
    // }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
