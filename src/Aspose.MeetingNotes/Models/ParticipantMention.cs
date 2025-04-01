namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents a mention of a participant in the meeting
    /// </summary>
    public class ParticipantMention
    {
        /// <summary>
        /// Gets or sets name of the participant
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets number of times mentioned
        /// </summary>
        public int MentionCount { get; set; }

        /// <summary>
        /// Gets or sets contexts in which the participant was mentioned
        /// </summary>
        public List<string> Contexts { get; set; } = new ();
    }
}
