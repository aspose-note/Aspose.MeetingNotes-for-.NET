using System.IO;

namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents processed audio data ready for transcription
    /// </summary>
    public class ProcessedAudio : IDisposable
    {
        private Stream? _audioStream;

        /// <summary>
        /// The processed audio stream in a format suitable for transcription
        /// </summary>
        public Stream AudioStream
        {
            get => _audioStream ?? throw new InvalidOperationException("Audio stream is not initialized");
            set => _audioStream = value;
        }

        /// <summary>
        /// The original file extension of the audio file
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// The sample rate of the audio in Hz
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// The number of channels in the audio
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// The duration of the audio
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Releases the unmanaged resources used by the ProcessedAudio
        /// </summary>
        public void Dispose()
        {
            if (_audioStream != null)
            {
                _audioStream.Dispose();
                _audioStream = null;
            }
        }
    }
} 