using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.SpeechRecognition;

namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Base configuration options for the MeetingNotes SDK.
    /// </summary>
    public class MeetingNotesOptions
    {
        /// <summary>
        /// Gets or sets the language code for transcription (e.g., "en", "ru").
        /// </summary>
        public string Language { get; set; } = "auto";

        /// <summary>
        /// Gets or sets the format to export the results in.
        /// </summary>
        public ExportFormat ExportFormat { get; set; } = ExportFormat.Markdown;

        /// <summary>
        /// Gets or sets the speech recognition options.
        /// </summary>
        public SpeechRecognitionOptions SpeechRecognition { get; set; } = new ();

        /// <summary>
        /// Gets or sets the AI model options.
        /// </summary>
        public AIModelOptions AIModel { get; set; } = new ChatGPTOptions();

        /// <summary>
        /// Gets or sets a custom implementation of ISpeechRecognizer.
        /// If set, this implementation will be used instead of the built-in WhisperSpeechRecognizer.
        /// </summary>
        public ISpeechRecognizer? CustomSpeechRecognizer { get; set; }

        /// <summary>
        /// Gets or sets a custom implementation of IAIModel.
        /// If set, this implementation will be used instead of the built-in models.
        /// </summary>
        public IAIModel? CustomAIModel { get; set; }
    }
}
