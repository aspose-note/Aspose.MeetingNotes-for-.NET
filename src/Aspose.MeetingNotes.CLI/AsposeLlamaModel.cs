using Aspose.LLaMACpp;
using Aspose.LLaMACpp.Endpoints.Models;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Aspose.MeetingNotes.CLI;

/// <summary>
/// Implementation of IAIModel using Aspose.LLApi for LLaMA model integration
/// </summary>
public class AsposeLlamaModel : IAIModel
{
    private readonly LLaMACppClient _client;
    private readonly ILogger<AsposeLlamaModel> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AsposeLlamaModel(string baseUrl, ILogger<AsposeLlamaModel> logger)
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };
        _client = new LLaMACppClient(baseUrl);
        _logger = logger;
    }

    public async Task<string> ProcessTextAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatClient = _client.GetChatCompletionsClient();
            var request = new ChatCompletionRequest
            {
                Model = "default",
                Messages = [
                    new ChatCompletionMessage { Role = "user", Content = text }
                ]
            };

            var response = await chatClient.CreateChatCompletionAsync(request, cancellationToken);
            return response.Choices[0].Message.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing text with LLaMA model");
            throw;
        }
    }

    public async Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default)
    {
        var prompt = @"Analyze the following meeting transcript and provide:
1. A brief summary (max 200 words)
2. Key discussion points (up to 5)
3. Main decisions made
4. Questions and answers identified
5. Important topics covered

Format the response as JSON with the following structure:
{
    ""summary"": ""string"",
    ""keyPoints"": [""string""],
    ""decisions"": [""string""],
    ""qaSection"": [{""question"": ""string"", ""answer"": ""string""}],
    ""topics"": [""string""]
}

Meeting transcript:
" + text;

        try
        {
            var response = await ProcessTextAsync(prompt, cancellationToken);

            // Clean up the response to ensure valid JSON
            response = response.Trim();
            if (response.StartsWith("```json"))
            {
                response = response.Substring(7);
            }
            if (response.EndsWith("```"))
            {
                response = response.Substring(0, response.Length - 3);
            }
            response = response.Trim();

            var result = JsonSerializer.Deserialize<AnalyzedContent>(response, _jsonOptions);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to parse AI response");
            }

            // Clean up empty or invalid values
            result.Summary = result.Summary?.Trim() ?? string.Empty;
            result.KeyPoints = result.KeyPoints?.Where(p => !string.IsNullOrWhiteSpace(p)).ToList() ?? [];
            result.Decisions = result.Decisions?.Where(d => !string.IsNullOrWhiteSpace(d)).ToList() ?? [];
            result.Topics = result.Topics?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList() ?? [];
            result.QASegments = result.QASegments?.Where(qa => 
                !string.IsNullOrWhiteSpace(qa.Question) && 
                !string.IsNullOrWhiteSpace(qa.Answer)
            ).ToList() ?? [];
            result.TranscribedText = text;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing content with LLaMA model");
            throw;
        }
    }

    public async Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default)
    {
        var prompt = @"Extract action items from the following meeting transcript. Look for:
- Tasks that need to be done
- Assignments to specific people
- Deadlines or due dates
- Priority indicators

Format the response as JSON array with the following structure:
[{
    ""description"": ""string"",
    ""assignee"": ""string"",
    ""dueDate"": ""string"",
    ""priority"": ""string""
}]

For each action item:
- description: Clear description of what needs to be done
- assignee: Name of person assigned (or ""Unassigned"")
- dueDate: Due date if mentioned (or ""Not specified"")
- priority: High/Medium/Low based on context

Meeting transcript:
" + text;

        try
        {
            var response = await ProcessTextAsync(prompt, cancellationToken);
            
            // Clean up the response to ensure valid JSON
            response = response.Trim();
            if (response.StartsWith("```json"))
            {
                response = response.Substring(7);
            }
            if (response.EndsWith("```"))
            {
                response = response.Substring(0, response.Length - 3);
            }
            response = response.Trim();

            var result = JsonSerializer.Deserialize<List<ActionItem>>(response, _jsonOptions);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to parse AI response");
            }

            // Clean up and validate action items
            return result
                .Where(item => !string.IsNullOrWhiteSpace(item.Description))
                .Select(item => new ActionItem
                {
                    Description = item.Description.Trim(),
                    Assignee = string.IsNullOrWhiteSpace(item.Assignee) ? "Unassigned" : item.Assignee.Trim(),
                    DueDate = string.IsNullOrWhiteSpace(item.DueDate) ? "Not specified" : item.DueDate.Trim(),
                    Priority = ValidatePriority(item.Priority)
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting action items with LLaMA model");
            throw;
        }
    }

    private static string ValidatePriority(string priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            return "Medium";
        }

        var normalizedPriority = priority.Trim().ToLower();
        return normalizedPriority switch
        {
            "high" => "High",
            "medium" => "Medium",
            "low" => "Low",
            _ => "Medium"
        };
    }
} 
