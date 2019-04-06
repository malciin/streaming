using System;
using System.IO;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Interfaces.Strategies;

namespace Streaming.Application.Strategies
{
    public class PathStrategies : IVideoFilesPathStrategy, IVideoProcessingFilesPathStrategy
    {
        private readonly string localStorageDirectoryPath;
        public PathStrategies(ILocalStorageDirectorySettings localStorageDirectory)
        {
            this.localStorageDirectoryPath = localStorageDirectory.LocalStorageDirectory;
        }

        string IVideoFilesPathStrategy.TransportStreamFilePath(Guid videoId, int partNumber)
            => String.Format($"{localStorageDirectoryPath}{{0}}ts_files{{0}}{partNumber}_{videoId}.ts", Path.DirectorySeparatorChar);


        string IVideoProcessingFilesPathStrategy.Mp4ConvertedFilePath(Guid videoId)
            => String.Format($"{localStorageDirectoryPath}{{0}}processing{{0}}{videoId}_mp4.mp4", Path.DirectorySeparatorChar);

        string IVideoProcessingFilesPathStrategy.RawUploadedVideoFilePath(Guid videoId)
            => String.Format($"{localStorageDirectoryPath}{{0}}processing{{0}}{videoId}", Path.DirectorySeparatorChar);

        string IVideoProcessingFilesPathStrategy.TransportStreamFilePath(Guid videoId, int partNumber)
            => String.Format($"{localStorageDirectoryPath}{{0}}processing{{0}}ts_files{{0}}{partNumber}_{videoId}.ts", Path.DirectorySeparatorChar);

        string IVideoProcessingFilesPathStrategy.ThumbnailFilePath(Guid videoId)
            => String.Format($"{localStorageDirectoryPath}{{0}}processing{{0}}{videoId}.jpg", Path.DirectorySeparatorChar);
    }
}
