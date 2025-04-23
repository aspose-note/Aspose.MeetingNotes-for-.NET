namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// LLama-specific configuration options.
    /// </summary>
    public class LLamaOptions : AIModelOptions
    {
        /// <summary>
        /// Gets the type of the AI model (LLama).
        /// </summary>
        public override AIModelType Type => AIModelType.LLama;

        /// <summary>
        /// Gets or sets the path to the LLama model file.
        /// </summary>
        public string ModelPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of GPU layers to use for LLama model.
        /// </summary>
        public int GpuLayerCount { get; set; } = 10;

        /// <summary>
        /// Gets or sets the temperature for text generation (0.0 to 1.0).
        /// </summary>
        public float Temperature { get; set; } = 0.6f;

        /// <summary>
        /// Gets or sets the context size for the model.
        /// </summary>
        public uint? ContextSize { get; set; } = 4096;
    }
}
