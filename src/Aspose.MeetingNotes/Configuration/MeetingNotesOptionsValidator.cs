using Microsoft.Extensions.Options;

namespace Aspose.MeetingNotes.Configuration;

/// <summary>
/// Performs validation checks on <see cref="MeetingNotesOptions"/> instances.
/// </summary>
public class MeetingNotesOptionsValidator : IValidateOptions<MeetingNotesOptions>
{
    private static readonly string[] ValidWhisperModelSizes = ["tiny", "base", "small", "medium", "large"];

    /// <summary>
    /// Validates a specific named options instance (or all when name is null).
    /// </summary>
    /// <param name="name">The name of the options instance being validated.</param>
    /// <param name="options">The <see cref="MeetingNotesOptions"/> instance to validate.</param>
    /// <returns>A <see cref="ValidateOptionsResult"/> indicating success or failure.</returns>
    public ValidateOptionsResult Validate(string? name, MeetingNotesOptions options)
    {
        var errors = new List<string>();

        // Validate Language (allow 'auto')
        if (string.IsNullOrWhiteSpace(options.Language))
        {
            errors.Add($"{nameof(options.Language)} cannot be null or empty");
        }

        // Validate FfMpegPath
        if (string.IsNullOrWhiteSpace(options.FfMpegPath))
        {
            errors.Add($"{nameof(options.FfMpegPath)} must be specified");
        }
        else if (!File.Exists(options.FfMpegPath)) // Check existence during validation
        {
            errors.Add($"FFmpeg executable not found at the specified path: {options.FfMpegPath}");
        }

        // Validate AsposeLicensePath
        if (!string.IsNullOrWhiteSpace(options.AsposeLicensePath))
        {
            if (!File.Exists(options.AsposeLicensePath))
            {
                errors.Add($"Aspose license file not found at the specified path: {options.AsposeLicensePath}.");
            }
        }

        // Validate SpeechRecognition options if default recognizer is likely used
        if (options.CustomSpeechRecognizer == null && options.SpeechRecognition != null)
        {
            if (string.IsNullOrWhiteSpace(options.SpeechRecognition.ModelSize))
            {
                errors.Add($"SpeechRecognition.{nameof(options.SpeechRecognition.ModelSize)} must be specified");
            }

            // Case-insensitive check against valid sizes
            else if (!ValidWhisperModelSizes.Contains(options.SpeechRecognition.ModelSize.ToLowerInvariant()))
            {
                errors.Add($"Invalid SpeechRecognition.{nameof(options.SpeechRecognition.ModelSize)} '{options.SpeechRecognition.ModelSize}'. Valid values are: {string.Join(", ", ValidWhisperModelSizes)}");
            }
        }

        // Validate API-based AI model config
        if (options.AIModel != null)
        {
            if (string.IsNullOrWhiteSpace(options.AIModel.Url))
            {
                errors.Add($"AIModel.{nameof(options.AIModel.Url)} must be specified.");
            }

            if (options.AIModel.Temperature is < 0 or > 1)
            {
                errors.Add($"AIModel.{nameof(options.AIModel.Temperature)} must be between 0.0 and 1.0.");
            }
        }

        if (errors.Count > 0)
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}
