using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.SpeechRecognition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aspose.MeetingNotes.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring MeetingNotes services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MeetingNotes services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configure">An action to configure the MeetingNotes options.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddMeetingNotes(
            this IServiceCollection services,
            Action<MeetingNotesOptions> configure)
        {
            // Add options
            services.Configure(configure);
            services.AddOptions<MeetingNotesOptions>();

            // Register services
            services.AddHttpClient();

            // Register audio format handlers
            services.AddSingleton<IAudioFormatHandler, WavAudioHandler>();
            services.AddSingleton<IAudioFormatHandler, OggAudioHandler>();

            // Register audio processor
            services.AddSingleton<IAudioProcessor, AudioProcessor>();

            services.AddScoped<ISpeechRecognizer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;
                return options.CustomSpeechRecognizer ?? new WhisperSpeechRecognizer(
                    sp.GetRequiredService<ILogger<WhisperSpeechRecognizer>>(),
                    sp.GetRequiredService<IOptions<MeetingNotesOptions>>());
            });

            services.AddSingleton<IContentAnalyzer, ContentAnalyzer>();
            services.AddSingleton<IActionItemExtractor, ActionItemExtractor>();
            services.AddSingleton<IContentExporter, ContentExporter>();

            // Register AI model based on configuration
            services.AddSingleton<IAIModel>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;

                // Use custom model if provided
                if (options.CustomAIModel != null)
                {
                    return options.CustomAIModel;
                }

                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                return options.AIModel.Type switch
                {
                    AIModelType.ChatGPT => new ChatGPTModel(
                        sp.GetRequiredService<HttpClient>(),
                        options,
                        loggerFactory.CreateLogger<ChatGPTModel>()),
                    AIModelType.LLama => new LLamaModel(
                        options,
                        loggerFactory.CreateLogger<LLamaModel>()),
                    _ => throw new ArgumentException($"Unsupported AI model: {options.AIModel.Type.ToString()}")
                };
            });

            // Add MeetingNotes client
            services.AddScoped<MeetingNotesClient>();

            return services;
        }
    }
}
