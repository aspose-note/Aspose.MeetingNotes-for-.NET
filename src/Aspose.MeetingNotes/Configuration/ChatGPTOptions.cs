namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// ChatGPT-specific configuration options.
    /// </summary>
    public class ChatGPTOptions : AIModelOptions
    {
        /// <summary>
        /// Gets the type of the AI model (ChatGPT).
        /// </summary>
        public override AIModelType Type => AIModelType.ChatGPT;

        /// <summary>
        /// Gets or sets the API endpoint for ChatGPT.
        /// </summary>
        public string ApiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";

        /// <summary>
        /// Gets or sets the model to use (e.g., "gpt-3.5-turbo", "gpt-4").
        /// </summary>
        public string Model { get; set; } = "gpt-3.5-turbo";

        /// <summary>
        /// Gets or sets the temperature for text generation (0.0 to 1.0).
        /// </summary>
        public float Temperature { get; set; } = 0.7f;
    }
}
