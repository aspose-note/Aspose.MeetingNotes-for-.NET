
# Aspose.LLApi

## Overview

**Aspose.LLApi** is a robust .NET library designed for interacting seamlessly with Large Language Model (LLM) backends. It provides intuitive clients for various operations such as Chat Completion, Tokenization, Embeddings, Document Reranking, Slots management, LoRA adapters, and advanced structured prompt composition.

## Installation

Install via NuGet:

```bash
dotnet add package Aspose.LLApi
```

Or install from a local package:

```bash
dotnet add package Aspose.LLApi --source ./nupkg
```

---

## Getting Started

Instantiate the main client `LLaMACppClient`:

```csharp
using Aspose.LLaMACpp;

var client = new LLaMACppClient("https://llm-server.example.com/v1", "optional-api-key");
var chatClient = client.GetChatCompletionsClient();
```

### Quick Example: Chat Completion

```csharp
var request = new ChatCompletionRequest
{
    Model = "my-model",
    Messages = [
        new ChatCompletionMessage { Role = "user", Content = "Tell me a joke!" }
    ]
};

var response = await chatClient.CreateChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message.Content);
```

---

## ChatCompletionsClient Detailed Usage

Provides methods for chat-based interactions with streaming support, structured data responses, and flexible message composition.

### Methods

- **CreateChatCompletionAsync(ChatCompletionRequest request)**  
  Retrieves standard chat completions.

- **CreateChatCompletionAsync(string model, string prompt)**  
  Simplified method for quick interactions.

- **CreateChatCompletionAsync<T>(string model, string prompt)**  
  Automatically deserializes structured JSON responses into custom types.

- **CreateChatCompletionAsStreamAsync(ChatCompletionRequest request)**  
  Supports streaming responses for incremental output.

### Comprehensive Example: Structured Streaming Response

```csharp
var request = new ChatCompletionRequest
{
    Model = "structured-model",
    Messages = [
        new ChatCompletionMessage { Role = "user", Content = "List three programming languages." }
    ],
    Stream = true
};

await foreach (var chunk in chatClient.CreateChatCompletionAsStreamAsync(request))
{
    Console.Write(chunk);
}
```

---

## Advanced Structured Prompts: AIPrompt

### Purpose of AIPrompt

The `AIPrompt` class hierarchy (`AIText`, `AIInstructions`) is specifically designed to isolate textual data clearly from instructional content when communicating with LLMs. This isolation ensures that even less sophisticated language models distinctly recognize the difference between commands they should execute (instructions) and content they should merely reference or use as context (text).

This structured separation is critical when incorporating unpredictable user inputs into your prompts, allowing you to safely wrap arbitrary user-provided text within precise, well-defined instructions. The result is significantly improved reliability and clarity in model responses.

### How AIPrompt Ensures Clear Separation

- **Explicit Prompt Roles**:  
  `AIText` clearly labels user or context content as non-instructional, preventing unintended execution by the model. Conversely, `AIInstructions` explicitly mark segments as commands that the LLM must interpret and act upon.

- **Recursive Composition**:  
  `AIPrompt` supports nesting and placeholders (`{0}`, `{1}`) that allow structured, hierarchical prompt assembly. This recursive approach simplifies dynamic prompt generation while maintaining strict isolation between instructions and textual data.

- **Metadata and Prefixes**:  
  Each prompt subclass (e.g., `AIText`, `AIInstructions`) has metadata defining prefix symbols and system-level interpretation guidelines, further aiding even basic language models in correctly distinguishing between instruction layers and plain text.

### Derived Prompt Classes

- **AIText**:  
  Marks purely informational or user-provided text, explicitly preventing accidental interpretation as executable instructions.

- **AIInstructions**:  
  Represents explicit commands and directives intended to instruct the model's behavior.

### Practical Example: Safely Incorporating User Inputs

Here's how you clearly wrap unpredictable user input (`AIText`) within carefully structured instructions (`AIInstructions`), ensuring safe and clear execution:

```csharp
// Raw user input safely isolated as text
var userQuestion = new AIText(userProvidedInput);

// Clearly defined instruction wrapping user content
var structuredPrompt = new AIInstructions(
    "You are an expert developer answering user questions. Respond briefly and clearly to this question:
{0}", 
    userQuestion
);

// Generate the final request
var request = new ChatCompletionRequest
{
    Model = "expert-dev-model",
    Messages = [
        new ChatCompletionMessage { Role = "user", Content = structuredPrompt.ToString() }
    ]
};

var response = await chatClient.CreateChatCompletionAsync(request);
Console.WriteLine(response.Choices[0].Message.Content);
```

In this scenario, `AIText` ensures the user's potentially ambiguous input is never mistaken for instructions, while `AIInstructions` gives the LLM clear guidance on exactly how to handle that input.

---

## Additional Endpoint Clients Summary

- **CompletionClient**: Text completion.
- **CompletionsClient**: OpenAI-style completions.
- **TokenizationClient**: Text tokenization/detokenization.
- **EmbeddingClient**: Text embeddings.
- **RerankingClient**: Document reranking.
- **SlotsClient**: Backend slots management.
- **LoraAdaptersClient**: LoRA adapter management.
- **InfillClient**: Context-aware text infilling.
- **PropsClient**: Server properties configuration.
- **MetricsClient**: Server metrics access.
- **ApplyTemplateClient**: Server-side template application.

---

## Data Models

### Chat Completion Models

- `ChatCompletionRequest`: Structure for chat requests.
- `ChatCompletionResponse`: Response containing generated content.
- `ChatCompletionMessage`: Individual messages for interaction.

### Tokenization Models

- `TokenizeRequest`/`DetokenizeRequest`: Requests for token conversions.

### Embedding Models

- `NonOaiEmbeddingRequest`/`NonOaiEmbeddingResponse`: Text embedding structures.

---

## Advanced Usage Tips

- Utilize streaming for responsive applications.
- Leverage structured prompts (`AIPrompt`) for complex prompt-building.
- Dynamically adjust LoRA adapters without reloading the entire model.

---

## License

Aspose.LLApi is distributed under the [Aspose Proprietary License](./LICENSE.txt).

For NuGet packaging, include the license file named exactly as:

**LICENSE.txt**

in the root directory of your NuGet package.
