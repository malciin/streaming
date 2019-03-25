using Streaming.Application.Events;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Application.Models.DTO.Video;
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
        private readonly IVideoFileInfoService videoFileInfo;
        private readonly IThumbnailService thumbnailService;
        private readonly IPathStrategy pathStrategy;
        private readonly IFileNameStrategy fileNameStrategy;
        private readonly IEventEmmiter emitter;

        private VideoState videoState = 0;
        private ProcessVideoCommand command;

        private DirectoryInfo processingDirectory;
        private DirectoryInfo thumbnailsDirectory;

        IEnumerable<FileInfo> splittedFiles => processingDirectory
            .GetFiles()
            .Where(x => Regex.IsMatch(x.Name, @"\d+\.ts"));

        public ProcessVideoHandler(
            IVideoRepository videoRepo,
			IVideoBlobService videoBlobService,
            IProcessVideoService processVideoService,
            IVideoFileInfoService videoFileInfo,
            IThumbnailService thumbnailService,
            IPathStrategy pathStrategy,
            IFileNameStrategy fileNameStrategy,
            IEventEmmiter emitter)
        {
			this.videoRepo = videoRepo;
			this.videoBlobService = videoBlobService;
            this.processVideoService = processVideoService;
            this.thumbnailService = thumbnailService;
            this.pathStrategy = pathStrategy;
            this.fileNameStrategy = fileNameStrategy;
            this.videoFileInfo = videoFileInfo;
            this.emitter = emitter;
        }


        public async Task HandleAsync(ProcessVideoCommand command)
        {
            this.command = command;
            var timer = Stopwatch.StartNew();

            setupProcessingEnvironment();

            var mp4VideoPath = pathStrategy.VideoConvertedToMp4FilePath(command.VideoId);
            emitStatus("Environment for processing setted up!");
            await processVideoService.ConvertVideoToMp4(command.InputFilePath, mp4VideoPath, (output) => emitStatus(output));
            emitStatus("File successfully converted to mp4!");

            var videoLength = await videoFileInfo.GetVideoLengthAsync(mp4VideoPath);
            emitStatus($"Video length of conveted file: {videoLength.TotalMilliseconds}ms");

            await processVideoService.SplitMp4FileIntoTSFiles(mp4VideoPath, processingDirectory.FullName);
            emitStatus($"Video successfully splitted to .ts files");

            var manifest = await createManifest(command.VideoId);
            emitStatus($"Manifest created");

            await getThumbnails(command.VideoId, mp4VideoPath, videoLength);
            await uploadVideoParts(command.VideoId);
            await uploadVideoThumbnails(command.VideoId);

            timer.Stop();

            emitStatus($"Total processing time: {timer.ElapsedMilliseconds}ms");

            processingDirectory.Delete(recursive: true);
            videoState |= VideoState.Processed;

            await videoRepo.UpdateAsync(new UpdateVideoAfterProcessing
            {
                FinishedProcessingDate = DateTime.UtcNow,
                ProcessingInfo = $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms",
                VideoState = videoState,
                VideoId = command.VideoId,
                VideoLength = videoLength,
                VideoManifestHLS = manifest.ToString()
            });
            await videoRepo.CommitAsync();
        }

        private void emitStatus(string str)
        {
            emitter.Emit(new VideoProcessingStatusEvent
            {
                VideoId = command.VideoId,
                UserId = command.UserId,
                Output = str
            });
        }

        private void setupProcessingEnvironment()
        {
            processingDirectory = Directory.CreateDirectory(pathStrategy.VideoProcessingDirectoryPath(command.VideoId));
            thumbnailsDirectory = Directory.CreateDirectory(pathStrategy.VideoThumbnailsDirectoryPath(command.VideoId));
            Directory.CreateDirectory(pathStrategy.VideoProcessedDirectoryPath(command.VideoId));
            emitStatus("Environment for processing setted up!");
        }

        private async Task<VideoManifest> createManifest(Guid videoId)
        {
            var manifest = new VideoManifest();
            manifest.SetHeaders(TargetDurationSeconds: 5);

            foreach(var file in splittedFiles)
            {
                var length = await videoFileInfo.GetVideoLengthAsync(file.FullName);
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
    }
}
