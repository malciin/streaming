using Streaming.Application.Settings;
using System;
using System.IO;

namespace Streaming.Application.Strategies
{
    public class PathStrategy : IPathStrategy
    {
        private readonly IFileNameStrategy fileNameStrategy;
        private readonly IDirectoriesSettings dirSettings;

        public PathStrategy(IFileNameStrategy fileNameStrategy, IDirectoriesSettings dirSettings)
        {
            this.fileNameStrategy = fileNameStrategy;
            this.dirSettings = dirSettings;
        }
        public string VideoProcessingMainDirectoryPath()
            => dirSettings.ProcessingDirectory;

        public string VideoOverviewThumbnailPath(Guid VideoId)
            => String.Format($"{VideoProcessingDirectoryPath(VideoId)}{{0}}{fileNameStrategy.GetThumbnailFileName(VideoId)}", Path.DirectorySeparatorChar);

        public string VideoProcessedDirectoryPath(Guid VideoId)
            => String.Format($"{dirSettings.LocalStorageDirectory}{{0}}{VideoId}{{0}}", Path.DirectorySeparatorChar);

        public string VideoProcessedFilePath(Guid VideoId, int PartNumber)
            => $"{VideoProcessedDirectoryPath(VideoId)}{fileNameStrategy.GetProcessedVideoFileName(VideoId, PartNumber)}";

        public string VideoProcessingDirectoryPath(Guid VideoId)
            => String.Format($"{dirSettings.ProcessingDirectory}{{0}}{VideoId}_Dir{{0}}", Path.DirectorySeparatorChar);

        public string VideoProcessingFilePath(Guid VideoId)
            => String.Format($"{VideoProcessingMainDirectoryPath()}{{0}}{fileNameStrategy.GetProcessingVideoFileName(VideoId)}", Path.DirectorySeparatorChar);

        public string VideoThumbnailsDirectoryPath(Guid VideoId)
            => String.Format($"{VideoProcessingDirectoryPath(VideoId)}Thumbnails{{0}}", Path.DirectorySeparatorChar);
    }
}
