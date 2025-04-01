using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Aspose.MeetingNotes.Models;
using Aspose.MeetingNotes.Exceptions;

namespace Aspose.MeetingNotes.AudioProcessing
{
    /// <summary>
    /// Implementation of audio processing operations
    /// </summary>
    public class AudioProcessor : IAudioProcessor
    {
        private readonly ILogger<AudioProcessor> _logger;
        private readonly HashSet<string> _supportedFormats = new(StringComparer.OrdinalIgnoreCase) 
        { ".mp3", ".wav", ".m4a", ".ogg", ".flac" };

        public AudioProcessor(ILogger<AudioProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<ProcessedAudio> ProcessAsync(Stream audioStream, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting audio processing");
            
            // Process audio in chunks to optimize memory usage
            const int chunkSize = 1024 * 1024; // 1MB chunks
            var processedChunks = new List<byte[]>();

            var buffer = new byte[chunkSize];
            int bytesRead;

            while ((bytesRead = await audioStream.ReadAsync(buffer, 0, chunkSize, cancellationToken)) > 0)
            {
                var chunk = new byte[bytesRead];
                Array.Copy(buffer, chunk, bytesRead);
                processedChunks.Add(chunk);
            }

            return new ProcessedAudio
            {
                AudioData = processedChunks.SelectMany(x => x).ToArray(),
                Duration = TimeSpan.FromSeconds(30) // This should be calculated based on actual audio duration
            };
        }

        public bool IsFormatSupported(string fileExtension)
        {
            return _supportedFormats.Contains(fileExtension);
        }
    }
} 