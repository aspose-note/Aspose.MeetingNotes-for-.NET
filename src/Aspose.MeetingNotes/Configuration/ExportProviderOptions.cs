namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for various export providers
    /// </summary>
    public class ExportProviderOptions
    {
        /// <summary>
        /// Configuration for Jira integration
        /// </summary>
        public JiraOptions? Jira { get; set; }

        /// <summary>
        /// Configuration for Trello integration
        /// </summary>
        public TrelloOptions? Trello { get; set; }

        /// <summary>
        /// Configuration for Google Docs integration
        /// </summary>
        public GoogleDocsOptions? GoogleDocs { get; set; }
    }

    /// <summary>
    /// Configuration options for Jira integration
    /// </summary>
    public class JiraOptions
    {
        /// <summary>
        /// Base URL of the Jira instance
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Jira username for authentication
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Jira API token for authentication
        /// </summary>
        public string ApiToken { get; set; } = string.Empty;

        /// <summary>
        /// Jira project key for task creation
        /// </summary>
        public string ProjectKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration options for Trello integration
    /// </summary>
    public class TrelloOptions
    {
        /// <summary>
        /// Trello API key for authentication
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Trello access token for authentication
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// ID of the Trello board for task creation
        /// </summary>
        public string BoardId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration options for Google Docs integration
    /// </summary>
    public class GoogleDocsOptions
    {
        /// <summary>
        /// JSON credentials for Google API authentication
        /// </summary>
        public string CredentialsJson { get; set; } = string.Empty;

        /// <summary>
        /// ID of the Google Drive folder for document creation
        /// </summary>
        public string FolderId { get; set; } = string.Empty;
    }
} 