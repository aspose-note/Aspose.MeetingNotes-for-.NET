using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Aspose.MeetingNotes.Configuration;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Progress;
using Aspose.MeetingNotes.DependencyInjection;

namespace Aspose.MeetingNotes.CLI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Aspose.MeetingNotes CLI tool for processing meeting recordings");

            // Process command
            var processCommand = new Command("process", "Process a meeting recording");
            var fileOption = new Option<FileInfo>(
                "--file",
                "The audio file to process")
            { IsRequired = true };
            var languageOption = new Option<string>(
                "--language",
                () => "en",
                "The language of the recording");
            var outputOption = new Option<string>(
                "--output",
                () => "markdown",
                "Output format (markdown, pdf, html, onenote)");
            var configOption = new Option<FileInfo>(
                "--config",
                "Path to configuration file");
            var llamaUrlOption = new Option<string>(
                "--llama-url",
                () => "http://localhost:8080/v1",
                "URL of the LLaMA API server");

            processCommand.AddOption(fileOption);
            processCommand.AddOption(languageOption);
            processCommand.AddOption(outputOption);
            processCommand.AddOption(configOption);
            processCommand.AddOption(llamaUrlOption);

            processCommand.SetHandler(async (file, language, output, config, llamaUrl) =>
            {
                await ProcessMeeting(file, language, output, config, llamaUrl);
            }, fileOption, languageOption, outputOption, configOption, llamaUrlOption);

            // Export command
            var exportCommand = new Command("export", "Export meeting notes to different format");
            var inputOption = new Option<FileInfo>(
                "--input",
                "The input JSON file containing meeting analysis")
            { IsRequired = true };
            
            exportCommand.AddOption(inputOption);
            exportCommand.AddOption(outputOption);

            exportCommand.SetHandler(async (input, output) =>
            {
                await ExportNotes(input, output);
            }, inputOption, outputOption);

            rootCommand.AddCommand(processCommand);
            rootCommand.AddCommand(exportCommand);

            return await rootCommand.InvokeAsync(args);
        }

        private static async Task ProcessMeeting(FileInfo file, string language, string output, FileInfo? config, string llamaUrl)
        {
            try
            {
                // Setup services
                var services = new ServiceCollection();
                
                // Add logging
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // Configure from file if provided
                var options = LoadConfiguration(config, llamaUrl);

                // Add MeetingNotes services
                services.AddMeetingNotes(opt =>
                {
                    opt.Language = language;
                    opt.CustomAIModel = options.CustomAIModel;
                    opt.SpeechRecognition = options.SpeechRecognition;
                });

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<MeetingNotesClient>();

                // Setup progress reporting
                var progress = new Progress<ProcessingProgress>(p =>
                {
                    Console.WriteLine($"{p.Stage}: {p.ProgressPercentage}% - {p.StatusMessage}");
                });

                // Process the file
                using var audioStream = file.OpenRead();
                var analysisResult = await client.ProcessMeetingAsync(
                    audioStream,
                    file.Extension,
                    progress);

                // Export the result
                var exportFormat = ParseExportFormat(output);
                var exportResult = await client.ExportAsync(
                    analysisResult.Content,
                    analysisResult.ActionItems,
                    exportFormat);

                // Save the output
                var outputPath = Path.ChangeExtension(file.FullName, GetFileExtension(exportFormat));
                if (exportResult.Data != null)
                {
                    await File.WriteAllBytesAsync(outputPath, exportResult.Data);
                }
                else if (exportResult.Text != null)
                {
                    await File.WriteAllTextAsync(outputPath, exportResult.Text);
                }

                Console.WriteLine($"Successfully processed and exported to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static async Task ExportNotes(FileInfo input, string output)
        {
            try
            {
                // Implementation of export command
                Console.WriteLine($"Exporting {input.Name} to {output} format...");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static MeetingNotesOptions LoadConfiguration(FileInfo? config, string llamaUrl)
        {
            var options = new MeetingNotesOptions();

            if (config != null && config.Exists)
            {
                options = JsonSerializer.Deserialize<MeetingNotesOptions>(
                    File.ReadAllText(config.FullName)) ?? options;
            }

            // Configure LLaMA model
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            options.CustomAIModel = new AsposeLlamaModel(
                llamaUrl,
                loggerFactory.CreateLogger<AsposeLlamaModel>());

            // Configure speech recognition
            options.SpeechRecognition = new SpeechRecognitionOptions
            {
                ModelSize = "base"
            };

            return options;
        }

        private static ExportFormat ParseExportFormat(string format) => format.ToLower() switch
        {
            "markdown" => ExportFormat.Markdown,
            "pdf" => ExportFormat.PDF,
            "html" => ExportFormat.HTML,
            "onenote" => ExportFormat.OneNote,
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };

        private static string GetFileExtension(ExportFormat format) => format switch
        {
            ExportFormat.Markdown => ".md",
            ExportFormat.PDF => ".pdf",
            ExportFormat.HTML => ".html",
            ExportFormat.OneNote => ".one",
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };
    }
} 
