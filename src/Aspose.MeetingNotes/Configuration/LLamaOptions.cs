namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Provides configuration options specific to using a local LLama model via LLamaSharp.
/// Inherits from <see cref="AIModelOptions"/>.
/// </summary>
public class LLamaOptions : AIModelOptions
{
    /// <summary>
    /// Gets the type of the AI model, which is always <see cref="AIModelType.LLama"/>.
    /// </summary>
    public override AIModelType Type => AIModelType.LLama;

    /// <summary>
    /// Gets or sets the required file system path to the LLama model file (e.g., GGUF format).
    /// </summary>
    /// <value>Must be a non-empty string pointing to an existing model file.</value>
    public required string ModelPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum context size (number of tokens) the model should handle.
    /// Check the specific model's documentation for appropriate values.
    /// </summary>
    /// <value>Defaults to 4096. Can be null if not specified.</value>
    public uint? ContextSize { get; set; } = 4096;

    /// <summary>
    /// Gets or sets the sampling temperature for text generation (typically between 0.0 and 1.0).
    /// Higher values make output more random, lower values make it more deterministic.
    /// </summary>
    /// <value>Defaults to 0.6f.</value>
    public float Temperature { get; set; } = 0.6f;
}
