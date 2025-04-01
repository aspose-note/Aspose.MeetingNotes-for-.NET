using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.SpeechRecognition;
using Aspose.MeetingNotes.AIIntegration;
using Aspose.MeetingNotes.ContentAnalysis;
using Aspose.MeetingNotes.ActionItems;
using Aspose.MeetingNotes.Exporters;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Monitoring;

namespace Aspose.MeetingNotes.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up MeetingNotes services in an IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MeetingNotes services to the specified IServiceCollection
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to</param>
        /// <param name="configureOptions">An action to configure the MeetingNotesOptions</param>
        /// <returns>The IServiceCollection so that additional calls can be chained</returns>
        public static IServiceCollection AddMeetingNotes(
            this IServiceCollection services,
            Action<MeetingNotesOptions> configureOptions)
        {
            // Add options
            services.Configure(configureOptions);
            services.AddOptions<MeetingNotesOptions>();
            
            // Register services
            services.AddHttpClient();
            services.AddSingleton<IAudioProcessor, AudioProcessor>();
            services.AddSingleton<ISpeechRecognizer, WhisperSpeechRecognizer>();
            services.AddSingleton<IContentAnalyzer, ContentAnalyzer>();
            services.AddSingleton<IActionItemExtractor, ActionItemExtractor>();
            services.AddSingleton<IContentExporter, ContentExporter>();
            services.AddSingleton<PerformanceMetrics>();

            // Register AI model based on configuration
            services.AddSingleton<IAIModel>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MeetingNotesOptions>>().Value;
                var httpClient = sp.GetRequiredService<HttpClient>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                return options.AIModel switch
                {
                    AIModelType.ChatGPT => new ChatGPTModel(httpClient, options, loggerFactory.CreateLogger<ChatGPTModel>()),
                    AIModelType.Grok => new GrokModel(httpClient, options, loggerFactory.CreateLogger<GrokModel>()),
                    AIModelType.DeepSeek => new DeepSeekModel(httpClient, options, loggerFactory.CreateLogger<DeepSeekModel>()),
                    _ => throw new ArgumentException($"Unsupported AI model: {options.AIModel}")
                };
            });

            // Register MeetingNotesClient
            services.AddSingleton<MeetingNotesClient>();

            return services;
        }
    }
} 