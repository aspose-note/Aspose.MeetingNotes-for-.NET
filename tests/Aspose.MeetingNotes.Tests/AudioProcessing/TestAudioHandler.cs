using Aspose.MeetingNotes.AudioProcessing;
using Aspose.MeetingNotes.Models;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Tests.AudioProcessing
{
    public class TestAudioHandler : BaseAudioFormatHandler
    {
        public TestAudioHandler(ILogger logger) : base(logger)
        {
        }

        public override string SupportedExtension => ".test";

        public override Task<ProcessedAudio> ConvertToWavAsync(Stream audioStream, CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();
            audioStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return Task.FromResult(CreateProcessedAudio(
                memoryStream,
                sampleRate: 16000,
                channels: 1,
                duration: TimeSpan.FromSeconds(1)));
        }

        public override Task<AudioFormatInfo> GetFormatInfoAsync(Stream audioStream)
        {
            return Task.FromResult(new AudioFormatInfo
            {
                SampleRate = 16000,
                Channels = 1,
                BitsPerSample = 16,
                Duration = TimeSpan.FromSeconds(1),
                OriginalFormat = "TEST"
            });
        }
    }
}
