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
        // Handles selection between custom, configured (ChatGPT/LLama), or potentially throws if misconfigured.
        services.TryAddSingleton<IAIModel>(sp => {
            var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            if (options.CustomAIModel != null)
            {
                loggerFactory.CreateLogger(typeof(ServiceCollectionExtensions))
                    .LogInformation("Using custom IAIModel implementation: {ModelType}", options.CustomAIModel.GetType().Name);
                return options.CustomAIModel;
            }

            // Select built-in based on configuration type
            switch (options.AIModel)
            {
                case ChatGPTOptions chatGptOpts:
                    loggerFactory.CreateLogger(typeof(ServiceCollectionExtensions))
                        .LogInformation("Using configured ChatGPTModel (API Key validation happens in constructor)");

                    var httpClient = sp.GetRequiredService<HttpClient>();
                    return new ChatGPTModel(httpClient, options, loggerFactory.CreateLogger<ChatGPTModel>());

                case LLamaOptions llamaOpts:
                    loggerFactory.CreateLogger(typeof(ServiceCollectionExtensions))
                        .LogInformation("Using configured LLamaModel from path: {ModelPath}", llamaOpts.ModelPath);
                    return new LLamaModel(options, loggerFactory.CreateLogger<LLamaModel>());

                case null:
                    loggerFactory.CreateLogger(typeof(ServiceCollectionExtensions))
                        .LogCritical("AIModel configuration is null and no CustomAIModel was provided");
                    throw new InvalidOperationException("AI model configuration is missing in MeetingNotesOptions. Provide either AIModel options (ChatGPTOptions or LLamaOptions) or a CustomAIModel instance.");

                default:
                    loggerFactory.CreateLogger(typeof(ServiceCollectionExtensions))
                        .LogCritical("Unsupported AIModel options type provided: {OptionsType}", options.AIModel.GetType().Name);
                    throw new ArgumentException($"Unsupported AI model options type: {options.AIModel.GetType().Name}");
            }
        });

        services.TryAddScoped<MeetingNotesClient>();

        return services;
    }
}
