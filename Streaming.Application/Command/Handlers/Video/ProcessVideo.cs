using MongoDB.Driver;
using Streaming.Application.Services;
using Streaming.Application.Settings;
using Streaming.Common.Extensions;
using Streaming.Common.Helpers;
using Streaming.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Handlers.Video
{
    public class ProcessVideo : ICommandHandler<Commands.Video.ProcessVideo>
    {
		private readonly IMongoCollection<Domain.Models.Core.Video> videoCollection;
		private readonly IVideoBlobService videoBlobService;
		private readonly IDirectoriesSettings directoriesSettings;
        private readonly IProcessVideoService processVideoService;
        private readonly IThumbnailService thumbnailService;

        private static readonly string thumbnailFolderName = "thumbnails";

        private VideoState videoState = 0;

        private DirectoryInfo processingDirectory;
        private DirectoryInfo thumbnailsDirectory;

        IEnumerable<FileInfo> splittedFiles => processingDirectory
            .GetFiles()
            .Where(x => Regex.IsMatch(x.Name, @"\d+\.ts"));

        public ProcessVideo(IDirectoriesSettings directoriesSettings,
			IMongoCollection<Domain.Models.Core.Video> videoCollection,
			IVideoBlobService videoBlobService,
            IProcessVideoService processVideoService,
            IThumbnailService thumbnailService)
        {
            this.directoriesSettings = directoriesSettings;
			this.videoCollection = videoCollection;
			this.videoBlobService = videoBlobService;
            this.processVideoService = processVideoService;
            this.thumbnailService = thumbnailService;
        }

        private void setupProcessingEnvironment(Commands.Video.ProcessVideo command)
        {
            processingDirectory = Directory.CreateDirectory(String.Format($"{directoriesSettings.ProcessingDirectory}{{0}}{command.VideoId}{{0}}", Path.DirectorySeparatorChar));
            thumbnailsDirectory = Directory.CreateDirectory(String.Format($"{processingDirectory.FullName}{thumbnailFolderName}{{0}}",
                Path.DirectorySeparatorChar));
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
                videoPath, $"{processingDirectory}{BlobNameHelper.GetThumbnailFilename(videoId)}", 
                new TimeSpan(videoLength.Ticks / 2));

            await processVideoService.GenerateVideoOverviewScreenshots(videoPath, 
                thumbnailsDirectory.FullName, new TimeSpan(videoLength.Ticks / 30));
            videoState |= VideoState.MainThumbnailGenerated;
        }

        private async Task uploadVideoThumbnails(Guid videoId)
        {
            using (var fileStream = File.OpenRead($"{processingDirectory}{BlobNameHelper.GetThumbnailFilename(videoId)}"))
            {
                await thumbnailService.UploadAsync(videoId, fileStream);
            }
        }

        public async Task HandleAsync(Commands.Video.ProcessVideo Command)
        {
            var timer = Stopwatch.StartNew();

            setupProcessingEnvironment(Command);
            var videoLength = await processVideoService.GetVideoLengthAsync(Command.VideoPath);
            await processVideoService.ProcessVideoAsync(Command.VideoPath,
                processingDirectory.FullName);
            var manifest = await createManifest(Command.VideoId);
            await getThumbnails(Command.VideoId, Command.VideoPath, videoLength);
            await uploadVideoParts(Command.VideoId);
            await uploadVideoThumbnails(Command.VideoId);

            timer.Stop();

            processingDirectory.Delete(recursive: true);
            videoState |= VideoState.Processed;

            var searchFilter = Builders<Domain.Models.Core.Video>.Filter.Eq(x => x.VideoId, Command.VideoId);
            var updateDefinition = Builders<Domain.Models.Core.Video>.Update
                .Set(x => x.FinishedProcessingDate, DateTime.UtcNow)
                .Set(x => x.ProcessingInfo, $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms")
                .Set(x => x.VideoManifestHLS, manifest.ToString())
                .Set(x => x.Length, videoLength)
                .Set(x => x.State, videoState);

            await videoCollection.UpdateOneAsync(searchFilter, updateDefinition);
        }        
    }
}
