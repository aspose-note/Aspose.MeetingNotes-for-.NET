namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for Jira integration
    /// </summary>
    public class JiraOptions
    {
        /// <summary>
        /// Gets or sets base URL of the Jira instance
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets jira username for authentication
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets jira API token for authentication
        /// </summary>
        public string ApiToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets jira project key for task creation
        /// </summary>
        public string ProjectKey { get; set; } = string.Empty;
    }
}