namespace Aspose.MeetingNotes.Models;

/// <summary>
/// Represents an action item or task identified during a meeting analysis.
/// Records are suitable here for representing data transfer objects.
/// </summary>
public record ActionItem
{
    /// <summary>
    /// Gets the description of the action item or task.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name or identifier of the person assigned to complete the action item.
    /// Can be null if unassigned.
    /// </summary>
    /// <value>Defaults to "Unassigned".</value>
    public string Assignee { get; init; } = "Unassigned";

    /// <summary>
    /// Gets the due date for completing the action item, represented as a string.
    /// Could be a specific date, a relative term (e.g., "Next Friday"), or unspecified.
    /// </summary>
    /// <value>Defaults to "Not specified".</value>
    public string DueDate { get; init; } = "Not specified";

    /// <summary>
    /// Gets the current status of the action item (e.g., "Not Started", "In Progress", "Completed").
    /// Can be null if status is not determined or applicable.
    /// </summary>
    /// <value>Defaults to null.</value>
    public string? Status { get; init; } = null;

    /// <summary>
    /// Gets the priority level of the action item (e.g., "High", "Medium", "Low").
    /// </summary>
    /// <value>Defaults to "Medium".</value>
    public string Priority { get; init; } = "Medium";
}
