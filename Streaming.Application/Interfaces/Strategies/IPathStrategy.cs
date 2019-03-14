using System;

namespace Streaming.Application.Interfaces.Strategies
{
    public interface IPathStrategy
    {
        string VideoProcessingMainDirectoryPath();
        string VideoProcessingDirectoryPath(Guid videoId);
        string VideoProcessedDirectoryPath(Guid VideoId);
        string VideoThumbnailsDirectoryPath(Guid VideoId);

        string VideoSplittedFilePath(Guid VideoId, int PartNumber);
        string VideoProcessingFilePath(Guid VideoId);
        string VideoConvertedToMp4FilePath(Guid VideoId);
        string VideoOverviewThumbnailPath(Guid VideoId);
    }
}
