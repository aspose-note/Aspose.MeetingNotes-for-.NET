namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents an action item or task identified during the meeting
    /// </summary>
    public class ActionItem
    {
        /// <summary>
        /// Description of the action item
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Person assigned to complete the action item
        /// </summary>
        public string? Assignee { get; set; }

        /// <summary>
        /// Due date for completing the action item
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Current status of the action item (e.g., "Not Started", "In Progress", "Completed")
        /// </summary>
        public string? Status { get; set; }
    }
} 