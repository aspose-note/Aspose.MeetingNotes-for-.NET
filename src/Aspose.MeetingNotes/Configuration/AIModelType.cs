namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Defines the available built-in AI model types that can be used for content analysis and action item extraction.
/// </summary>
public enum AIModelType
{
    /// <summary>
    /// Represents OpenAI's ChatGPT model service. Requires <see cref="ChatGPTOptions"/>.
    /// </summary>
    ChatGPT,

    /// <summary>
    /// Represents a local LLama model accessed via LLamaSharp. Requires <see cref="LLamaOptions"/>.
    /// </summary>
    LLama
}
