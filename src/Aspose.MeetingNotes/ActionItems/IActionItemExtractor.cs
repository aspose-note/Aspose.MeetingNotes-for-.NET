using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.ActionItems;

/// <summary>
/// Defines the contract for services that extract action items from analyzed meeting content.
/// </summary>
public interface IActionItemExtractor
{
    /// <summary>
    /// Asynchronously extracts action items from the analyzed meeting content using an AI model.
    /// </summary>
    /// <param name="content">The analyzed content containing summaries, sections, etc.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a list (<see cref="List{ActionItem}"/>) of extracted action items.
    /// Returns an empty list if no action items are found.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="content"/> is null.</exception>
    /// <exception cref="Aspose.MeetingNotes.Exceptions.AIModelException">Thrown if the underlying AI model fails during extraction.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    Task<List<ActionItem>> ExtractActionItemsAsync(AnalyzedContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously exports a list of action items to an external task tracking system.
    /// </summary>
    /// <param name="actionItems">The list of action items to export.</param>
    /// <param name="trackerType">The type of task tracking system to export to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous export operation.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="actionItems"/> is null.</exception>
    /// <exception cref="System.NotSupportedException">Thrown when the specified <paramref name="trackerType"/> is not supported by the implementation.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    /// <remarks>
    /// Implementations should handle authentication and API interactions with the specific task tracker.
    /// This method may throw specific exceptions related to the task tracker API communication.
    /// </remarks>
    Task ExportToTaskTrackerAsync(List<ActionItem> actionItems, TaskTrackerType trackerType, CancellationToken cancellationToken = default);
}
