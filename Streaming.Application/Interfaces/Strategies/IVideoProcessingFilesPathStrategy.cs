using System;

namespace Streaming.Application.Interfaces.Strategies
{
    public interface IVideoProcessingFilesPathStrategy
    {
        string ThumbnailFilePath(Guid videoId);
        string TransportStreamDirectoryPath(Guid videoId);
        string RawUploadedVideoFilePath(Guid videoId);
        string Mp4ConvertedFilePath(Guid videoId);
    }
}
