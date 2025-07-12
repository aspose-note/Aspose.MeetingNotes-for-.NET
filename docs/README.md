# Aspose.MeetingNotes for .NET

SDK for converting meeting audio into structured text notes and actionable tasks using .NET.

**Aspose.MeetingNotes for .NET** analyzes audio recordings of meetings, leveraging external tools and AI models to provide comprehensive summaries and structured data. The process involves:

1.  **Audio Conversion:** Input audio files (various formats supported) are standardized using FFmpeg.
2.  **Transcription:** Speech is converted to text using the Whisper model via Whisper.net.
3.  **AI Analysis:** The transcript is processed by a configured AI model (via `ApiBasedAIModel`) using a unified configuration structure (`AIModelOptions`) to extract key information.
4.  **Structured Output:** The analysis yields summaries, key points, decisions, discussed topics, Q&A pairs, and identified action items.
5.  **Export:** Results can be exported into various formats including Markdown, HTML, PDF, and OneNote. Export to HTML, PDF, and OneNote utilizes Aspose components and may require a valid Aspose license for full functionality.

The library is designed to be configurable (e.g., choice of AI model, language) and extensible. A command-line interface (CLI) tool is also included for convenient processing without writing code.

## Requirements

### FFmpeg

The library uses FFmpeg for audio file conversion. FFmpeg must be installed on the system. The path to the FFmpeg executable **must be configured** using one of the following methods:
* The `--ffmpeg-path` command-line argument (CLI tool only).
* The `FFMPEG_PATH` environment variable.
* The `FfMpegPath` property set in a JSON configuration file or programmatically via `MeetingNotesOptions`.

### Whisper & AI Models

* **Whisper Model:** Requires a Whisper speech recognition model (GGUF format). By default, the `base` model is downloaded automatically if needed. Model size or path can be configured via `SpeechRecognitionOptions`.
* **AI Model:** Requires configuration for the AI model using `AIModelOptions`, which includes endpoint URL, model name, API key, temperature, top-p sampling, max tokens, and other parameters. This unified model interacts with external LLM APIs like LLaMA.cpp, DeepSeek, or compatible services. You may also provide a custom `IAIModel` implementation.

### Aspose Components & Licensing
For exporting meeting notes to PDF, HTML, and OneNote formats, Aspose.MeetingNotes utilizes powerful components from the Aspose library suite (specifically Aspose.Note). To use these export features without evaluation limitations (such as watermarks), a valid Aspose license file (e.g., `Aspose.Total.NET.lic`) is required.

The path to your Aspose license file can be configured via the `AsposeLicensePath` property in `MeetingNotesOptions`. This can be set using:

* The `--aspose-license` (or `--license`) command-line argument when using the CLI tool.
* The `ASPOSE_LICENSE_PATH` environment variable.
* The `AsposeLicensePath` property in a JSON configuration file.
* Programmatically when configuring `MeetingNotesOptions` in your code.

If a valid license is not provided or the specified file is not found, the Aspose components will operate in **evaluation mode**. Export to Markdown format does **not** require an Aspose license.

### Supported Audio Formats

The library supports all audio formats that the configured FFmpeg version can handle, including but not limited to:
* WAV (.wav)
* OGG (.ogg)
* MP3 (.mp3)
* M4A (.m4a)
* FLAC (.flac)
* And many more...

All audio files are automatically converted to WAV format with a sample rate of 16 kHz and mono channel before being passed to Whisper.

## Installation

```bash
dotnet add package Aspose.MeetingNotes 
```

## Usage (Library)

To use the library within your application, integration with `Microsoft.Extensions.DependencyInjection` is recommended:

1.  **Register Services:** Configure your `IServiceCollection` by adding necessary services like Logging (e.g., `services.AddLogging(...)`) and HttpClient (`services.AddHttpClient()`).
2.  **Add MeetingNotes:** Call the `services.AddMeetingNotes(options => { ... });` extension method. Within the configuration action (`options => { ... }`), you **must** provide the required `options.FfMpegPath`.  
    You also need to configure the desired AI model by setting `options.AIModel` with an instance of `AIModelOptions`. The following properties are supported:

    * `Url`: API endpoint URL for the AI model server (required).
    * `ModelName`: Model identifier to use with the AI API (required for APIs supporting multiple models).
    * `ApiKey`: API key for authenticating with the AI model server (if required).
    * `Temperature`, `TopP`, `MaxTokens`: Optional tuning parameters for controlling generation behavior.

    Optionally, provide a custom `IAIModel` via `options.CustomAIModel`.  
    Optionally, configure other settings like `options.Language` or `options.SpeechRecognition`.  

    To enable full functionality for PDF, HTML, or OneNote exports, provide the path to your Aspose license file via `options.AsposeLicensePath = "path/to/your/license.lic";`.  

    Alternatively, provide your own implementations via `options.CustomAIModel` or `options.CustomSpeechRecognizer`.
3.  **Build Provider:** Create the `ServiceProvider` from the `IServiceCollection`.
4.  **Resolve Client:** Obtain an instance of `MeetingNotesClient` using `serviceProvider.GetRequiredService<MeetingNotesClient>()`.
5.  **Prepare Input:** Create a `System.IO.FileInfo` object pointing to the audio file you want to process.
6.  **Process:** Call the `meetingNotesClient.ProcessMeetingAsync(fileInfo)` method, passing the `FileInfo` object. This asynchronous method returns a `MeetingAnalysisResult`.
7.  **Handle Results/Errors:** Check the `Success` property of the returned `MeetingAnalysisResult`. If `false`, check the `ErrorMessage`. Handle potential exceptions derived from `MeetingNotesException` (e.g., `AudioProcessingException`, `TranscriptionException`, `AIModelException`) that might be thrown during processing.
8.  **Export:** Use the `meetingNotesClient.ExportAsync(result.Content, result.ActionItems, format)` method to get the processed notes in the desired `ExportFormat` (e.g., Markdown).

## Command-Line Interface (CLI)

A CLI tool (`Aspose.MeetingNotes.CLI`) is provided for direct use. Build the `Aspose.MeetingNotes.CLI` project to get the executable.

The main command is `process`. Key arguments:
* `--file`/`-f` (**Required**): Path to the input audio file.
* `--ffmpeg-path`: Path to the FFmpeg executable (overrides other methods).
* `--aspose-license` / `--license`: Path to the Aspose license file (e.g., for PDF, HTML, OneNote export). Overrides config and environment variable.
* `--config`/`-c`: Path to a JSON configuration file (optional).
* `--ai-url`: URL for the external AI model server (used if not specified in config).
* `--model-name`: AI model name for the external API.
* `--api-key`: API key for authenticating with the external AI model server.
* `--language`/`-l`: Language code (optional).
* `--output`/`-o`: Output format (optional, defaults to markdown. Supported: markdown, pdf, html, onenote).

Run with `--help` for details.

### Example Usage

```bash
dotnet Aspose.MeetingNotes.CLI.dll process --file meeting.mp3 --ffmpeg-path "C:\Tools\ffmpeg.exe" --ai-url http://localhost:8080/v1 --model-name llama-3-8b-instruct --api-key your-api-key --output pdf
```

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.