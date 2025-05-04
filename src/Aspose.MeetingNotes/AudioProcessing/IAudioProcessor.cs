using Aspose.MeetingNotes.Exceptions;

namespace Aspose.MeetingNotes.AudioProcessing;

/// <summary>
/// Defines the contract for services that process audio input,
/// primarily converting various formats to a standardized WAV format suitable for speech recognition.
/// </summary>
public interface IAudioProcessor
{
    /// <summary>
    /// Asynchronously converts the input audio file to a WAV format stream (16-bit PCM, 16kHz, Mono).
    /// </summary>
    /// <param name="audioFileInfo">Information about the audio file to process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{Stream}"/> representing the asynchronous operation.
    /// The resulting stream contains the audio data in WAV format (PCM 16-bit, 16kHz, Mono).
    /// The caller is responsible for disposing the returned stream.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="audioFileInfo"/> is null.</exception>
    /// <exception cref="System.IO.FileNotFoundException">Thrown if the file specified in <paramref name="audioFileInfo"/> does not exist.</exception>
    /// <exception cref="AudioProcessingException">Thrown if the audio conversion process fails (e.g., unsupported format by FFmpeg, FFmpeg error).</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    Task<Stream> ConvertToWavAsync(FileInfo audioFileInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the audio file format (based on its extension) is generally supported for conversion.
    /// Note: This is often a preliminary check; the actual conversion might still fail if the file is corrupted or uses an unsupported codec within the container.
    /// </summary>
    /// <param name="fileExtension">The file extension of the audio file (e.g., ".mp3", ".wav", ".m4a"). Should include the leading dot.</param>
    /// <returns><c>true</c> if the format is potentially supported by the underlying conversion tool (like FFmpeg); otherwise, <c>false</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fileExtension"/> is null.</exception>
    bool IsFormatSupported(string fileExtension);
}
