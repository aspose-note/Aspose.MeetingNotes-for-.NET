using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IAudioProcessor for testing purposes
    /// </summary>
    public class MockAudioProcessor : IAudioProcessor
    {
        private readonly ILogger<MockAudioProcessor> logger;

        public MockAudioProcessor(ILogger<MockAudioProcessor> logger)
        {
            this.logger = logger;
        }

        public async Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, string fileExtension, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Processing audio with mock audio processor");

            // Simulate some processing time
            await Task.Delay(100, cancellationToken);

            // Create a valid WAV header
            var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            
            // Write WAV header
            writer.Write("RIFF".ToCharArray());
            writer.Write(36); // Chunk size
            writer.Write("WAVE".ToCharArray());
            writer.Write("fmt ".ToCharArray());
            writer.Write(16); // Subchunk size
            writer.Write((short)1); // Audio format (PCM)
            writer.Write((short)1); // Number of channels
            writer.Write(16000); // Sample rate
            writer.Write(32000); // Byte rate
            writer.Write((short)2); // Block align
            writer.Write((short)16); // Bits per sample
            writer.Write("data".ToCharArray());
            writer.Write(0); // Data size

            memoryStream.Position = 0;

            return new ProcessedAudio
            {
                AudioStream = memoryStream,
                SampleRate = 16000,
                Channels = 1,
                Duration = TimeSpan.FromSeconds(1)
            };
        }

        public bool IsFormatSupported(string fileExtension)
        {
            return fileExtension == ".wav";
        }

        public async Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream, string fileExtension)
        {
            logger.LogInformation("Getting format information with mock audio processor");

            // Simulate some processing time
            await Task.Delay(100, default);

            return new AudioFormatInfo
            {
                SampleRate = 16000,
                Channels = 1,
                BitsPerSample = 16,
                Duration = TimeSpan.FromSeconds(1),
                OriginalFormat = "WAV"
            };
        }
    }
}
