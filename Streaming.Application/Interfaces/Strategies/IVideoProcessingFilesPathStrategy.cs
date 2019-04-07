using System;

namespace Streaming.Application.Interfaces.Strategies
{
    public interface IVideoProcessingFilesPathStrategy
    {
        string ThumbnailFilePath(Guid videoId);
        string TransportStreamFilePath(Guid videoId, int partNumber);
        string RawUploadedVideoFilePath(Guid videoId);
        string Mp4ConvertedFilePath(Guid videoId);
    }
}
