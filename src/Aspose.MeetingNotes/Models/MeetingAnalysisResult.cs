namespace Aspose.MeetingNotes.Models
{
    public class MeetingAnalysisResult
    {
        public AnalyzedContent Content { get; set; } = new();
        public List<ActionItem> ActionItems { get; set; } = new();
        public TranscriptionResult Transcription { get; set; } = new();
    }
} 