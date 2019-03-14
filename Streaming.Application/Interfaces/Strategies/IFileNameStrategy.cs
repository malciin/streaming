using System;

namespace Streaming.Application.Interfaces.Strategies
{
    public interface IFileNameStrategy
    {
        string GetSplittedVideoFileName(Guid VideoId, int PartNumber);
        string GetProcessedVideoFileName(Guid VideoId);
        string GetProcessingVideoFileName(Guid VideoId);
        string GetThumbnailFileName(Guid VideoId);
    }
}
