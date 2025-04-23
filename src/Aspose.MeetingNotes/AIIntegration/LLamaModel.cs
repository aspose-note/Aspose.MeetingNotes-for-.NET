using System.Text;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;
using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLama.Sampling;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.AIIntegration
{
    /// <summary>
    /// Implementation of AI model integration using LLama
    /// </summary>
    public class LLamaModel : IAIModel
    {
        private readonly LLamaOptions llamaOptions;
        private readonly ILogger<LLamaModel> logger;
        private readonly LLamaWeights model;
        private readonly LLamaContext context;
        private readonly InteractiveExecutor executor;
        private readonly ChatSession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaModel"/> class.
        /// </summary>
        /// <param name="options">Configuration options for the AI model.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        public LLamaModel(MeetingNotesOptions options, ILogger<LLamaModel> logger)
        {
            this.llamaOptions = (LLamaOptions)options.AIModel;
            this.logger = logger;

            if (string.IsNullOrEmpty(this.llamaOptions.ModelPath))
            {
                throw new ArgumentException("LLama model path is required", nameof(options));
            }

            var parameters = new ModelParams(this.llamaOptions.ModelPath)
            {
                GpuLayerCount = this.llamaOptions.GpuLayerCount,
                ContextSize = this.llamaOptions.ContextSize
            };

            model = LLamaWeights.LoadFromFile(parameters);
            context = model.CreateContext(parameters);
            executor = new InteractiveExecutor(context);
            session = new ChatSession(executor);

            // Configure session
            session.WithHistoryTransform(new LLama2HistoryTransformer());
            session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
                ["User:", "Assistant:"],
                redundancyLength: 5));
        }

        /// <summary>
        /// Analyzes the provided text content using the LLama model
        /// </summary>
        /// <param name="text">The text content to analyze</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation</param>
        /// <returns>An AnalyzedContent containing the analysis results</returns>
        /// <exception cref="AIModelException">Thrown when there is an error during the analysis</exception>
        public async Task<AnalyzedContent> AnalyzeContentAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Starting LLama content analysis");

                var prompt = $@"Analyze the following meeting transcript and provide:
1. A concise summary (max 200 words)
2. Key points discussed
3. Main topics covered
4. Any questions and answers

Transcript:
{text}

Please format your response as follows:
Summary: [your summary]
Key Points:
- [point 1]
- [point 2]
...
Topics:
- [topic 1]
- [topic 2]
...
Q&A:
- Q: [question]
  A: [answer]
...";

                var inferenceParams = new InferenceParams
                {
                    SamplingPipeline = new DefaultSamplingPipeline
                    {
                        Temperature = this.llamaOptions.Temperature
                    },
                    MaxTokens = -1,
                    AntiPrompts = ["User:"]
                };

                var response = new StringBuilder();
                await foreach (var textChunk in session.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, prompt),
                    inferenceParams))
                {
                    response.Append(textChunk);
                }

                var result = ParseAnalysisResponse(response.ToString());
                result.TranscribedText = text;

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during LLama content analysis");
                throw new AIModelException("Failed to analyze content with LLama", ex);
            }
        }

        /// <summary>
        /// Extracts action items from the provided text using the LLama model
        /// </summary>
        /// <param name="text">The text content to extract action items from</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation</param>
        /// <returns>A list of extracted action items</returns>
        /// <exception cref="AIModelException">Thrown when there is an error during action item extraction</exception>
        public async Task<List<ActionItem>> ExtractActionItemsAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Extracting action items using LLama");

                var prompt = $@"Extract action items from the following meeting transcript. For each action item, identify:
1. Description of the task
2. Assignee (if mentioned)
3. Due date (if mentioned)
4. Status (default to 'New')

Format your response as a JSON array of objects with the following structure:
[
    {{
        ""Description"": ""task description"",
        ""Assignee"": ""person name"",
        ""DueDate"": ""YYYY-MM-DD"",
        ""Status"": ""New""
    }}
]

Transcript:
{text}";

                var inferenceParams = new InferenceParams
                {
                    SamplingPipeline = new DefaultSamplingPipeline
                    {
                        Temperature = this.llamaOptions.Temperature
                    },
                    MaxTokens = -1,
                    AntiPrompts = ["User:"]
                };

                var response = new StringBuilder();
                await foreach (var textChunk in session.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, prompt),
                    inferenceParams))
                {
                    response.Append(textChunk);
                }

                return ParseActionItemsResponse(response.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during LLama action item extraction");
                throw new AIModelException("Failed to extract action items with LLama", ex);
            }
        }

        private static AnalyzedContent ParseAnalysisResponse(string response)
        {
            var result = new AnalyzedContent();
            var lines = response.Split('\n');

            var currentSection = string.Empty;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("Summary:"))
                {
                    currentSection = "Summary";
                    result.Summary = line.Replace("Summary:", string.Empty).Trim();
                }
                else if (line.StartsWith("Key Points:"))
                {
                    currentSection = "KeyPoints";
                }
                else if (line.StartsWith("Topics:"))
                {
                    currentSection = "Topics";
                }
                else if (line.StartsWith("Q&A:"))
                {
                    currentSection = "Q&A";
                }
                else if (line.StartsWith("-"))
                {
                    var content = line.TrimStart('-').Trim();
                    switch (currentSection)
                    {
                        case "KeyPoints":
                            result.KeyPoints.Add(content);
                            break;
                        case "Topics":
                            result.Topics.Add(content);
                            break;
                        case "Q&A":
                            if (content.StartsWith("Q:"))
                            {
                                result.QASegments.Add(new QASegment
                                {
                                    Question = content.Replace("Q:", string.Empty).Trim()
                                });
                            }
                            else if (content.StartsWith("A:"))
                            {
                                if (result.QASegments.Any())
                                {
                                    result.QASegments.Last().Answer = content.Replace("A:", string.Empty).Trim();
                                }
                            }
                            break;
                    }
                }
            }

            return result;
        }

        private static List<ActionItem> ParseActionItemsResponse(string response)
        {
            try
            {
                // Try to parse as JSON first
                var jsonStart = response.IndexOf('[');
                var jsonEnd = response.LastIndexOf(']');
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var json = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    return System.Text.Json.JsonSerializer.Deserialize<List<ActionItem>>(json) ?? new List<ActionItem>();
                }

                // Fallback to text parsing
                return ParseActionItemsFromText(response);
            }
            catch (Exception)
            {
                return ParseActionItemsFromText(response);
            }
        }

        private static List<ActionItem> ParseActionItemsFromText(string response)
        {
            var actionItems = new List<ActionItem>();
            var lines = response.Split('\n');

            ActionItem? currentItem = null;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("Description:"))
                {
                    if (currentItem != null)
                    {
                        actionItems.Add(currentItem);
                    }
                    currentItem = new ActionItem
                    {
                        Description = line.Replace("Description:", string.Empty).Trim(),
                        Status = "New"
                    };
                }
                else if (currentItem != null)
                {
                    if (line.StartsWith("Assignee:"))
                    {
                        currentItem.Assignee = line.Replace("Assignee:", string.Empty).Trim();
                    }
                    else if (line.StartsWith("DueDate:"))
                    {
                        currentItem.DueDate = line.Replace("DueDate:", string.Empty).Trim();
                    }
                }
            }

            if (currentItem != null)
            {
                actionItems.Add(currentItem);
            }

            return actionItems;
        }

        /// <summary>
        /// Chat History transformer for LLama 2 family
        /// </summary>
        private class LLama2HistoryTransformer : IHistoryTransform
        {
            public string Name => "LLama2";

            public IHistoryTransform Clone()
            {
                return new LLama2HistoryTransformer();
            }

            public string HistoryToText(ChatHistory history)
            {
                if (history.Messages.Count == 0)
                    return string.Empty;

                var builder = new StringBuilder(64 * history.Messages.Count);

                int i = 0;
                if (history.Messages[i].AuthorRole == AuthorRole.System)
                {
                    builder.Append($"[INST] <<SYS>>\n").Append(history.Messages[0].Content.Trim()).Append("\n<</SYS>>\n");
                    i++;

                    if (history.Messages.Count > 1)
                    {
                        builder.Append(history.Messages[1].Content.Trim()).Append(" [/INST]");
                        i++;
                    }
                }

                for (; i < history.Messages.Count; i++)
                {
                    if (history.Messages[i].AuthorRole == AuthorRole.User)
                    {
                        builder.Append(i == 0 ? "[INST] " : "<s>[INST] ").Append(history.Messages[i].Content.Trim()).Append(" [/INST]");
                    }
                    else
                    {
                        builder.Append(' ').Append(history.Messages[i].Content.Trim()).Append(" </s>");
                    }
                }

                return builder.ToString();
            }

            public ChatHistory TextToHistory(AuthorRole role, string text)
            {
                return new ChatHistory([new ChatHistory.Message(role, text)]);
            }
        }
    }
}
