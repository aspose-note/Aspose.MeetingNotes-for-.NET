using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;

namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Provides the main configuration options for the Aspose.MeetingNotes library.
/// Aggregates settings for language, audio processing, speech recognition, AI models, and custom implementations.
/// </summary>
public class MeetingNotesOptions
{
    /// <summary>
    /// Gets or sets the expected language code for the meeting audio (e.g., "en", "ru").
    /// Setting to "auto" (if supported by the speech recognizer) attempts language detection.
    /// </summary>
    /// <value>Defaults to "auto".</value>
    public string Language { get; set; } = "auto";

    /// <summary>
    /// Gets or sets the required file system path to the FFmpeg executable used for audio conversion.
    /// </summary>
    /// <value>Must be a non-empty string pointing to a valid FFmpeg executable.</value>
    public required string FfMpegPath { get; set; }

    /// <summary>
    /// Gets or sets the default format for exporting results when not specified otherwise.
    /// </summary>
    /// <value>Defaults to <see cref="ExportFormat.Markdown"/>.</value>
    public ExportFormat ExportFormat { get; set; } = ExportFormat.Markdown;

    /// <summary>
    /// Gets or sets the configuration options for the speech recognition component.
    /// </summary>
    /// <value>Defaults to a new instance of <see cref="SpeechRecognitionOptions"/>.</value>
    public SpeechRecognitionOptions SpeechRecognition { get; set; } = new ();

    /// <summary>
    /// Gets or sets the configuration options for the primary AI model to be used.
    /// This should be an instance of a class derived from <see cref="AIModelOptions"/> (e.g., <see cref="ChatGPTOptions"/>, <see cref="LLamaOptions"/>).
    /// If <see cref="CustomAIModel"/> is set, this property might be ignored by the default dependency injection setup.
    /// </summary>
    /// <value>Defaults to a new instance of <see cref="ChatGPTOptions"/>.</value>
    public AIModelOptions AIModel { get; set; } = new ChatGPTOptions();

    /// <summary>
    /// Gets or sets a custom implementation of <see cref="ISpeechRecognizer"/>.
    /// If set, this instance will be used instead of the default recognizer configured via <see cref="SpeechRecognition"/> options.
    /// </summary>
    /// <value>Defaults to null.</value>
    public ISpeechRecognizer? CustomSpeechRecognizer { get; set; } = null;

    /// <summary>
    /// Gets or sets a custom implementation of <see cref="IAIModel"/>.
    /// If set, this instance will be used instead of the AI model configured via <see cref="AIModel"/> options.
    /// </summary>
    /// <value>Defaults to null.</value>
    public IAIModel? CustomAIModel { get; set; } = null;
}
