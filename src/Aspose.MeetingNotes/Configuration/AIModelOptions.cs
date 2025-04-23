namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Base class for AI model configuration options.
    /// </summary>
    public abstract class AIModelOptions
    {
        /// <summary>
        /// Gets the type of the AI model.
        /// </summary>
        public abstract AIModelType Type { get; }

        /// <summary>
        /// Gets or sets the API key for the AI model service.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
