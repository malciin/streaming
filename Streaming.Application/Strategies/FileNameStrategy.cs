using Streaming.Application.Interfaces.Strategies;
using System;

namespace Streaming.Application.Strategies
{
    class FileNameStrategy : IFileNameStrategy
    {
        public string GetSplittedVideoFileName(Guid VideoId, int PartNumber)
            => $"{VideoId}_{PartNumber}.ts";

        public string GetProcessingVideoFileName(Guid VideoId)
            => VideoId.ToString();

        public string GetThumbnailFileName(Guid VideoId)
            => $"{VideoId}_Thumbnail.jpg";

        public string GetProcessedVideoFileName(Guid VideoId)
            => $"{VideoId}_Processed.mp4";
    }
}
