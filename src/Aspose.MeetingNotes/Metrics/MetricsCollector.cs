namespace Aspose.MeetingNotes.Metrics
{
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Collects and records metrics for the MeetingNotes library.
    /// </summary>
    public class MetricsCollector : IMetricsCollector
    {
        private readonly ILogger<MetricsCollector> logger;
        private readonly ConcurrentDictionary<string, List<double>> metrics = new ();
        private readonly ConcurrentDictionary<string, List<double>> timings = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsCollector"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging metrics operations.</param>
        public MetricsCollector(ILogger<MetricsCollector> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Records a metric value.
        /// </summary>
        /// <param name="name">The name of the metric.</param>
        /// <param name="value">The value to record.</param>
        public void RecordMetric(string name, double value)
        {
            var metricsList = metrics.GetOrAdd(name, _ => new List<double>());
            lock (metricsList)
            {
                metricsList.Add(value);
            }

            logger.LogDebug("Recorded metric {Name}: {Value}", name, value);
        }

        /// <summary>
        /// Records a timing value in milliseconds.
        /// </summary>
        /// <param name="name">The name of the timing metric.</param>
        /// <param name="milliseconds">The timing value in milliseconds.</param>
        public void RecordTiming(string name, double milliseconds)
        {
            var timingsList = timings.GetOrAdd(name, _ => new List<double>());
            lock (timingsList)
            {
                timingsList.Add(milliseconds);
            }

            logger.LogDebug("Recorded timing {Name}: {Milliseconds}ms", name, milliseconds);
        }

        /// <summary>
        /// Gets the average value for a metric.
        /// </summary>
        /// <param name="name">The name of the metric.</param>
        /// <returns>The average value of the metric, or 0 if no values recorded.</returns>
        public double GetAverageMetric(string name)
        {
            if (!metrics.TryGetValue(name, out var metricsList))
            {
                return 0;
            }

            lock (metricsList)
            {
                return metricsList.Count > 0 ? metricsList.Average() : 0;
            }
        }

        /// <summary>
        /// Gets the average timing value for a metric.
        /// </summary>
        /// <param name="name">The name of the timing metric.</param>
        /// <returns>The average timing value in milliseconds, or 0 if no values recorded.</returns>
        public double GetAverageTiming(string name)
        {
            if (!timings.TryGetValue(name, out var timingsList))
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
