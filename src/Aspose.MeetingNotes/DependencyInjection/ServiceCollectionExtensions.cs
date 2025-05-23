using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.SpeechRecognition;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aspose.MeetingNotes.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Aspose.MeetingNotes services
/// with the <see cref="IServiceCollection"/> dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the core Aspose.MeetingNotes services to the specified <see cref="IServiceCollection"/>.
    /// This method configures services based on the provided <see cref="MeetingNotesOptions"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configure">An action delegate to configure the required <see cref="MeetingNotesOptions"/>.</param>
    /// <returns>The original <see cref="IServiceCollection"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configure"/> is null.</exception>
    public static IServiceCollection AddMeetingNotes(
        this IServiceCollection services,
        Action<MeetingNotesOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        // Ensure options infrastructure is available
        services.AddOptions();

        // Apply the user-provided configuration action
        services.Configure(configure);

        // Add validation for the options
        services.TryAddSingleton<IValidateOptions<MeetingNotesOptions>, MeetingNotesOptionsValidator>();

        // Register HttpClientFactory infrastructure
        services.AddHttpClient();

        // Register Audio Processor (Singleton: assumes thread-safety and no per-request state)
        // Takes the FFmpeg path directly from configured options during registration.
        services.TryAddSingleton<IAudioProcessor>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<AudioProcessor>>();
            return new AudioProcessor(options.FfMpegPath, logger);
        });

        // Register Speech Recognizer (Scoped: potentially holds state per operation)
        // Handles selection between custom implementation and default Whisper recognizer.
        services.TryAddScoped<ISpeechRecognizer>(sp => {
            var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;
            if (options.CustomSpeechRecognizer != null)
            {
                return options.CustomSpeechRecognizer;
            }

            var logger = sp.GetRequiredService<ILogger<WhisperSpeechRecognizer>>();
            var optionsSnapshot = sp.GetRequiredService<IOptions<MeetingNotesOptions>>();
            return new WhisperSpeechRecognizer(logger, optionsSnapshot);
        });

        // Register core stateless services (Singletons)
        services.TryAddSingleton<IContentAnalyzer, ContentAnalyzer>();
        services.TryAddSingleton<IActionItemExtractor, ActionItemExtractor>();
        services.TryAddSingleton<IContentExporter, ContentExporter>();

        // Register AI Model (Singleton: assumes models are thread-safe or managed internally)
        // Uses the API-based model configuration from AIModelOptions.
        services.TryAddSingleton<IAIModel>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<ApiBasedAIModel>>();

            if (options.AIModel == null || string.IsNullOrWhiteSpace(options.AIModel.Url))
            {
                logger.LogCritical("AIModel configuration is missing or invalid.");
                throw new InvalidOperationException("AIModel configuration is missing or invalid.");
            }

            logger.LogInformation("Using ApiBasedAIModel with URL: {Url}", options.AIModel.Url);
            return new ApiBasedAIModel(options.AIModel, logger);
        });

        services.TryAddScoped<MeetingNotesClient>();

        return services;
    }
}
