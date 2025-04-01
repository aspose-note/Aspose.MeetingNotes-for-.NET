# Architecture of Aspose.MeetingNotes-for-.NET

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
            AI[AI Integration]
            AI1[ChatGPT]
            AI2[Grok]
            AI3[DeepSeek]
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
    AI --> AI1
    AI --> AI2
    AI --> AI3

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
- **CLI Tool**: Command-line application for working with the SDK
- **SDK API**: Main interface for integration with other applications

### 2. Core System

#### 2.1 Processing
- **AudioProcessor**: Audio file processing
- **SpeechRecognizer**: Speech recognition using Whisper
- **ContentAnalyzer**: Content analysis and structuring
- **AI Integration**: AI model integration
  - ChatGPT
  - Grok
  - DeepSeek

#### 2.2 Export
- **ContentExporter**: Export to various formats
  - Markdown
  - HTML
  - PDF
  - OneNote

#### 2.3 Tasks
- **ActionExtractor**: Task extraction and management
  - Jira integration
  - Trello integration
  - Azure DevOps integration

### 3. Infrastructure
- **Dependency Injection**: Dependency management
- **Logging**: Logging system
- **Caching**: Result caching
- **Configuration**: Configuration management
- **Monitoring**: System monitoring

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
    ContentAnalyzer->>ActionExtractor: Content for task extraction
    ActionExtractor-->>Client: Tasks
    ContentAnalyzer->>ContentExporter: Content for export
    ContentExporter-->>Client: Exported data
```

## Dependencies

```mermaid
graph LR
    subgraph External["External Dependencies"]
        Whisper[Whisper.NET]
        AsposeNote[Aspose.Note]
        AsposeHtml[Aspose.Html]
        OpenAI[OpenAI API]
        GrokAPI[Grok API]
        DeepSeekAPI[DeepSeek API]
    end

    subgraph Internal["Internal Dependencies"]
        Core[Core SDK]
        CLI[CLI Tool]
        Tests[Tests]
    end

    Core --> Whisper
    Core --> AsposeNote
    Core --> AsposeHtml
    Core --> OpenAI
    Core --> GrokAPI
    Core --> DeepSeekAPI
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
            D3[Audit]
        end
    end

    Authentication --> Data
``` 