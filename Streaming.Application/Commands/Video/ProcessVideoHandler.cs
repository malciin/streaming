using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
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

            await videoRepo.UpdateAsync(processVideoCommand.Video);
        }

        private async Task ProcessVideo()
        {
            videoProcessState.Mp4VideoPath = videoFilesPath.Mp4ConvertedFilePath(command.Video.VideoId);
            await processVideoService.ConvertVideoToMp4Async(command.InputFilePath, videoProcessState.Mp4VideoPath);
            videoProcessState.VideoLength = await videoFileInfo.GetVideoLengthAsync(videoProcessState.Mp4VideoPath);

            videoProcessState.TSFilesOutputDirectory = Directory.CreateDirectory(videoFilesPath.TransportStreamDirectoryPath(command.Video.VideoId));
            videoProcessState.TSFiles = await processVideoService.SplitMp4FileIntoTsFilesAsync(videoProcessState.Mp4VideoPath,
                partNumber => Path.Combine(videoProcessState.TSFilesOutputDirectory.FullName, $"{command.Video.VideoId}.{partNumber}.ts"));
            await UploadTSFiles(videoProcessState.TSFiles);
            command.Video.SetVideoManifest(await CreateManifest(videoProcessState.TSFiles));
            
            await GenerateThumbnails();
            await UploadVideoThumbnails();
            Cleanup();
        }

        private void Cleanup()
        {
            File.Delete(command.InputFilePath);
            File.Delete(videoFilesPath.Mp4ConvertedFilePath(command.Video.VideoId));
            File.Delete(videoFilesPath.ThumbnailFilePath(command.Video.VideoId));
            videoProcessState.TSFilesOutputDirectory.Delete(recursive: true);
        }

        private async Task<VideoManifest> CreateManifest(IEnumerable<string> tsFiles)
        {
            var manifest = VideoManifest.Create(command.Video.VideoId);

            foreach(var file in tsFiles)
            {
                var length = await videoFileInfo.GetVideoLengthAsync(file);
                manifest.AddPart(length);
            }
            return manifest;
        }

        private async Task UploadTSFiles(IEnumerable<string> tsFiles)
        {
            int partNum = 0;
            foreach (var filePath in tsFiles)
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    await videoBlobService.UploadAsync(command.Video.VideoId, partNum++, fileStream);
                }
            }
        }

        private async Task GenerateThumbnails()
        {
            var mainThumbnailPath = videoFilesPath.ThumbnailFilePath(command.Video.VideoId);
            await processVideoService.TakeVideoScreenshotAsync(
                videoProcessState.Mp4VideoPath, videoFilesPath.ThumbnailFilePath(command.Video.VideoId),
                TimeSpan.FromTicks(videoProcessState.VideoLength.Ticks / 2));
            
            // await processVideoService.GenerateVideoOverviewScreenshots(videoPath,
            //    thumbnailsDirectory.FullName, new TimeSpan(videoLength.Ticks / 30));
        }

        private async Task UploadVideoThumbnails()
        {
            var mainThumbnailFilePath = videoFilesPath.ThumbnailFilePath(command.Video.VideoId);
            using (var fileStream = File.OpenRead(mainThumbnailFilePath))
            {
                await thumbnailService.UploadAsync(command.Video.VideoId, fileStream);
            }
            command.Video.SetThumbnail(thumbnailService.GetThumbnailUrl(command.Video.VideoId));
        }

        private class VideoProcessState
        {
            public TimeSpan VideoLength;
            public string Mp4VideoPath;
            public List<string> TSFiles;
            public DirectoryInfo TSFilesOutputDirectory;
        }
    }
}
