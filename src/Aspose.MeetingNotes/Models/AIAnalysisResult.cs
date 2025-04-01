using System;
using System.Collections.Generic;

namespace Aspose.MeetingNotes.Models
{
    /// <summary>
    /// Represents the result of AI analysis of meeting content
    /// </summary>
    public class AIAnalysisResult
    {
        /// <summary>
        /// Brief summary of the meeting (max 200 words)
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// List of key points discussed during the meeting
        /// </summary>
        public List<string> KeyPoints { get; set; } = new();

        /// <summary>
        /// Main topics covered in the meeting
        /// </summary>
        public List<string> Topics { get; set; } = new();

        /// <summary>
        /// Identified decisions made during the meeting
        /// </summary>
        public List<string> Decisions { get; set; } = new();

        /// <summary>
        /// Questions and their answers from the meeting
        /// </summary>
        public List<QASegment> QASegments { get; set; } = new();

        /// <summary>
        /// Sentiment analysis of the meeting
        /// </summary>
        public SentimentAnalysis Sentiment { get; set; } = new();

        /// <summary>
        /// Participants mentioned in the meeting
        /// </summary>
        public List<ParticipantMention> Participants { get; set; } = new();

        /// <summary>
        /// Any follow-up items or next steps identified
        /// </summary>
        public List<string> FollowUps { get; set; } = new();

        /// <summary>
        /// Confidence score of the analysis (0-1)
        /// </summary>
        public double ConfidenceScore { get; set; }
    }

    /// <summary>
    /// Represents sentiment analysis of the meeting
    /// </summary>
    public class SentimentAnalysis
    {
        /// <summary>
        /// Overall sentiment score (-1 to 1)
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Primary sentiment category
        /// </summary>
        public SentimentCategory Category { get; set; }

        /// <summary>
        /// Key emotional markers identified
        /// </summary>
        public List<EmotionalMarker> EmotionalMarkers { get; set; } = new();
    }

    /// <summary>
    /// Represents a mention of a participant in the meeting
    /// </summary>
    public class ParticipantMention
    {
        /// <summary>
        /// Name of the participant
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Number of times mentioned
        /// </summary>
        public int MentionCount { get; set; }

        /// <summary>
        /// Contexts in which the participant was mentioned
        /// </summary>
        public List<string> Contexts { get; set; } = new();
    }

    /// <summary>
    /// Represents an emotional marker in the sentiment analysis
    /// </summary>
    public class EmotionalMarker
    {
        /// <summary>
        /// Type of emotion
        /// </summary>
        public string Emotion { get; set; } = string.Empty;

        /// <summary>
        /// Intensity of the emotion (0-1)
        /// </summary>
        public double Intensity { get; set; }

        /// <summary>
        /// Timestamp when this emotion was detected
        /// </summary>
        public TimeSpan Timestamp { get; set; }
    }

    /// <summary>
    /// Categories for sentiment analysis
    /// </summary>
    public enum SentimentCategory
    {
        /// <summary>
        /// Positive sentiment
        /// </summary>
        Positive,

        /// <summary>
        /// Neutral sentiment
        /// </summary>
        Neutral,

        /// <summary>
        /// Negative sentiment
        /// </summary>
        Negative,

        /// <summary>
        /// Mixed sentiment
        /// </summary>
        Mixed
    }
} 