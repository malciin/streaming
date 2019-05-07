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
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Video
{
    public class ProcessVideoHandler : ICommandHandler<ProcessVideoCommand>
    {
		private readonly IVideoRepository videoRepo;
		private readonly IVideoPartsFileService videoBlobService;
        private readonly IProcessVideoService processVideoService;
        private readonly IVideoFileInfoService videoFileInfo;
        private readonly IVideoProcessingFilesPathStrategy videoFilesPath;
        private readonly IThumbnailService thumbnailService;

        private ProcessVideoCommand command;
        private readonly VideoProcessState videoProcessState;

        public ProcessVideoHandler(
            IVideoRepository videoRepo,
			IVideoPartsFileService videoBlobService,
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
            videoProcessState = new VideoProcessState();
        }

        public async Task HandleAsync(ProcessVideoCommand processVideoCommand)
        {
            this.command = processVideoCommand;
            
            var timer = Stopwatch.StartNew();
                await ProcessVideo();
            timer.Stop();

            await videoRepo.UpdateAsync(new UpdateVideoAfterProcessing
            {
                FinishedProcessingDate = DateTime.UtcNow,
                ProcessingInfo = $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms",
                VideoState = videoProcessState.VideoState,
                VideoId = command.VideoId,
                VideoLength = videoProcessState.VideoLength,
                VideoManifest = videoProcessState.Manifest,
                MainThumbnailUrl = thumbnailService.GetThumbnailUrl(command.VideoId)
            });
        }

        private async Task ProcessVideo()
        {
            videoProcessState.Mp4VideoPath = videoFilesPath.Mp4ConvertedFilePath(command.VideoId);
            await processVideoService.ConvertVideoToMp4Async(command.InputFilePath, videoProcessState.Mp4VideoPath);
            videoProcessState.VideoLength = await videoFileInfo.GetVideoLengthAsync(videoProcessState.Mp4VideoPath);

            videoProcessState.TSFilesOutputDirectory = Directory.CreateDirectory(videoFilesPath.TransportStreamDirectoryPath(command.VideoId));
            videoProcessState.TSFiles = await processVideoService.SplitMp4FileIntoTsFilesAsync(videoProcessState.Mp4VideoPath, videoProcessState.TSFilesOutputDirectory.FullName);
            await UploadTSFiles(videoProcessState.TSFiles);
            videoProcessState.Manifest = await CreateManifest(videoProcessState.TSFiles);

            await GenerateThumbnails();
            await UploadVideoThumbnails();
            Cleanup();
            
            videoProcessState.VideoState |= VideoState.Processed;
        }

        private void Cleanup()
        {
            File.Delete(command.InputFilePath);
            File.Delete(videoFilesPath.Mp4ConvertedFilePath(command.VideoId));
            File.Delete(videoFilesPath.ThumbnailFilePath(command.VideoId));
            videoProcessState.TSFilesOutputDirectory.Delete(recursive: true);
        }

        private async Task<VideoManifest> CreateManifest(IEnumerable<string> tsFiles)
        {
            var manifest = VideoManifest.Create(command.VideoId);

            foreach(var file in tsFiles)
            {
                var length = await videoFileInfo.GetVideoLengthAsync(file);
                manifest.AddPart(length);
            }
            videoProcessState.VideoState |= VideoState.ManifestGenerated;
            return manifest;
        }

        private async Task UploadTSFiles(IEnumerable<string> tsFiles)
        {
            int partNum = 0;
            foreach (var filePath in tsFiles)
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    await videoBlobService.UploadAsync(command.VideoId, partNum++, fileStream);
                }
            }
        }

        private async Task GenerateThumbnails()
        {
            await processVideoService.TakeVideoScreenshotAsync(
                command.InputFilePath, videoFilesPath.ThumbnailFilePath(command.VideoId),
                new TimeSpan(videoProcessState.VideoLength.Ticks / 2));

            // await processVideoService.GenerateVideoOverviewScreenshots(videoPath,
            //    thumbnailsDirectory.FullName, new TimeSpan(videoLength.Ticks / 30));
            videoProcessState.VideoState |= VideoState.MainThumbnailGenerated;
        }

        private async Task UploadVideoThumbnails()
        {
            using (var fileStream = File.OpenRead(videoFilesPath.ThumbnailFilePath(command.VideoId)))
            {
                await thumbnailService.UploadAsync(command.VideoId, fileStream);
            }
        }

        private class VideoProcessState
        {
            public VideoState VideoState;
            public VideoManifest Manifest;
            public TimeSpan VideoLength;
            public string Mp4VideoPath;
            public List<string> TSFiles;
            public DirectoryInfo TSFilesOutputDirectory;

            public VideoProcessState()
            {
                VideoState = 0;
            }
        }
    }
}
