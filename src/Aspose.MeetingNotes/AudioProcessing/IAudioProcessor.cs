using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Interface for audio processing operations
    /// </summary>
    public interface IAudioProcessor
    {
        /// <summary>
        /// Process audio stream and prepare it for speech recognition
        /// </summary>
        Task<ProcessedAudio> ProcessAsync(Stream audioStream, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Validate if the audio format is supported
        /// </summary>
        bool IsFormatSupported(string fileExtension);
    }
} 