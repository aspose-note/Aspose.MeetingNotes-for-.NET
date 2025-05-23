
# Aspose.MeetingNotes Architecture

## General Architecture

```mermaid
graph TB
    subgraph Client["Client Layer"]
        CLI[CLI Tool]
        SDK[SDK API]
    end

    subgraph Core["Core System"]
        direction TB
        subgraph Processing["Processing"]
            AP[AudioProcessor]
            SR[SpeechRecognizer]
            CA[ContentAnalyzer]
            AI[AI Integration: ApiBasedAIModel]
        end

        subgraph Export["Export"]
            CE[ContentExporter]
            E1[Markdown]
            E2[HTML]
            E3[PDF]
            E4[OneNote]
        end

        subgraph Tasks["Tasks"]
            AE[ActionExtractor]
            T1[Jira]
            T2[Trello]
            T3[Azure DevOps]
        end
    end

    subgraph Infrastructure["Infrastructure"]
        DI[Dependency Injection]
        Log[Logging]
        Cache[Caching]
        Config[Configuration]
        Monitor[Monitoring]
    end

    Client --> Core
    Core --> Infrastructure

    AP --> SR
    SR --> CA
    CA --> AI

    CA --> CE
    CE --> E1
    CE --> E2
    CE --> E3
    CE --> E4

    CA --> AE
    AE --> T1
    AE --> T2
    AE --> T3
```

## System Components

### 1. Client Layer
- **CLI Tool**: Command-line interface for processing and exporting meeting notes
- **SDK API**: Entry point for external integrations

### 2. Core System

#### 2.1 Processing
- **AudioProcessor**: Handles audio preprocessing using `ffmpeg`
- **SpeechRecognizer**: Performs transcription using Whisper
- **ContentAnalyzer**: Analyzes transcribed text for structure and insights
- **AI Integration**: Uses `ApiBasedAIModel` with configurable `AIModelOptions`

#### 2.2 Export
- **ContentExporter**: Converts analysis results into various formats
  - Markdown
  - HTML
  - PDF
  - OneNote

#### 2.3 Tasks
- **ActionExtractor**: Extracts tasks and action items from content
  - ⚠ Planned: Integrations with Jira, Trello, Azure DevOps

### 3. Infrastructure
- **Dependency Injection**: Service configuration and wiring
- **Logging**: Centralized logging via `ILogger`
- **Caching**: Placeholder for potential performance improvements
- **Configuration**: Supports JSON file and environment/CLI overrides
- **Monitoring**: ⚠ Planned: runtime health and diagnostics

## Data Flow

```mermaid
sequenceDiagram
    participant Client
    participant AudioProcessor
    participant SpeechRecognizer
    participant ContentAnalyzer
    participant AI
    participant ContentExporter
    participant ActionExtractor

    Client->>AudioProcessor: Audio file
    AudioProcessor->>SpeechRecognizer: Processed audio
    SpeechRecognizer->>ContentAnalyzer: Transcription
    ContentAnalyzer->>AI: Text for analysis
    AI-->>ContentAnalyzer: Analyzed content
    ContentAnalyzer->>ActionExtractor: Extract tasks
    ActionExtractor-->>Client: Tasks
    ContentAnalyzer->>ContentExporter: Export
    ContentExporter-->>Client: Exported file
```

## Dependencies

```mermaid
graph LR
    subgraph External["External Dependencies"]
        Whisper[Whisper.NET]
        AsposeNote[Aspose.Note]
        AsposeHtml[Aspose.Html]
        LLM_API[Generic LLM API via ApiBasedAIModel]
    end

    subgraph Internal["Internal Dependencies"]
        Core[Core SDK]
        CLI[CLI Tool]
        Tests[Tests]
    end

    Core --> Whisper
    Core --> AsposeNote
    Core --> AsposeHtml
    Core --> LLM_API
    CLI --> Core
    Tests --> Core
```

## Scalability

```mermaid
graph TB
    subgraph Scalability["Scalability"]
        direction LR
        subgraph Horizontal["Horizontal Scaling"]
            H1[Audio Processing]
            H2[AI Analysis]
            H3[Export]
        end

        subgraph Vertical["Vertical Scaling"]
            V1[Caching]
            V2[Memory Optimization]
            V3[Parallel Processing]
        end
    end

    Horizontal --> Vertical
```

## Security

```mermaid
graph TB
    subgraph Security["Security"]
        direction LR
        subgraph Authentication["Authentication"]
            A1[API Keys]
            A2[OAuth]
            A3[SSO]
        end

        subgraph Data["Data Protection"]
            D1[Encryption]
            D2[Secure Storage]
            D3[Audit Logging]
        end
    end

    Authentication --> Data
```
