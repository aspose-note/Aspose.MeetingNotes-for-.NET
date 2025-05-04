namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Provides configuration options specific to using the OpenAI ChatGPT model.
/// Inherits from <see cref="AIModelOptions"/>.
/// </summary>
public class ChatGPTOptions : AIModelOptions
{
    /// <summary>
    /// Gets the type of the AI model, which is always <see cref="AIModelType.ChatGPT"/>.
    /// </summary>
    public override AIModelType Type => AIModelType.ChatGPT;

    /// <summary>
    /// Gets or sets the API endpoint URL for the ChatGPT service.
    /// </summary>
    /// <value>Defaults to "https://api.openai.com/v1/chat/completions".</value>
    public string ApiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";

    /// <summary>
    /// Gets or sets the specific ChatGPT model to use (e.g., "gpt-3.5-turbo", "gpt-4").
    /// </summary>
    /// <value>Defaults to "gpt-3.5-turbo".</value>
    public string Model { get; set; } = "gpt-3.5-turbo";

    /// <summary>
    /// Gets or sets the sampling temperature for text generation (typically between 0.0 and 1.0).
    /// Higher values make output more random, lower values make it more deterministic.
    /// </summary>
    /// <value>Defaults to 0.7f.</value>
    public float Temperature { get; set; } = 0.7f;
}
