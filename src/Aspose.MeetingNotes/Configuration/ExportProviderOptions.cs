namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for various export providers
    /// </summary>
    public class ExportProviderOptions
    {
        /// <summary>
        /// Gets or sets configuration for Jira integration
        /// </summary>
        public JiraOptions? Jira { get; set; }

        /// <summary>
        /// Gets or sets configuration for Trello integration
        /// </summary>
        public TrelloOptions? Trello { get; set; }

        /// <summary>
        /// Gets or sets configuration for Google Docs integration
        /// </summary>
        public GoogleDocsOptions? GoogleDocs { get; set; }
    }
}