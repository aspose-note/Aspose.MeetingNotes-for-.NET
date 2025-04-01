using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Aspose.MeetingNotes;
using Aspose.MeetingNotes.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup dependency injection
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Add MeetingNotes services
        services.AddMeetingNotes(options =>
        {
            options.Language = "ru";
            options.AIModel = AIModelType.ChatGPT;
            options.AIApiKey = "your-api-key";
            options.WhisperModelSize = "base";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Get MeetingNotes client
        var client = serviceProvider.GetRequiredService<MeetingNotesClient>();

        // Process meeting audio
        using var audioStream = File.OpenRead("meeting.mp3");
        var analysis = await client.ProcessMeetingAsync(audioStream, ".mp3");

        // Export to different formats
        var markdown = await client.ExportAsync(analysis, ExportFormat.Markdown);
        File.WriteAllText("meeting-notes.md", markdown.Text);

        var pdf = await client.ExportAsync(analysis, ExportFormat.PDF);
        File.WriteAllBytes("meeting-notes.pdf", pdf.Data);

        // Export action items to task tracker
        var actionItemExtractor = serviceProvider.GetRequiredService<IActionItemExtractor>();
        await actionItemExtractor.ExportToTaskTrackerAsync(
            analysis.ActionItems,
            TaskTrackerType.Jira);
    }
} 