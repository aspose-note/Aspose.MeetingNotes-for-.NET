# Code Style Guide for Aspose.MeetingNotes

This document defines the coding standards for Aspose.MeetingNotes to ensure consistency, readability, and maintainability.

## General Guidelines
- Use **.NET 6+** features where applicable.
- Write **clean, self-documenting** code with meaningful names.
- Follow the **Single Responsibility Principle (SRP)** and modular design.
- Avoid **magic numbers** and **hardcoded values** – use constants or configuration.

## Formatting
- Use **4 spaces** for indentation (no tabs).
- Keep **line length ≤ 120 characters**.
- Use **camelCase** for local variables and method parameters.
- Use **PascalCase** for class names, methods, and properties.
- Use **ALL_CAPS** for constants.

## Naming Conventions
| Type            | Convention  | Example |
|----------------|------------|---------|
| Classes        | PascalCase  | `MeetingNotesProcessor` |
| Methods        | PascalCase  | `ExtractActionItems()` |
| Properties     | PascalCase  | `AudioFormat` |
| Variables      | camelCase   | `transcriptionResult` |
| Constants      | ALL_CAPS    | `MAX_FILE_SIZE` |
| Interfaces     | PascalCase (I-prefix) | `ITextAnalyzer` |
| Enums          | PascalCase  | `AudioType.WAV` |

## Comments & Documentation
- Use **XML documentation comments (`///`)** for public APIs.
- Use **inline comments (`//`)** sparingly and only when necessary.
- Avoid redundant comments (good code should be self-explanatory).

```csharp
/// <summary>
/// Processes the given audio file and extracts text.
/// </summary>
/// <param name="filePath">Path to the audio file.</param>
/// <returns>Extracted text as a string.</returns>
public string ProcessAudio(string filePath) { ... }
```

## Error Handling
- Use exceptions instead of error codes.
- Catch specific exceptions, not general ones (catch (Exception ex)).
- Provide meaningful error messages and logs.

```csharp
try
{
    var result = audioProcessor.Process(file);
}
catch (AudioProcessingException ex)
{
    logger.LogError($"Audio processing failed: {ex.Message}");
}
```

## Testing
- Follow AAA (Arrange-Act-Assert) in unit tests.
- Use xUnit / NUnit for testing.
- Write isolated and independent tests.

