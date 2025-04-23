using System.Text;
using System.Text.Json;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.AIIntegration
{
    /// <summary>
    /// ChatGPT implementation of AI model integration.
    /// </summary>
    public class ChatGPTModel : IAIModel
    {
        private const string ApiEndpoint = "https://api.openai.com/v1/chat/completions";

        private readonly HttpClient httpClient;
        private readonly ChatGPTOptions chatgptOptions;
        private readonly ILogger<ChatGPTModel> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatGPTModel"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making API requests.</param>
        /// <param name="options">The meeting notes options containing configuration.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public ChatGPTModel(HttpClient httpClient, MeetingNotesOptions options, ILogger<ChatGPTModel> logger)
        {
            this.httpClient = httpClient;
            this.chatgptOptions = (ChatGPTOptions)options.AIModel;
            this.logger = logger;

            if (string.IsNullOrEmpty(this.chatgptOptions.ApiKey))
            {
                throw new ArgumentException("ChatGPT API key is required", nameof(options));
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.chatgptOptions.ApiKey}");
        }

        /// <summary>
        /// Analyzes the provided text content using the ChatGPT API.
        /// </summary>
        /// <param name="text">The text content to analyze.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the AI analysis result.</returns>
        public async Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                var prompt = $@"Analyze the following meeting transcript and provide:
1. A concise summary (max 200 words)
2. Key points discussed
3. Main topics covered

Transcript:
{text}";

                var response = await SendChatGPTRequestAsync(prompt, cancellationToken);
                var result = ParseAnalysisResponse(response);
                result.TranscribedText = text;

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error analyzing content with ChatGPT");
                throw;
            }
        }

        /// <summary>
        /// Extracts action items from the provided text using the ChatGPT API.
        /// </summary>
        /// <param name="text">The text content to extract action items from.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the list of extracted action items.</returns>
        public async Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                var prompt = $@"Extract action items from the following meeting transcript. For each action item, identify:
1. The task description
2. The assignee (if mentioned)
3. The due date (if mentioned)

Format the response as a JSON array of objects with properties: description, assignee, dueDate.

Transcript:
{text}";

                var response = await SendChatGPTRequestAsync(prompt, cancellationToken);
                var actionItems = ParseActionItemsResponse(response);

                return actionItems;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error extracting action items with ChatGPT");
                throw;
            }
        }

        private async Task<string> SendChatGPTRequestAsync(string prompt, CancellationToken cancellationToken)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(ApiEndpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseObject = JsonSerializer.Deserialize<ChatGPTResponse>(responseContent);

            return responseObject?.Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty;
        }

        private static AnalyzedContent ParseAnalysisResponse(string response)
        {
            // Simple parsing logic - in a real implementation, you might want to use more sophisticated parsing
            var lines = response.Split('\n');
            var summary = lines.FirstOrDefault(l => l.StartsWith("Summary:"))?.Replace("Summary:", string.Empty).Trim() ?? string.Empty;
            var keyPoints = lines.Where(l => l.StartsWith("-")).Select(l => l.TrimStart('-').Trim()).ToList();
            var topics = lines.Where(l => l.StartsWith("Topic:")).Select(l => l.Replace("Topic:", string.Empty).Trim()).ToList();

            return new AnalyzedContent
            {
                Summary = summary,
                KeyPoints = keyPoints,
                Topics = topics
            };
        }

        private List<ActionItem> ParseActionItemsResponse(string response)
        {
            try
            {
                return JsonSerializer.Deserialize<List<ActionItem>>(response) ?? new List<ActionItem>();
            }
            catch (JsonException)
            {
                logger.LogWarning("Failed to parse action items as JSON, falling back to text parsing");
                return ParseActionItemsFromText(response);
            }
        }

        private static List<ActionItem> ParseActionItemsFromText(string response)
        {
            var actionItems = new List<ActionItem>();
            var lines = response.Split('\n');

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split('|').Select(p => p.Trim()).ToArray();
                if (parts.Length >= 1)
                {
                    actionItems.Add(new ActionItem
                    {
                        Description = parts[0],
                        Assignee = parts.Length > 1 ? parts[1] : null,
                        DueDate = parts[2]
                    });
                }
            }

            return actionItems;
        }

        private class ChatGPTResponse
        {
            public List<Choice> Choices { get; set; } = new();
        }

        private class Choice
        {
            public Message Message { get; set; } = new();
        }

        private class Message
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
