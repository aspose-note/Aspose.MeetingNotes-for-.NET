using Microsoft.Extensions.Options;

namespace Aspose.MeetingNotes.Configuration
{
    /// <summary>
    /// Validator for MeetingNotes configuration options
    /// </summary>
    public class MeetingNotesOptionsValidator : IValidateOptions<MeetingNotesOptions>
    {
        /// <summary>
        /// Validates the MeetingNotes configuration options.
        /// </summary>
        /// <param name="name">The name of the options instance being validated.</param>
        /// <param name="options">The options instance to validate.</param>
        /// <returns>The validation result.</returns>
        public ValidateOptionsResult Validate(string? name, MeetingNotesOptions options)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(options.Language))
            {
                errors.Add("Language must be specified");
            }

            if (string.IsNullOrEmpty(options.AIModelApiKey))
            {
                errors.Add("AI API key must be specified");
            }

            if (!IsValidWhisperModelSize(options.WhisperModelSize))
            {
                errors.Add("Invalid Whisper model size. Valid values are: tiny, base, small, medium, large");
            }

            return errors.Count > 0
                ? ValidateOptionsResult.Fail(errors)
                : ValidateOptionsResult.Success;
        }

        private bool IsValidWhisperModelSize(string size)
        {
            return new[] { "tiny", "base", "small", "medium", "large" }
                .Contains(size.ToLower());
        }
    }
}