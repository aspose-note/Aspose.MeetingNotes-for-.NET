using Aspose.MeetingNotes.AudioProcessing;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Tests.Mocks;

/// <summary>
/// Mock implementation of the refactored IAudioProcessor for testing purposes.
/// Returns a simple MemoryStream.
/// </summary>
public class MockAudioProcessor : IAudioProcessor
{
    private readonly ILogger<MockAudioProcessor> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockAudioProcessor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public MockAudioProcessor(ILogger<MockAudioProcessor> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        this.logger = logger;
    }

    /// <summary>
    /// Simulates audio conversion, returning a simple MemoryStream.
    /// Ignores the actual content of audioFileInfo for simplicity in mocking.
    /// </summary>
    /// <param name="audioFileInfo">Information about the input audio file (ignored in mock).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task containing a MemoryStream with dummy data.</returns>
    public Task<Stream> ConvertToWavAsync(FileInfo audioFileInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioFileInfo);
        this.logger.LogInformation("MockAudioProcessor: Simulating conversion for {FileName}", audioFileInfo.Name);

        byte[] dummyWavData = System.Text.Encoding.UTF8.GetBytes("Mock WAV data");
        var memoryStream = new MemoryStream(dummyWavData);
        memoryStream.Position = 0;

        return Task.FromResult<Stream>(memoryStream);
    }

    /// <summary>
    /// Mock implementation that supports any extension for simplicity.
    /// </summary>
    /// <param name="fileExtension">The file extension (ignored in mock).</param>
    /// <returns>Always true.</returns>
    public bool IsFormatSupported(string fileExtension)
    {
        ArgumentNullException.ThrowIfNull(fileExtension);
        this.logger.LogDebug("MockAudioProcessor: Assuming format {Extension} is supported", fileExtension);
        return true;
    }
}
