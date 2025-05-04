using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;

using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.AIIntegration;

/// <summary>
/// Implementation of <see cref="IAIModel"/> using the OpenAI ChatGPT API.
/// </summary>
public class ChatGPTModel : IAIModel
{
    private const string DefaultApiEndpoint = "https://api.openai.com/v1/chat/completions";
    private const string DefaultModel = "gpt-3.5-turbo";

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

    private readonly HttpClient httpClient;
    private readonly ChatGPTOptions chatgptOptions;
    private readonly ILogger<ChatGPTModel> logger;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGPTModel"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client for making API requests.</param>
    /// <param name="options">The meeting notes options containing configuration for the AI model.</param>
    /// <param name="logger">The logger instance for logging operations.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpClient"/>, <paramref name="options"/>, or <paramref name="logger"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the AI model options are not <see cref="ChatGPTOptions"/>, or if the API key is missing.</exception>
    public ChatGPTModel(HttpClient httpClient, MeetingNotesOptions options, ILogger<ChatGPTModel> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        // Safely cast AIModel options and validate
        if (options.AIModel is not ChatGPTOptions specificOptions)
        {
            throw new ArgumentException($"Configuration error: AIModel options must be of type {nameof(ChatGPTOptions)} for {nameof(ChatGPTModel)}.", nameof(options));
        }
        if (string.IsNullOrWhiteSpace(specificOptions.ApiKey))
        {
            throw new ArgumentException("ChatGPT API key is required in ChatGPTOptions and cannot be empty.", nameof(options));
        }

        this.httpClient = httpClient;
        this.chatgptOptions = specificOptions;
        this.logger = logger;

        // Configure HttpClient (consider doing this via IHttpClientFactory configuration if applicable)
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.chatgptOptions.ApiKey);
        this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Setup JsonSerializerOptions
        this.jsonSerializerOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };
    }

    /// <inheritdoc/>
    public async Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        string prompt = string.Format(AnalyzeContentPromptFormat, text);
        string model = this.chatgptOptions.Model ?? DefaultModel;
        float temperature = this.chatgptOptions.Temperature;

        try
        {
            this.logger.LogInformation("Sending content analysis request to ChatGPT model {Model}", model);
            string responseContent = await this.SendChatGPTRequestAsync(prompt, model, temperature, cancellationToken);
            this.logger.LogDebug("Raw AI response for analysis: {Response}", responseContent);

            // Attempt to directly deserialize the expected JSON structure
            var initialResult = JsonSerializer.Deserialize<AnalyzedContent>(responseContent, this.jsonSerializerOptions);

            if (initialResult == null)
            {
                this.logger.LogError("Failed to deserialize ChatGPT analysis response to AnalyzedContent. Response: {Response}", responseContent);
                throw new AIModelException("Failed to parse AI response for content analysis. Response was empty or invalid JSON");
            }

            this.logger.LogInformation("Successfully parsed ChatGPT analysis response. Post-processing results...");

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
            this.logger.LogError(jsonEx, "Failed to deserialize JSON response from ChatGPT during content analysis");
            throw new AIModelException("Failed to parse JSON response from ChatGPT for content analysis", jsonEx);
        }
        catch (HttpRequestException httpEx)
        {
            this.logger.LogError(httpEx, "HTTP request failed during content analysis with ChatGPT");
            throw new AIModelException("HTTP request failed while communicating with ChatGPT for content analysis", httpEx);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
        {
            this.logger.LogError(ex, "An unexpected error occurred during ChatGPT content analysis");
            throw new AIModelException("An unexpected error occurred during ChatGPT content analysis", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        string prompt = string.Format(ExtractActionItemsPromptFormat, text);
        string model = this.chatgptOptions.Model ?? DefaultModel;
        float temperature = this.chatgptOptions.Temperature;

        try
        {
            this.logger.LogInformation("Sending action item extraction request to ChatGPT model {Model}", model);
            string responseContent = await this.SendChatGPTRequestAsync(prompt, model, temperature, cancellationToken);

            this.logger.LogDebug("Raw AI response for action items: {Response}", responseContent);

            // Attempt to directly deserialize the expected JSON array
            var result = JsonSerializer.Deserialize<List<ActionItem>>(responseContent, this.jsonSerializerOptions);

            if (result == null)
            {
                this.logger.LogWarning("ChatGPT response for action items was null or empty JSON array. Assuming no action items found. Response: {Response}", responseContent);
                return []; // Return empty list if deserialize result is null
            }

            this.logger.LogInformation("Successfully parsed ChatGPT action items response. Found initially {Count} items", result.Count);

            // Clean up and validate action items
            return result
                .Where(item => !string.IsNullOrWhiteSpace(item.Description)) // Ensure description exists
                .Select(item => new ActionItem {
                    Description = item.Description.Trim(),
                    Assignee = string.IsNullOrWhiteSpace(item.Assignee) ? "Unassigned" : item.Assignee.Trim(),
                    DueDate = string.IsNullOrWhiteSpace(item.DueDate) ? "Not specified" : item.DueDate.Trim(),
                    Priority = ValidatePriority(item.Priority) // Use shared or local helper
                })
                .ToList();
        }
        catch (JsonException jsonEx)
        {
            this.logger.LogError(jsonEx, "Failed to deserialize JSON response from ChatGPT during action item extraction");
            throw new AIModelException("Failed to parse JSON response from ChatGPT for action item extraction", jsonEx);
        }
        catch (HttpRequestException httpEx)
        {
            this.logger.LogError(httpEx, "HTTP request failed during action item extraction with ChatGPT");
            throw new AIModelException("HTTP request failed while communicating with ChatGPT for action item extraction", httpEx);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not AIModelException)
        {
            this.logger.LogError(ex, "An unexpected error occurred during ChatGPT action item extraction");
            throw new AIModelException("An unexpected error occurred during ChatGPT action item extraction", ex);
        }
    }

    /// <summary>
    /// Sends a request to the ChatGPT API.
    /// </summary>
    /// <param name="prompt">The user prompt.</param>
    /// <param name="model">The specific model to use.</param>
    /// <param name="temperature">The generation temperature.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The content string from the response.</returns>
    /// <exception cref="HttpRequestException">Thrown if the API request fails.</exception>
    /// <exception cref="AIModelException">Thrown if the API response is invalid or missing expected content.</exception>
    private async Task<string> SendChatGPTRequestAsync(string prompt, string model, float temperature, CancellationToken cancellationToken)
    {
        string endpoint = this.chatgptOptions.ApiEndpoint ?? DefaultApiEndpoint;
        var requestPayload = new ChatGPTRequest {
            Model = model,
            Messages =
            [
                new RequestMessage { Role = "user", Content = prompt }
            ],
            Temperature = temperature
        };

        this.logger.LogDebug("Sending request to ChatGPT endpoint: {Endpoint}, Model: {Model}", endpoint, model);

        // Use PostAsJsonAsync for cleaner serialization
        HttpResponseMessage response = await this.httpClient.PostAsJsonAsync(endpoint, requestPayload, this.jsonSerializerOptions, cancellationToken);

        // Check for HTTP errors (4xx, 5xx)
        response.EnsureSuccessStatusCode(); // Throws HttpRequestException on failure

        // Use ReadFromJsonAsync for cleaner deserialization
        var responseData = await response.Content.ReadFromJsonAsync<ChatGPTResponse>(this.jsonSerializerOptions, cancellationToken);

        // Extract content and handle potential nulls
        string? content = responseData?.Choices?.FirstOrDefault()?.Message?.Content;

        if (string.IsNullOrEmpty(content))
        {
            this.logger.LogWarning("ChatGPT response was successful but contained no content or failed to deserialize correctly");

            // Consider logging the raw response body here for debugging if possible without exposing sensitive info
            throw new AIModelException("ChatGPT response did not contain valid content");
        }

        return content;
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

    private record ChatGPTRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; init; } = DefaultModel;

        [JsonPropertyName("messages")]
        public required List<RequestMessage> Messages { get; init; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; init; } = 0.7f;
    }

    private record RequestMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; init; } = "user";

        [JsonPropertyName("content")]
        public required string Content { get; init; }
    }

    private record ChatGPTResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; init; }
    }

    private record Choice
    {
        [JsonPropertyName("message")]
        public ResponseMessage? Message { get; init; }
    }

    private record ResponseMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; init; }
    }
}
