namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for Trello integration
    /// </summary>
    public class TrelloOptions
    {
        /// <summary>
        /// Gets or sets trello API key for authentication
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets trello access token for authentication
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets iD of the Trello board for task creation
        /// </summary>
        public string BoardId { get; set; } = string.Empty;
    }
}
