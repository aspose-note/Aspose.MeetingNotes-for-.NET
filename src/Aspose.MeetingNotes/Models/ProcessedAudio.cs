namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents processed audio data ready for transcription
    /// </summary>
    public class ProcessedAudio : IDisposable
    {
        private Stream? audioStream;

        /// <summary>
        /// Gets or sets the processed audio stream in a format suitable for transcription
        /// </summary>
        public Stream AudioStream
        {
            get => audioStream ?? throw new InvalidOperationException("Audio stream is not initialized");
            set => audioStream = value;
        }

        /// <summary>
        /// Gets or sets the original file extension of the audio file
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sample rate of the audio in Hz
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the number of channels in the audio
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Gets or sets the duration of the audio
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Releases the unmanaged resources used by the ProcessedAudio
        /// </summary>
        public void Dispose()
        {
            if (audioStream != null)
            {
                audioStream.Dispose();
                audioStream = null;
            }
        }
    }
}
