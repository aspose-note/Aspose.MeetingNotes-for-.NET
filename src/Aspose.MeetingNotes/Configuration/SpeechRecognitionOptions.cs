namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Speech recognition configuration options.
    /// </summary>
    public class SpeechRecognitionOptions
    {
        /// <summary>
        /// Gets or sets the path to the Whisper model file.
        /// </summary>
        public string ModelPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the size of the Whisper model to use.
        /// </summary>
        public string ModelSize { get; set; } = "base";
    }
}
