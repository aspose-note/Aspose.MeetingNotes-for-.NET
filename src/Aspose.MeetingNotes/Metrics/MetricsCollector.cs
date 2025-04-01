using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Metrics
{
    /// <summary>
    /// Default implementation of IMetricsCollector that logs metrics
    /// </summary>
    public class MetricsCollector : IMetricsCollector
    {
        private readonly ILogger<MetricsCollector> _logger;
        private readonly Dictionary<string, List<double>> _metrics = new();
        private readonly Dictionary<string, List<double>> _timings = new();

        public MetricsCollector(ILogger<MetricsCollector> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Records a metric value
        /// </summary>
        public void RecordMetric(string metricName, double value)
        {
            if (!_metrics.ContainsKey(metricName))
            {
                _metrics[metricName] = new List<double>();
            }
            _metrics[metricName].Add(value);
            _logger.LogDebug("Recorded metric {MetricName}: {Value}", metricName, value);
        }

        /// <summary>
        /// Records a timing metric
        /// </summary>
        public void RecordTiming(string metricName, double durationMs)
        {
            if (!_timings.ContainsKey(metricName))
            {
                _timings[metricName] = new List<double>();
            }
            _timings[metricName].Add(durationMs);
            _logger.LogDebug("Recorded timing {MetricName}: {Duration}ms", metricName, durationMs);
        }

        /// <summary>
        /// Gets the average value for a metric
        /// </summary>
        public double GetAverageMetric(string metricName)
        {
            if (!_metrics.ContainsKey(metricName) || !_metrics[metricName].Any())
            {
                return 0;
            }
            return _metrics[metricName].Average();
        }

        /// <summary>
        /// Gets the average duration for a timing metric
        /// </summary>
        public double GetAverageTiming(string metricName)
        {
            if (!_timings.ContainsKey(metricName) || !_timings[metricName].Any())
            {
                return 0;
            }
            return _timings[metricName].Average();
        }
    }
} 