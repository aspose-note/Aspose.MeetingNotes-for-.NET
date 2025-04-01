namespace Aspose.MeetingNotes.Monitoring
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Collects and monitors performance metrics for the MeetingNotes library.
    /// </summary>
    public class PerformanceMetrics : IPerformanceMetrics
    {
        private readonly ILogger<PerformanceMetrics> logger;
        private readonly ConcurrentDictionary<string, Stopwatch> activeTimings = new ();
        private readonly ConcurrentDictionary<string, List<double>> completedTimings = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMetrics"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging performance metrics.</param>
        public PerformanceMetrics(ILogger<PerformanceMetrics> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Starts timing a performance metric.
        /// </summary>
        /// <param name="name">The name of the performance metric.</param>
        public void StartTiming(string name)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            activeTimings.TryAdd(name, stopwatch);
            logger.LogDebug("Started timing {Name}", name);
        }

        /// <summary>
        /// Stops timing a performance metric and records the duration.
        /// </summary>
        /// <param name="name">The name of the performance metric.</param>
        public void StopTiming(string name)
        {
            if (activeTimings.TryRemove(name, out var stopwatch))
            {
                stopwatch.Stop();
                var duration = stopwatch.ElapsedMilliseconds;
                var timingsList = completedTimings.GetOrAdd(name, _ => new List<double>());
                lock (timingsList)
                {
                    timingsList.Add(duration);
                }

                logger.LogDebug("Stopped timing {Name}: {Duration}ms", name, duration);
            }
        }

        /// <summary>
        /// Gets the average duration for a performance metric.
        /// </summary>
        /// <param name="name">The name of the performance metric.</param>
        /// <returns>The average duration in milliseconds, or 0 if no measurements recorded.</returns>
        public double GetAverageDuration(string name)
        {
            if (!completedTimings.TryGetValue(name, out var timingsList))
            {
                return 0;
            }

            lock (timingsList)
            {
                return timingsList.Count > 0 ? timingsList.Average() : 0;
            }
        }
    }
}
