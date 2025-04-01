namespace Aspose.MeetingNotes.Metrics
{
    /// <summary>
    /// Interface for collecting and reporting performance metrics
    /// </summary>
    public interface IMetricsCollector
    {
        /// <summary>
        /// Records a metric value
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="value">The value to record</param>
        void RecordMetric(string name, double value);

        /// <summary>
        /// Records a timing metric
        /// </summary>
        /// <param name="name">The name of the timing metric</param>
        /// <param name="milliseconds">The duration in milliseconds</param>
        void RecordTiming(string name, double milliseconds);
    }
} 