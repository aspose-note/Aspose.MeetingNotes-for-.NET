using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Monitoring
{
    /// <summary>
    /// Tracks performance metrics for operations
    /// </summary>
    public class PerformanceMetrics
    {
        private readonly ILogger<PerformanceMetrics> _logger;
        private readonly Dictionary<string, Stopwatch> _activeOperations;

        public PerformanceMetrics(ILogger<PerformanceMetrics> logger)
        {
            _logger = logger;
            _activeOperations = new Dictionary<string, Stopwatch>();
        }

        /// <summary>
        /// Start tracking an operation
        /// </summary>
        public IDisposable TrackOperation(string operationName)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _activeOperations[operationName] = stopwatch;

            return new OperationTracker(this, operationName, stopwatch);
        }

        private void EndOperation(string operationName, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            _activeOperations.Remove(operationName);
            _logger.LogInformation("Operation {OperationName} completed in {ElapsedMilliseconds}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
        }

        private class OperationTracker : IDisposable
        {
            private readonly PerformanceMetrics _metrics;
            private readonly string _operationName;
            private readonly Stopwatch _stopwatch;

            public OperationTracker(PerformanceMetrics metrics, string operationName, Stopwatch stopwatch)
            {
                _metrics = metrics;
                _operationName = operationName;
                _stopwatch = stopwatch;
            }

            public void Dispose()
            {
                _metrics.EndOperation(_operationName, _stopwatch);
            }
        }
    }
} 