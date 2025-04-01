using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for the MeetingNotes SDK
    /// </summary>
    public class MeetingNotesOptions
    {
        /// <summary>
        /// The language code for transcription (e.g., "en", "ru")
        /// </summary>
        public string Language { get; set; } = "auto";

        /// <summary>
        /// The format to export the results in
        /// </summary>
        public ExportFormat ExportFormat { get; set; } = ExportFormat.Markdown;

        /// <summary>
        /// The path to the Whisper model file
        /// </summary>
        public string WhisperModelPath { get; set; } = string.Empty;

        /// <summary>
        /// The size of the Whisper model to use
        /// </summary>
        public string WhisperModelSize { get; set; } = "base";

        /// <summary>
        /// The API key for the AI model service
        /// </summary>
        public string AIModelApiKey { get; set; } = string.Empty;

        /// <summary>
        /// The type of AI model to use
        /// </summary>
        public AIModelType AIModelType { get; set; } = AIModelType.ChatGPT;

        /// <summary>
        /// The AI model to use (legacy property, use AIModelType instead)
        /// </summary>
        [Obsolete("Use AIModelType instead")]
        public string AIModel
        {
            get => AIModelType.ToString();
            set => AIModelType = Enum.Parse<AIModelType>(value, true);
        }
    }

    public enum AIModelType
    {
        ChatGPT,
        Grok,
        DeepSeek
    }
} 