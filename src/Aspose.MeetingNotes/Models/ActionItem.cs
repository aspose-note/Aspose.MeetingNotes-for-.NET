namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents an action item or task identified during the meeting
    /// </summary>
    public class ActionItem
    {
        /// <summary>
        /// Gets or sets description of the action item
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets person assigned to complete the action item
        /// </summary>
        public string? Assignee { get; set; }

        /// <summary>
        /// Gets or sets due date for completing the action item
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets current status of the action item (e.g., "Not Started", "In Progress", "Completed")
        /// </summary>
        public string? Status { get; set; }
    }
}
