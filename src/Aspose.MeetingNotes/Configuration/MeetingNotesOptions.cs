using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for the MeetingNotes SDK.
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
        /// Gets or sets the path to the Whisper model file.
        /// </summary>
        public string WhisperModelPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the size of the Whisper model to use.
        /// </summary>
        public string WhisperModelSize { get; set; } = "base";

        /// <summary>
        /// Gets or sets the API key for the AI model service.
        /// </summary>
        public string AIModelApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selected AI model to use for analysis.
        /// </summary>
        public AIModelType AIModelType { get; set; } = AIModelType.ChatGPT;
    }
}
