namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Base abstract class for AI model configuration options.
/// Implementations should specify the type of the AI model and its specific settings.
/// </summary>
public abstract class AIModelOptions
{
    /// <summary>
    /// Gets the specific type of the AI model configured by this options instance.
    /// </summary>
    public abstract AIModelType Type { get; }

    /// <summary>
    /// Gets or sets the API key required for accessing the AI model service, if applicable.
    /// </summary>
    /// <value>The API key string. Defaults to an empty string.</value>
    public string ApiKey { get; set; } = string.Empty;
}
