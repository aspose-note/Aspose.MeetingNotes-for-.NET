namespace Aspose.MeetingNotes.Monitoring
{
    /// <summary>
    /// Defines the interface for collecting and monitoring performance metrics.
    /// </summary>
    public interface IPerformanceMetrics
    {
        /// <summary>
        /// Starts timing a performance metric.
        /// </summary>
        /// <param name="name">The name of the performance metric.</param>
        void StartTiming(string name);

        /// <summary>
        /// Stops timing a performance metric and records the duration.
        /// </summary>
        /// <param name="name">The name of the performance metric.</param>
        void StopTiming(string name);

        /// <summary>
        /// Gets the average duration for a performance metric.
        /// </summary>
        /// <param name="name">The name of the performance metric.</param>
        /// <returns>The average duration in milliseconds, or 0 if no measurements recorded.</returns>
        double GetAverageDuration(string name);
    }
}