using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Metrics;
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
            services.AddSingleton<IAudioProcessor, AudioProcessor>();
            services.AddScoped<ISpeechRecognizer, WhisperSpeechRecognizer>();
            services.AddSingleton<IContentAnalyzer, ContentAnalyzer>();
            services.AddSingleton<IActionItemExtractor, ActionItemExtractor>();
            services.AddSingleton<IContentExporter, ContentExporter>();

            // Register AI model based on configuration
            services.AddSingleton<IAIModel>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;
                var httpClient = sp.GetRequiredService<HttpClient>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                return options.AIModelType switch
                {
                    AIModelType.ChatGPT => new ChatGPTModel(httpClient, options, loggerFactory.CreateLogger<ChatGPTModel>()),
                    AIModelType.Grok => new GrokModel(httpClient, options, loggerFactory.CreateLogger<GrokModel>()),
                    AIModelType.DeepSeek => new DeepSeekModel(httpClient, options, loggerFactory.CreateLogger<DeepSeekModel>()),
                    _ => throw new ArgumentException($"Unsupported AI model: {options.AIModelType.ToString()}")
                };
            });

            // Add metrics collector
            services.AddScoped<IMetricsCollector, MetricsCollector>();

            // Add MeetingNotes client
            services.AddScoped<MeetingNotesClient>();

            return services;
        }
    }
}
