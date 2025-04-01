using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.ActionItems
{
    /// <summary>
    /// Interface for extracting action items from meeting content
    /// </summary>
    public interface IActionItemExtractor
    {
        /// <summary>
        /// Extract action items from analyzed content
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<ActionItem>> ExtractActionItemsAsync(AnalyzedContent content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Export action items to external task tracking system
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ExportToTaskTrackerAsync(List<ActionItem> actionItems, TaskTrackerType trackerType, CancellationToken cancellationToken = default);
    }
}
