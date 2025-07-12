namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Base abstract class for AI model configuration options.
/// Implementations should specify the type of the AI model and its specific settings.
/// </summary>
public class AIModelOptions
{
    /// <summary>
    /// Gets or sets the base URL of the external AI API endpoint.
    /// This should point to a compatible LLM API such as LLaMA.cpp, DeepSeek, or similar.
    /// </summary>
    /// <value>Base URL string (e.g., "http://localhost:8080/v1"). Defaults to local endpoint.</value>
    public string Url { get; set; } = "http://localhost:8080/v1";

    /// <summary>
    /// Gets or sets the name of the AI model to use for processing requests.
    /// This corresponds to the model identifier used by the external LLM API (e.g., "llama-3-8b-instruct").
    /// </summary>
    /// <value>Defaults to "default".</value>
    public string ModelName { get; set; } = "default";

    /// <summary>
    /// Gets or sets the API key required for accessing the AI model service, if applicable.
    /// </summary>
    /// <value>The API key string. Defaults to an empty string.</value>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the temperature parameter controlling randomness in text generation.
    /// Values typically range from 0.0 (deterministic) to 1.0 (more creative).
    /// </summary>
    /// <value>Defaults to 0.7.</value>
    public float Temperature { get; set; } = 0.7f;

    /// <summary>
    /// Gets or sets the top-p (nucleus sampling) parameter.
    /// Only the smallest set of tokens with cumulative probability ≥ top_p are considered.
    /// </summary>
    /// <value>Defaults to 0.95.</value>
    public float TopP { get; set; } = 0.95f;

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate in the response.
    /// Use -1 or null to indicate unlimited.
    /// </summary>
    /// <value>Defaults to -1 (unlimited).</value>
    public int MaxTokens { get; set; } = -1;
}
