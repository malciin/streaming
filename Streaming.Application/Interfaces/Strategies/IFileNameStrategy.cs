using System;

namespace Streaming.Application.Interfaces.Strategies
{
    public interface IFileNameStrategy
    {
        string GetProcessedVideoFileName(Guid VideoId, int PartNumber);
        string GetProcessingVideoFileName(Guid VideoId);
        string GetThumbnailFileName(Guid VideoId);
    }
}
