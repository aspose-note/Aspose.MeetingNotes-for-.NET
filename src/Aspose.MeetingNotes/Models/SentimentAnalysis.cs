namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents sentiment analysis of the meeting
    /// </summary>
    public class SentimentAnalysis
    {
        /// <summary>
        /// Gets or sets overall sentiment score (-1 to 1)
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets primary sentiment category
        /// </summary>
        public SentimentCategory Category { get; set; }

        /// <summary>
        /// Gets or sets key emotional markers identified
        /// </summary>
        public List<EmotionalMarker> EmotionalMarkers { get; set; } = new ();
    }
}
