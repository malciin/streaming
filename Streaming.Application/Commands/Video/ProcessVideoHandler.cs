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
		private readonly IVideoFilesService videoBlobService;
        private readonly IProcessVideoService processVideoService;
        private readonly IVideoFileInfoService videoFileInfo;
        private readonly IVideoProcessingFilesPathStrategy videoFilesPath;
        private readonly IThumbnailService thumbnailService;

        private VideoState videoState = 0;
        private DirectoryInfo trasportFilesDirectory;

        IEnumerable<FileInfo> splittedFiles => trasportFilesDirectory.GetFiles()
            .Where(x => Regex.IsMatch(x.Name, @"\d+\.ts"));

        public ProcessVideoHandler(
            IVideoRepository videoRepo,
			IVideoFilesService videoBlobService,
            IProcessVideoService processVideoService,
            IVideoFileInfoService videoFileInfo,
            IThumbnailService thumbnailService,
            IVideoProcessingFilesPathStrategy videoFilesPath)
        {
			this.videoRepo = videoRepo;
			this.videoBlobService = videoBlobService;
            this.processVideoService = processVideoService;
            this.thumbnailService = thumbnailService;
            this.videoFileInfo = videoFileInfo;
            this.videoFilesPath = videoFilesPath;
        }

        public async Task HandleAsync(ProcessVideoCommand command)
        {
            var timer = Stopwatch.StartNew();

            setupProcessingEnvironment(command);

            var mp4VideoPath = videoFilesPath.Mp4ConvertedFilePath(command.VideoId);
            await processVideoService.ConvertVideoToMp4(command.InputFilePath, mp4VideoPath);
            var videoLength = await videoFileInfo.GetVideoLengthAsync(mp4VideoPath);

            await processVideoService.SplitMp4FileIntoTSFiles(mp4VideoPath, trasportFilesDirectory.FullName);

            var manifest = await createManifest(command.VideoId);

            await getThumbnails(command.VideoId, mp4VideoPath, videoLength);
            await uploadVideoParts(command.VideoId);
            await uploadVideoThumbnails(command.VideoId);

            timer.Stop();

            filesCleanup(command);

            videoState |= VideoState.Processed;

            await videoRepo.UpdateAsync(new UpdateVideoAfterProcessing
            {
                FinishedProcessingDate = DateTime.UtcNow,
                ProcessingInfo = $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms",
                VideoState = videoState,
                VideoId = command.VideoId,
                VideoLength = videoLength,
                VideoManifest = manifest
            });
            await videoRepo.CommitAsync();
        }

        private void filesCleanup(ProcessVideoCommand command)
        {
            File.Delete(command.InputFilePath);
            File.Delete(videoFilesPath.Mp4ConvertedFilePath(command.VideoId));
            File.Delete(videoFilesPath.ThumbnailFilePath(command.VideoId));
            trasportFilesDirectory.Delete(recursive: true);
        }

        private void setupProcessingEnvironment(ProcessVideoCommand command)
        {
            trasportFilesDirectory = Directory.CreateDirectory(videoFilesPath.TransportStreamDirectoryPath(command.VideoId));
        }

        private async Task<VideoManifest> createManifest(Guid videoId)
        {
            var manifest = VideoManifest.Create(videoId);

            foreach(var file in splittedFiles)
            {
                var length = await videoFileInfo.GetVideoLengthAsync(file.FullName);
                manifest.AddPart(length);
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
                videoPath, videoFilesPath.ThumbnailFilePath(videoId),
                new TimeSpan(videoLength.Ticks / 2));

            // await processVideoService.GenerateVideoOverviewScreenshots(videoPath,
            //    thumbnailsDirectory.FullName, new TimeSpan(videoLength.Ticks / 30));
            videoState |= VideoState.MainThumbnailGenerated;
        }

        private async Task uploadVideoThumbnails(Guid videoId)
        {
            using (var fileStream = File.OpenRead(videoFilesPath.ThumbnailFilePath(videoId)))
            {
                await thumbnailService.UploadAsync(videoId, fileStream);
            }
        }
    }
}
