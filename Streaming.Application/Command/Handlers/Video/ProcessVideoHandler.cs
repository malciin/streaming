using MongoDB.Driver;
using Streaming.Application.Services;
using Streaming.Application.Settings;
using Streaming.Application.Strategies;
using Streaming.Common.Helpers;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Handlers.Video
{
    public class ProcessVideoHandler : ICommandHandler<Commands.Video.ProcessVideoCommand>
    {
		private readonly IMongoCollection<Domain.Models.Video> videoCollection;
		private readonly IVideoBlobService videoBlobService;
        private readonly IProcessVideoService processVideoService;
        private readonly IThumbnailService thumbnailService;
        private readonly IPathStrategy pathStrategy;
        private readonly IFileNameStrategy fileNameStrategy;

        private VideoState videoState = 0;

        private DirectoryInfo processingDirectory;
        private DirectoryInfo thumbnailsDirectory;

        IEnumerable<FileInfo> splittedFiles => processingDirectory
            .GetFiles()
            .Where(x => Regex.IsMatch(x.Name, @"\d+\.ts"));

        public ProcessVideoHandler(
            IMongoCollection<Domain.Models.Video> videoCollection,
			IVideoBlobService videoBlobService,
            IProcessVideoService processVideoService,
            IThumbnailService thumbnailService,
            IPathStrategy pathStrategy,
            IFileNameStrategy fileNameStrategy)
        {
			this.videoCollection = videoCollection;
			this.videoBlobService = videoBlobService;
            this.processVideoService = processVideoService;
            this.thumbnailService = thumbnailService;
            this.pathStrategy = pathStrategy;
            this.fileNameStrategy = fileNameStrategy;
        }

        private void setupProcessingEnvironment(Commands.Video.ProcessVideoCommand command)
        {
            processingDirectory = Directory.CreateDirectory(pathStrategy.VideoProcessingDirectoryPath(command.VideoId));
            thumbnailsDirectory = Directory.CreateDirectory(pathStrategy.VideoThumbnailsDirectoryPath(command.VideoId));
        }

        private async Task<VideoManifest> createManifest(Guid videoId)
        {
            var manifest = new VideoManifest();
            manifest.SetHeaders(TargetDurationSeconds: 5);

            foreach(var file in splittedFiles)
            {
                var length = await processVideoService.GetVideoLengthAsync(file.FullName);
                manifest.AddPart(videoId, length);
            }
            videoState |= VideoState.ManifestGenerated;
            return manifest;
        }

        private async Task uploadVideoParts(Guid videoId)
        {
            int partNum = 0;
            foreach (var file in splittedFiles)
            {
                using (var fileStream = file.OpenRead())
                {
                    await videoBlobService.UploadAsync(videoId, partNum++, fileStream);
                }
            }
        }

        private async Task getThumbnails(Guid videoId, string videoPath, TimeSpan videoLength)
        {
            await processVideoService.TakeVideoScreenshot(
                videoPath, pathStrategy.VideoOverviewThumbnailPath(videoId), 
                new TimeSpan(videoLength.Ticks / 2));

            await processVideoService.GenerateVideoOverviewScreenshots(videoPath, 
                thumbnailsDirectory.FullName, new TimeSpan(videoLength.Ticks / 30));
            videoState |= VideoState.MainThumbnailGenerated;
        }

        private async Task uploadVideoThumbnails(Guid videoId)
        {
            using (var fileStream = File.OpenRead(pathStrategy.VideoOverviewThumbnailPath(videoId)))
            {
                await thumbnailService.UploadAsync(videoId, fileStream);
            }
        }

        public async Task HandleAsync(Commands.Video.ProcessVideoCommand Command)
        {
            var timer = Stopwatch.StartNew();

            setupProcessingEnvironment(Command);

            var videoPath = pathStrategy.VideoProcessingFilePath(Command.VideoId);
            var videoLength = await processVideoService.GetVideoLengthAsync(videoPath);
            await processVideoService.ProcessVideoAsync(videoPath, processingDirectory.FullName);
            var manifest = await createManifest(Command.VideoId);
            await getThumbnails(Command.VideoId, videoPath, videoLength);
            await uploadVideoParts(Command.VideoId);
            await uploadVideoThumbnails(Command.VideoId);

            timer.Stop();

            processingDirectory.Delete(recursive: true);
            videoState |= VideoState.Processed;

            var searchFilter = Builders<Domain.Models.Video>.Filter.Eq(x => x.VideoId, Command.VideoId);
            var updateDefinition = Builders<Domain.Models.Video>.Update
                .Set(x => x.FinishedProcessingDate, DateTime.UtcNow)
                .Set(x => x.ProcessingInfo, $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms")
                .Set(x => x.VideoManifestHLS, manifest.ToString())
                .Set(x => x.Length, videoLength)
                .Set(x => x.State, videoState);

            await videoCollection.UpdateOneAsync(searchFilter, updateDefinition);
        }        
    }
}
