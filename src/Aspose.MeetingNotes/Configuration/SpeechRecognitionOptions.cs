namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Provides configuration options specific to the speech recognition component,
/// particularly when using the default Whisper-based recognizer.
/// </summary>
public class SpeechRecognitionOptions
{
    /// <summary>
    /// Gets or sets the file system path to a specific Whisper model file (e.g., GGUF format).
    /// If provided, this specific model file will be loaded.
    /// If null or empty, the recognizer might attempt to download a model based on <see cref="ModelSize"/>.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string ModelPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size or type of the Whisper model to use when <see cref="ModelPath"/> is not specified.
    /// The default recognizer may use this value to download the appropriate model (e.g., "tiny", "base", "small").
    /// Check the specific recognizer implementation for supported values.
    /// </summary>
    /// <value>Defaults to "base".</value>
    public string ModelSize { get; set; } = "base";
}
