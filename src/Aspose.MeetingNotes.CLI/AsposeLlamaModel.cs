using System.Text.Json;

using Aspose.LLaMACpp;
using Aspose.LLaMACpp.Endpoints.Models;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;

using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.CLI;

/// <summary>
/// Implements the <see cref="IAIModel"/> interface using the Aspose.LLaMACpp client
/// to interact with a LLaMA C++ API server (like llama.cpp server).
/// </summary>
public class AsposeLlamaModel : IAIModel
{
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

IMPORTANT: Respond *only* with the valid JSON object, without any surrounding text or markdown formatting.

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

IMPORTANT: Respond *only* with the valid JSON array, without any surrounding text or markdown formatting.

Meeting transcript:
{0}";

    private static readonly JsonSerializerOptions jsonOptions;
    private readonly LLaMACppClient client;
    private readonly ILogger<AsposeLlamaModel> logger;
    private readonly string modelName = "default";

    /// <summary>
    /// Initializes static members of the <see cref="AsposeLlamaModel"/> class.
    /// </summary>
    static AsposeLlamaModel()
    {
        jsonOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsposeLlamaModel"/> class.
    /// </summary>
    /// <param name="baseUrl">The base URL of the LLaMA C++ API server (e.g., "http://localhost:8080/v1").</param>
    /// <param name="logger">The logger instance for logging operations.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="baseUrl"/> is null or whitespace.</exception>
    public AsposeLlamaModel(string baseUrl, ILogger<AsposeLlamaModel> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentNullException.ThrowIfNull(logger);

        this.logger = logger;
        this.client = new LLaMACppClient(baseUrl);
        this.logger.LogInformation("AsposeLlamaModel initialized with base URL: {BaseUrl}", baseUrl);
    }

    /// <inheritdoc/>
    public async Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        string prompt = string.Format(AnalyzeContentPromptFormat, text);

        try
        {
            this.logger.LogInformation("Sending content analysis request to LLaMA model '{ModelName}'", this.modelName);
            string responseContent = await this.ProcessTextAsync(prompt, cancellationToken);
            this.logger.LogDebug("Raw AI response for analysis: {Response}", responseContent);

            string cleanedResponse = CleanAiJsonResponse(responseContent, this.logger);
            var initialResult = JsonSerializer.Deserialize<AnalyzedContent>(cleanedResponse, jsonOptions);

            if (initialResult == null)
            {
                this.logger.LogError("Failed to deserialize LLaMA analysis response to AnalyzedContent. Cleaned Response: {Response}", cleanedResponse);
                throw new AIModelException("Failed to parse AI response for content analysis. Response was empty or invalid JSON after cleaning");
            }

            this.logger.LogInformation("Successfully parsed LLaMA analysis response. Post-processing results...");

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
            this.logger.LogError(jsonEx, "Failed to deserialize JSON response from LLaMA during content analysis");
            throw new AIModelException("Failed to parse JSON response from LLaMA for content analysis", jsonEx);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
        {
            this.logger.LogError(ex, "An unexpected error occurred during LLaMA content analysis");
            throw new AIModelException("An unexpected error occurred during LLaMA content analysis", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        string prompt = string.Format(ExtractActionItemsPromptFormat, text);

        try
        {
            this.logger.LogInformation("Sending action item extraction request to LLaMA model '{ModelName}'", this.modelName);
            string responseContent = await this.ProcessTextAsync(prompt, cancellationToken);
            this.logger.LogDebug("Raw AI response for action items: {Response}", responseContent);

            string cleanedResponse = CleanAiJsonResponse(responseContent, this.logger);
            var result = JsonSerializer.Deserialize<List<ActionItem>>(cleanedResponse, jsonOptions);

            if (result == null)
            {
                this.logger.LogWarning("LLaMA response for action items was null or empty JSON array after cleaning. Assuming no action items found. Cleaned Response: {Response}", cleanedResponse);
                return [];
            }

            this.logger.LogInformation("Successfully parsed LLaMA action items response. Found initially {Count} items", result.Count);

            // Clean up and validate action items
            // Using 'with' expression for records to create modified copies
            return result
                .Where(item => item != null && !string.IsNullOrWhiteSpace(item.Description))
                .Select(item => item with {
                    Description = item.Description.Trim(),
                    Assignee = string.IsNullOrWhiteSpace(item.Assignee) ? "Unassigned" : item.Assignee.Trim(),
                    DueDate = string.IsNullOrWhiteSpace(item.DueDate) ? "Not specified" : item.DueDate.Trim(),
                    Priority = ValidatePriority(item.Priority)
                })
                .ToList();
        }
        catch (JsonException jsonEx)
        {
            this.logger.LogError(jsonEx, "Failed to deserialize JSON response from LLaMA during action item extraction");
            throw new AIModelException("Failed to parse JSON response from LLaMA for action item extraction", jsonEx);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
        {
            this.logger.LogError(ex, "An unexpected error occurred during LLaMA action item extraction");
            throw new AIModelException("An unexpected error occurred during LLaMA action item extraction", ex);
        }
    }

    /// <summary>
    /// Sends a text prompt to the LLaMA C++ API server and retrieves the response content.
    /// </summary>
    /// <param name="text">The prompt text to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response content string.</returns>
    /// <exception cref="AIModelException">Thrown if the API call fails or returns an unexpected result.</exception>
    private async Task<string> ProcessTextAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatClient = this.client.GetChatCompletionsClient();
            var request = new ChatCompletionRequest {
                Model = this.modelName,
                Messages =
                [
                    new ChatCompletionMessage { Role = "user", Content = text }
                ]
            };

            this.logger.LogDebug("Sending ChatCompletionRequest to LLaMA server");
            var response = await chatClient.CreateChatCompletionAsync(request, cancellationToken);

            // Validate response structure (example validation)
            if (response?.Choices == null || !response.Choices.Any())
            {
                this.logger.LogWarning("LLaMA response contained no choices");
                throw new AIModelException("LLaMA response structure was invalid: missing choices.");
            }
            if (response.Choices[0].Message == null)
            {
                this.logger.LogWarning("LLaMA response choice contained no message");
                throw new AIModelException("LLaMA response structure was invalid: missing message in choice.");
            }

            string? content = response.Choices[0].Message.Content;

            if (string.IsNullOrEmpty(content))
            {
                this.logger.LogWarning("LLaMA response message content was null or empty");
                return string.Empty;
            }

            return content;
        }
        // Catch specific exceptions from Aspose.LLaMACpp if they are documented/known
        // catch (AsposeSpecificException ex) ...
        catch (Exception ex) when (ex is not OperationCanceledException) // Catch general errors from the client library
        {
            this.logger.LogError(ex, "Error processing text with LLaMA model via Aspose.LLaMACpp client");
            throw new AIModelException($"Failed to process text with Aspose.LLaMACpp client: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Attempts to clean common non-JSON artifacts (like markdown code fences) from an AI response string.
    /// Primarily useful if the AI doesn't strictly follow instructions to return only JSON.
    /// </summary>
    /// <param name="rawResponse">The raw string response from the AI.</param>
    /// <param name="logger">Logger instance for warnings.</param>
    /// <returns>The potentially cleaned string, aiming for valid JSON.</returns>
    private static string CleanAiJsonResponse(string rawResponse, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return string.Empty;
        }

        string cleanedResponse = rawResponse.Trim();
        bool changed = false;

        // Simple check for markdown code fences (can be adapted)
        if (cleanedResponse.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
        {
            cleanedResponse = cleanedResponse.Substring(7).TrimStart();
            changed = true;
        }
        else if (cleanedResponse.StartsWith("```")) // Handle case without 'json' specifier
        {
            cleanedResponse = cleanedResponse.Substring(3).TrimStart();
            changed = true;
        }

        if (cleanedResponse.EndsWith("```"))
        {
            cleanedResponse = cleanedResponse.Substring(0, cleanedResponse.Length - 3).TrimEnd();
            changed = true;
        }

        // Optional: More robust search for '{' or '[' and '}' or ']' as fallback
        // This might be needed if the AI adds explanatory text before/after JSON
        int jsonStartIndex = cleanedResponse.IndexOfAny(['{', '[']);
        int jsonEndIndex = cleanedResponse.LastIndexOfAny(['}', ']']);

        if (jsonStartIndex > 0 || (jsonEndIndex >= 0 && jsonEndIndex < cleanedResponse.Length - 1))
        {
            // Only attempt substring if we found likely JSON boundaries inside surrounding text
            if (jsonStartIndex >= 0 && jsonEndIndex >= jsonStartIndex)
            {
                string potentiallyBetterJson = cleanedResponse.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1);
                // Basic sanity check - does it look like JSON? (Starts/ends correctly)
                if ((potentiallyBetterJson.StartsWith('{') && potentiallyBetterJson.EndsWith('}')) ||
                    (potentiallyBetterJson.StartsWith('[') && potentiallyBetterJson.EndsWith(']')))
                {
                    if (cleanedResponse != potentiallyBetterJson)
                    {
                        logger.LogWarning("Attempting aggressive JSON cleaning by extracting content between first '{{'/'[' and last '}}'/']'");
                        cleanedResponse = potentiallyBetterJson;
                        changed = true;
                    }
                }
            }
        }


        if (changed)
        {
            logger.LogInformation("Performed cleaning on AI JSON response");
            logger.LogDebug("Cleaned response: {CleanedResponse}", cleanedResponse);
        }

        return cleanedResponse;
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
}
