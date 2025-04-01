using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Aspose.MeetingNotes.Processing
{
    /// <summary>
    /// Handles parallel processing of large audio files in chunks
    /// </summary>
    public class ChunkProcessor
    {
        private readonly int _maxDegreeOfParallelism;
        private readonly ILogger<ChunkProcessor> _logger;

        public ChunkProcessor(int maxDegreeOfParallelism, ILogger<ChunkProcessor> logger)
        {
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _logger = logger;
        }

        public async Task<IEnumerable<T>> ProcessChunksAsync<T>(
            IEnumerable<byte[]> chunks,
            Func<byte[], Task<T>> processor,
            CancellationToken cancellationToken = default)
        {
            var results = new ConcurrentBag<T>();
            
            await Parallel.ForEachAsync(
                chunks,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxDegreeOfParallelism,
                    CancellationToken = cancellationToken
                },
                async (chunk, ct) =>
                {
                    var result = await processor(chunk);
                    results.Add(result);
                });

            return results;
        }
    }
} 