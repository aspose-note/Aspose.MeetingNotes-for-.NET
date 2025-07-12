using System.CommandLine;
using System.Text.Json;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.DependencyInjection;
using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Progress;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.CLI;

/// <summary>
/// Main entry point for the Aspose.MeetingNotes Command Line Interface.
/// Parses command line arguments, configures services, and orchestrates meeting processing.
/// </summary>
internal static class Program
{
    private const string DefaultLanguage = "en";
    private const string DefaultOutputFormat = "markdown";
    private const string DefaultAiUrl = "http://localhost:8080/v1";
    private const string DefaultFfmpegPath = @"C:\ProgramData\chocolatey\bin\ffmpeg.exe";
    private const string DefaultWhisperModelSize = "base";

    /// <summary>
    /// The main entry point of the CLI application.
    /// </summary>
    /// <param name="args">Command line arguments passed to the application.</param>
    /// <returns>Exit code (0 for success, non-zero for errors).</returns>
    static async Task<int> Main(string[] args)
    {
        var fileOption = CreateFileOption();
        var languageOption = CreateLanguageOption();
        var outputFormatOption = CreateOutputFormatOption();
        var configOption = CreateConfigOption();
        var aiUrlOption = CreateAiUrlOption();
        var modelNameOption = CreateModelNameOption();
        var apiKeyOption = CreateApiKeyOption();
        var ffmpegPathOption = CreateFfmpegPathOption();
        var inputFileOption = CreateInputFileOption();
        var asposeLicensePathOption = CreateAsposeLicensePathOption();

        var processCommand = new Command("process", "Process a meeting recording file to generate notes and action items.")
        {
            fileOption,
            languageOption,
            outputFormatOption,
            configOption,
            aiUrlOption,
            modelNameOption,
            apiKeyOption,
            ffmpegPathOption,
            asposeLicensePathOption
        };

        var exportCommand = new Command("export", "Export previously analyzed meeting notes from JSON to a different format (Not fully implemented).")
        {
            inputFileOption,
            outputFormatOption
        };

        var rootCommand = new RootCommand("Aspose.MeetingNotes CLI tool for processing meeting recordings.");
        rootCommand.AddCommand(processCommand);
        rootCommand.AddCommand(exportCommand);

        processCommand.SetHandler(async (context) =>
        {
            var file = context.ParseResult.GetValueForOption(fileOption)!;
            var language = context.ParseResult.GetValueForOption(languageOption)!;
            var outputFormat = context.ParseResult.GetValueForOption(outputFormatOption)!;
            var config = context.ParseResult.GetValueForOption(configOption);
            var aiUrl = context.ParseResult.GetValueForOption(aiUrlOption)!;
            var modelName = context.ParseResult.GetValueForOption(modelNameOption);
            var apiKey = context.ParseResult.GetValueForOption(apiKeyOption);
            var ffmpegPath = context.ParseResult.GetValueForOption(ffmpegPathOption);
            var asposeLicensePath = context.ParseResult.GetValueForOption(asposeLicensePathOption);

            context.ExitCode = await HandleProcessCommandAsync(file, language, outputFormat, config, aiUrl, modelName, apiKey, ffmpegPath, asposeLicensePath);
        });

        exportCommand.SetHandler(async (context) => {
            var inputFile = context.ParseResult.GetValueForOption(inputFileOption)!;
            var outputFormat = context.ParseResult.GetValueForOption(outputFormatOption)!;

            context.ExitCode = await HandleExportCommandAsync(inputFile, outputFormat);
        });

        return await rootCommand.InvokeAsync(args);
    }

    private static Option<FileInfo> CreateFileOption()
    {
        var option = new Option<FileInfo>(
            aliases: ["--file", "-f"],
            description: "Path to the input audio file to process.") {
            IsRequired = true
        };
        option.ExistingOnly();
        return option;
    }

    private static Option<string> CreateLanguageOption() =>
        new Option<string>(
            aliases: ["--language", "-l"],
            getDefaultValue: () => DefaultLanguage,
            description: "Language of the recording (e.g., 'en', 'ru', 'auto').");

    private static Option<string> CreateOutputFormatOption()
    {
        var option = new Option<string>(
            aliases: ["--output", "-o"],
            getDefaultValue: () => DefaultOutputFormat,
            description: "Output format for the generated notes.");

        option.FromAmong("markdown", "md", "pdf", "html", "onenote", "one");
        return option;
    }

    private static Option<FileInfo?> CreateConfigOption()
    {
        var option = new Option<FileInfo?>(
            aliases: ["--config", "-c"],
            description: "Path to a JSON configuration file to load options.");
        option.ExistingOnly();
        return option;
    }

    private static Option<string> CreateAiUrlOption() =>
        new Option<string>(
            name: "--ai-url",
            getDefaultValue: () => DefaultAiUrl,
            description: "URL of the AI model API endpoint used by the default model.");

    private static Option<string?> CreateModelNameOption() =>
        new Option<string?>(
            name: "--model-name",
            description: "Name of the AI model to use (overrides config file).");

    private static Option<string?> CreateApiKeyOption() =>
        new Option<string?>(
            name: "--api-key",
            description: "Optional API key to override configuration for the AI model.");

    private static Option<FileInfo> CreateInputFileOption()
    {
        var option = new Option<FileInfo>(
             aliases: ["--input", "-i"],
             description: "Path to the input JSON file containing meeting analysis data (for export).") { IsRequired = true };
        option.ExistingOnly();
        return option;
    }

    private static Option<string?> CreateFfmpegPathOption() =>
        new Option<string?>(
            name: "--ffmpeg-path",
            description: "Full path to the ffmpeg executable. Overrides config file and environment variable.");

    private static Option<string?> CreateAsposeLicensePathOption() =>
        new(
            aliases: ["--aspose-license", "--license"],
            description: "Full path to the Aspose license file (.lic). Overrides config file and environment variable. Required for PDF, HTML, OneNote export.");

    /// <summary>
    /// Handles the logic for the 'process' command.
    /// </summary>
    private static async Task<int> HandleProcessCommandAsync(
        FileInfo file,
        string language,
        string outputFormat,
        FileInfo? config,
        string aiUrl,
        string? modelName,
        string? apiKey,
        string? ffmpegPathFromCli,
        string? asposeLicensePathFromCli)
    {
        IServiceProvider serviceProvider;
        ILogger logger;

        try
        {
            serviceProvider = ConfigureServices(config, aiUrl, modelName, apiKey, language, ffmpegPathFromCli, asposeLicensePathFromCli);
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            logger = loggerFactory.CreateLogger("Aspose.MeetingNotes.CLI.Process");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERROR] Failed to configure application services: {ex.Message}");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }

        // Resolve the main client
        var client = serviceProvider.GetRequiredService<MeetingNotesClient>();

        // Setup progress reporting to console
        var progress = new Progress<ProcessingProgress>(p => {
            Console.WriteLine($"[{p.Stage,-15}] {p.ProgressPercentage,3}%: {p.StatusMessage}");
        });

        try
        {
            logger.LogInformation("Processing file: {FilePath}", file.FullName);

            // Process the meeting
            var analysisResult = await client.ProcessMeetingAsync(file, progress, CancellationToken.None);

            if (!analysisResult.Success)
            {
                logger.LogError("Meeting processing failed: {ErrorMessage}", analysisResult.ErrorMessage);
                Console.Error.WriteLine($"[ERROR] Processing failed: {analysisResult.ErrorMessage}");
                return 1;
            }

            logger.LogInformation("Meeting processed successfully. Exporting results...");

            // Parse export format
            var exportFormat = ParseExportFormat(outputFormat);

            // Export the result
            var exportResult = await client.ExportAsync(
                analysisResult.Content,
                analysisResult.ActionItems,
                exportFormat,
                CancellationToken.None);

            // Determine output path
            var outputExtension = GetFileExtension(exportFormat);
            var outputPath = Path.ChangeExtension(file.FullName, outputExtension);

            // Save the output
            if (exportResult.Data is not null)
            {
                await File.WriteAllBytesAsync(outputPath, exportResult.Data);
                logger.LogInformation("Output saved as binary file: {OutputPath}", outputPath);
            }
            else if (exportResult.Text is not null)
            {
                await File.WriteAllTextAsync(outputPath, exportResult.Text);
                logger.LogInformation("Output saved as text file: {OutputPath}", outputPath);
            }
            else
            {
                logger.LogWarning("Export result contained no data for format {Format}", exportFormat);
                Console.WriteLine($"[Warning] Export completed but no data generated for format {exportFormat}.");
            }

            Console.WriteLine($"Processing complete. Output saved to: {outputPath}");
            return 0;
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Configuration or input error during processing");
            Console.Error.WriteLine($"[ERROR] Input Error: {ex.Message}");
            return 1;
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "File not found during processing");
            Console.Error.WriteLine($"[ERROR] File Not Found: {ex.Message}");
            return 1;
        }
        catch (AudioProcessingException ex)
        {
            logger.LogError(ex, "Audio processing failed");
            Console.Error.WriteLine($"[ERROR] Audio Processing Error: {ex.Message}");
            return 1;
        }
        catch (TranscriptionException ex)
        {
            logger.LogError(ex, "Transcription failed");
            Console.Error.WriteLine($"[ERROR] Transcription Error: {ex.Message}");
            return 1;
        }
        catch (AIModelException ex)
        {
            logger.LogError(ex, "AI analysis failed");
            Console.Error.WriteLine($"[ERROR] AI Error: {ex.Message}");
            return 1;
        }
        catch (NotSupportedException ex)
        {
            logger.LogError(ex, "Operation not supported");
            Console.Error.WriteLine($"[ERROR] Not Supported: {ex.Message}");
            return 1;
        }
        catch (NotImplementedException ex)
        {
            logger.LogError(ex, "Functionality not implemented");
            Console.Error.WriteLine($"[ERROR] Not Implemented: {ex.Message}");
            return 1;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Processing operation was cancelled");
            Console.WriteLine("[Cancelled]");
            return 2;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An unexpected error occurred during processing");
            Console.Error.WriteLine($"[CRITICAL ERROR] Unexpected Error: {ex.Message}");
            Console.Error.WriteLine("Check logs for more details.");
            return 1;
        }
    }

    /// <summary>
    /// Handles the logic for the 'export' command (placeholder).
    /// </summary>
    private static Task<int> HandleExportCommandAsync(FileInfo inputFile, string outputFormat)
    {
        // Proper implementation would involve:
        // 1. Deserializing 'inputFile' (JSON) into MeetingAnalysisResult.
        // 2. Configuring services (potentially just IContentExporter and ILogger).
        // 3. Resolving IContentExporter.
        // 4. Calling ExportAsync with the deserialized data and parsed format.
        // 5. Saving the ExportResult.
        // 6. Proper error handling.

        Console.WriteLine($"Exporting {inputFile.Name} to {outputFormat} format...");
        Console.WriteLine("[Warning] Export command is not yet fully implemented.");
        // Return success (0) or error code? Returning error code (1) for unimplemented feature.
        return Task.FromResult(1);
    }

    /// <summary>
    /// Configures and builds the service provider for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices(
        FileInfo? configFile,
        string aiUrl,
        string? modelName,
        string? apiKey,
        string language,
        string? ffmpegPathFromCli,
        string? asposeLicensePathFromCli)
    {
        var services = new ServiceCollection();

        // Add Logging (using simple console logging for CLI)
        services.AddLogging(builder => {
            builder.AddConsole()
                   .SetMinimumLevel(LogLevel.Information);
        });

        // Create a temporary logger factory to use during configuration loading
        var tempLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var configLogger = tempLoggerFactory.CreateLogger("Aspose.MeetingNotes.CLI.Configuration");

        // Load configuration options, handling potential errors
        MeetingNotesOptions options = LoadMeetingNotesOptions(configFile, aiUrl, modelName, apiKey, ffmpegPathFromCli, asposeLicensePathFromCli, configLogger);

        // Override language from command line argument if different from default/config
        if (!string.IsNullOrWhiteSpace(language) && !language.Equals(options.Language, StringComparison.OrdinalIgnoreCase))
        {
            configLogger.LogInformation("Overriding language from command line: '{Language}' (was '{OriginalLanguage}')", language, options.Language);
            options.Language = language;
        }

        // Add MeetingNotes core services using the configured options
        services.AddMeetingNotes(opt =>
        {
            opt.Language = options.Language;
            opt.FfMpegPath = options.FfMpegPath;
            opt.AsposeLicensePath = options.AsposeLicensePath;
            opt.SpeechRecognition = options.SpeechRecognition;
            opt.AIModel = options.AIModel;
        });

        // Dispose the temporary logger factory
        tempLoggerFactory.Dispose();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Loads MeetingNotesOptions from a config file or sets defaults.
    /// </summary>
    private static MeetingNotesOptions LoadMeetingNotesOptions(
        FileInfo? configFile,
        string aiUrl,
        string? modelName,
        string? apiKey,
        string? ffmpegPathFromCli,
        string? asposeLicensePathFromCli,
        ILogger configLogger)
    {
        MeetingNotesOptions options;

        // Load base options from file if provided
        if (configFile != null)
        {
            configLogger.LogInformation("Attempting to load configuration from: {ConfigFile}", configFile.FullName);
            try
            {
                using (JsonDocument jsonDoc = JsonDocument.Parse(File.ReadAllText(configFile.FullName)))
                {
                    options = jsonDoc.Deserialize<MeetingNotesOptions>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? CreateDefaultOptions();
                    if (jsonDoc.RootElement.TryGetProperty("AIModel", out JsonElement aiModelElement) && aiModelElement.ValueKind != JsonValueKind.Null)
                    {
                        configLogger.LogInformation("AIModel section found in configuration file. Type: {ModelType}", options.AIModel?.GetType().Name ?? "Unknown");
                    }
                    else
                    {
                        configLogger.LogInformation("AIModel section not found or is null in configuration file");
                    }
                }
                configLogger.LogInformation("Successfully loaded initial configuration from file");
            }
            catch (Exception ex)
            {
                configLogger.LogError(ex, "Error loading/parsing configuration from '{ConfigFile}'. Using defaults.", configFile.FullName);
                options = CreateDefaultOptions();
            }
        }
        else
        {
            configLogger.LogInformation("No configuration file provided. Using defaults and command-line arguments");
            options = CreateDefaultOptions();
        }

        // Ffmpeg Path Precedence: CLI > Env Var > Config File > Hardcoded Default
        string? ffmpegPathFromEnv = Environment.GetEnvironmentVariable("FFMPEG_PATH");
        options.FfMpegPath = // Assign final value based on priority
            !string.IsNullOrWhiteSpace(ffmpegPathFromCli) ? ffmpegPathFromCli : // Use CLI if provided
            !string.IsNullOrWhiteSpace(ffmpegPathFromEnv) ? ffmpegPathFromEnv : // Else use Env Var if provided
            !string.IsNullOrWhiteSpace(options.FfMpegPath) ? options.FfMpegPath : // Else use value from Config (or its default if config didn't set it)
            DefaultFfmpegPath; // Else use hardcoded default
        configLogger.LogInformation("Final Ffmpeg Path set to: {Path}", options.FfMpegPath);

        // Priority for AsposeLicensePath: CLI > Env Var > Config File > Hardcoded Default (null)
        string? asposeLicensePathFromEnv = Environment.GetEnvironmentVariable("ASPOSE_LICENSE_PATH");
        options.AsposeLicensePath =
            !string.IsNullOrWhiteSpace(asposeLicensePathFromCli) ? asposeLicensePathFromCli :
            !string.IsNullOrWhiteSpace(asposeLicensePathFromEnv) ? asposeLicensePathFromEnv :
            !string.IsNullOrWhiteSpace(options.AsposeLicensePath) ? options.AsposeLicensePath :
            null; // Default is null
        configLogger.LogInformation("Final Aspose License Path set to: {Path}", options.AsposeLicensePath ?? "Not set");

        // Ensure AIModel is always present
        options.AIModel ??= new AIModelOptions();

        if (!string.IsNullOrWhiteSpace(aiUrl))
        {
            options.AIModel.Url = aiUrl;
            configLogger.LogInformation("AIModel URL overridden from CLI: {Url}", aiUrl);
        }

        // Apply CLI model name if provided
        if (!string.IsNullOrWhiteSpace(modelName))
        {
            options.AIModel.ModelName = modelName;
            configLogger.LogInformation("Model name overridden from CLI: {ModelName}", modelName);
        }

        // Apply CLI API key if provided
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            options.AIModel.ApiKey = apiKey;
            configLogger.LogInformation("API key overridden from CLI: {ApiKey}", apiKey);
        }

        // Ensure other essential defaults if not loaded
        options.SpeechRecognition ??= new SpeechRecognitionOptions { ModelSize = DefaultWhisperModelSize };
        options.Language ??= DefaultLanguage; // Language might be overridden again by CLI arg in ConfigureServices

        return options;
    }

    private static MeetingNotesOptions CreateDefaultOptions()
    {
        return new MeetingNotesOptions
        {
            FfMpegPath = DefaultFfmpegPath,
            SpeechRecognition = new SpeechRecognitionOptions { ModelSize = DefaultWhisperModelSize },
            Language = DefaultLanguage,
            AIModel = new AIModelOptions { Url = DefaultAiUrl }
        };
    }

    /// <summary>
    /// Parses the export format string (case-insensitive).
    /// </summary>
    private static ExportFormat ParseExportFormat(string format) => format.ToLowerInvariant() switch
    {
        "markdown" or "md" => ExportFormat.Markdown,
        "pdf" => ExportFormat.PDF,
        "html" => ExportFormat.HTML,
        "onenote" or "one" => ExportFormat.OneNote,
        _ => throw new ArgumentException($"Unsupported export format: '{format}'. Supported: markdown/md, pdf, html, onenote/one")
    };

    /// <summary>
    /// Gets the standard file extension for a given export format.
    /// </summary>
    private static string GetFileExtension(ExportFormat format) => format switch
    {
        ExportFormat.Markdown => ".md",
        ExportFormat.PDF => ".pdf",
        ExportFormat.HTML => ".html",
        ExportFormat.OneNote => ".one",
        _ => throw new ArgumentOutOfRangeException(nameof(format), $"Unsupported export format enum value: {format}")
    };
}
