# Aspose.MeetingNotes for .NET

Aspose.MeetingNotes is a .NET SDK for converting meeting audio recordings into structured notes and tasks. The project is under active development.

## Features

- 🎯 **Speech Recognition** – Transcribe meetings using Whisper.NET
- 📝 **Content Structuring** – Extract key points and summarize discussions
- ✅ **Task Extraction** – Identify action items and deadlines
- 📄 **Export** – Save notes in Markdown, HTML, OneNote, and PDF formats
- 🤖 **AI Integration** – Enhance analysis using AI models (ChatGPT, Grok, DeepSeek)
- 🔧 **Modular Architecture** – Flexible components for easy extension

## Current Status

### Implemented
- Basic audio processing
- Whisper.NET integration
- Support for multiple audio formats
- Basic logging system
- Initial ChatGPT integration
- Markdown export

### In Development
- Grok and DeepSeek integration
- OneNote and PDF export
- CLI tool
- Testing system
- Performance optimization

## Project Structure

- `src/AudioProcessing/` – Audio processing and preprocessing
- `src/SpeechRecognition/` – Speech-to-text conversion
- `src/AIIntegration/` – AI model integration
- `src/ContentAnalysis/` – Content analysis and structuring
- `src/ActionItems/` – Task identification and assignment
- `src/Exporters/` – Export to various formats
- `src/Utilities/` – Helper functions

## Technical Stack

- **.NET 6+** – Core framework
- **Whisper.NET** – Speech recognition
- **Aspose.Note & Aspose.Html** – Export functionality
- **AI Models** – Integration with ChatGPT, Grok, and DeepSeek

## Code Style

We follow **.NET Coding Guidelines** and best practices for maintainable and readable code:

- Use `camelCase` for local variables and method parameters
- Use `PascalCase` for method names, class names, and properties
- Keep methods concise and focused on a single responsibility
- Follow **SOLID** principles for maintainable architecture
- Use `async/await` for asynchronous operations

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

## Documentation

- [Roadmap](roadmap.md) – Project development plan
- [Architecture](architecture.md) – Architecture description
- [Examples](examples/) – Code examples
- [Code Style](CODE_STYLE.md) – Code formatting rules
- [Contributing](CONTRIBUTING.md) – How to contribute

## Feedback

We welcome feedback and contributions to the project. Please use:
- Issues for bug reports and feature requests
- Pull Requests for code changes
- Discussions for questions and discussions

---

Stay tuned for updates! 🚀
