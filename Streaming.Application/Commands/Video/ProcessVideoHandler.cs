using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Application.Models.Repository.Video;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Video
{
    public class ProcessVideoHandler : ICommandHandler<ProcessVideoCommand>
    {
		private readonly IVideoRepository videoRepo;
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
            IVideoRepository videoRepo,
			IVideoBlobService videoBlobService,
            IProcessVideoService processVideoService,
            IThumbnailService thumbnailService,
            IPathStrategy pathStrategy,
            IFileNameStrategy fileNameStrategy)
        {
			this.videoRepo = videoRepo;
			this.videoBlobService = videoBlobService;
            this.processVideoService = processVideoService;
            this.thumbnailService = thumbnailService;
            this.pathStrategy = pathStrategy;
            this.fileNameStrategy = fileNameStrategy;
        }

        private void setupProcessingEnvironment(ProcessVideoCommand command)
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

        public async Task HandleAsync(ProcessVideoCommand Command)
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

            await videoRepo.UpdateAsync(new UpdateVideoAfterProcessing
            {
                FinishedProcessingDate = DateTime.UtcNow,
                ProcessingInfo = $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms",
                VideoState = videoState,
                VideoId = Command.VideoId,
                VideoLength = videoLength,
                VideoManifestHLS = manifest.ToString()
            });
            await videoRepo.CommitAsync();
        }        
    }
}
