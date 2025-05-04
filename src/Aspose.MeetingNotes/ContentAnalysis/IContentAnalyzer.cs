using Aspose.MeetingNotes.Exceptions;
using Aspose.MeetingNotes.Models;

namespace Aspose.MeetingNotes.ContentAnalysis;

/// <summary>
/// Defines the contract for services that analyze transcribed meeting content
/// to extract structured information like summaries, key points, topics, etc.
/// </summary>
public interface IContentAnalyzer
{
    /// <summary>
    /// Analyzes the provided transcription result using an AI model to structure the content.
    /// </summary>
    /// <param name="transcription">The result object obtained from speech recognition, containing text segments.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains the <see cref="AnalyzedContent"/> with structured information
    /// derived from the transcription.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="transcription"/> is null.</exception>
    /// <exception cref="AIModelException">Thrown if the underlying AI model fails during analysis.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    Task<AnalyzedContent> AnalyzeAsync(TranscriptionResult transcription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a concise summary from already analyzed content.
    /// Note: In default implementations, the summary is typically generated during the initial
    /// <see cref="AnalyzeAsync"/> call by the AI model and this method might just return that pre-generated summary.
    /// </summary>
    /// <param name="content">The analyzed content object containing the summary and other details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests (may not be used if just returning existing summary).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains the summary string.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="content"/> is null.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled (relevant if implementation involves async work).</exception>
    Task<string> GenerateSummaryAsync(AnalyzedContent content, CancellationToken cancellationToken = default);
}
