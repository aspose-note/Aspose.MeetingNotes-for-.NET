namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for the MeetingNotes SDK
    /// </summary>
    public class MeetingNotesOptions
    {
        /// <summary>
        /// Language code for speech recognition (e.g., "en", "ru")
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// Selected AI model for content analysis
        /// </summary>
        public AIModelType AIModel { get; set; } = AIModelType.ChatGPT;

        /// <summary>
        /// API key for the selected AI service
        /// </summary>
        public string AIApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Whisper model size (tiny, base, small, medium, large)
        /// </summary>
        public string WhisperModelSize { get; set; } = "base";
    }

    public enum AIModelType
    {
        ChatGPT,
        Grok,
        DeepSeek
    }
} 