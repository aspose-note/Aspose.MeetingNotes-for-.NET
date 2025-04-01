namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Configuration options for Google Docs integration
    /// </summary>
    public class GoogleDocsOptions
    {
        /// <summary>
        /// Gets or sets jSON credentials for Google API authentication
        /// </summary>
        public string CredentialsJson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets iD of the Google Drive folder for document creation
        /// </summary>
        public string FolderId { get; set; } = string.Empty;
    }
}