namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents a segment of transcribed text with speaker and timing information
    /// </summary>
    public class TranscriptionSegment
    {
        /// <summary>
        /// Gets or sets identifier of the speaker who spoke this segment
        /// </summary>
        public string Speaker { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the transcribed text content
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets start time of the segment in the audio
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Gets or sets end time of the segment in the audio
        /// </summary>
        public TimeSpan EndTime { get; set; }
    }
}
