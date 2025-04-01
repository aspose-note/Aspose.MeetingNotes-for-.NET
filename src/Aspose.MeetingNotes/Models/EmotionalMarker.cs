namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents an emotional marker in the sentiment analysis
    /// </summary>
    public class EmotionalMarker
    {
        /// <summary>
        /// Gets or sets type of emotion
        /// </summary>
        public string Emotion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets intensity of the emotion (0-1)
        /// </summary>
        public double Intensity { get; set; }

        /// <summary>
        /// Gets or sets timestamp when this emotion was detected
        /// </summary>
        public TimeSpan Timestamp { get; set; }
    }
}
