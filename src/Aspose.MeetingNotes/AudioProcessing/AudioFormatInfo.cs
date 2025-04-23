namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Contains information about audio format
    /// </summary>
    public class AudioFormatInfo
    {
        /// <summary>
        /// Gets or sets the sample rate in Hz
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the number of channels
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Gets or sets the bits per sample
        /// </summary>
        public int BitsPerSample { get; set; }

        /// <summary>
        /// Gets or sets the duration of the audio
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the original format of the audio
        /// </summary>
        public string OriginalFormat { get; set; } = string.Empty;
    }
}
