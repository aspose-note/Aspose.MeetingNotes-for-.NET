# Aspose.MeetingNotes Roadmap

## Core Features

- [x] Command-line interface with `process` command
- [x] Audio preprocessing via `ffmpeg`
- [x] Transcription using Whisper (`whisper.cpp`)
- [x] AI-based analysis and action item extraction via `ApiBasedAIModel`
- [x] Model behavior customization via `AIModelOptions` (`temperature`, `top_p`, etc.)
- [x] Output generation in Markdown, HTML, PDF, OneNote (via `--output`)
- [x] JSON configuration file support and CLI overrides
- [x] Aspose license support for proprietary export formats

## Quality and Tooling

- [x] Unit tests for content analysis, export, and configuration validation
- [x] GitHub Actions CI: build and test on push to `master`
- [x] CLI logging and progress output

## In Progress

- [ ] Initial implementation of `export` command for re-exporting saved analysis results
- [ ] Improved diagnostics for malformed or invalid AI responses

## Planned Features

- [ ] Enhanced speech recognition with speaker diarization
- [ ] Custom vocabulary support for domain-specific meeting contexts
- [ ] Enhanced task extraction and management capabilities
- [ ] Integration with task trackers for automated task creation
- [ ] Memory optimization for handling large audio files
- [ ] Publication of the core library as a NuGet package
