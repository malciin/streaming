using MongoDB.Driver;
using Streaming.Application.Services;
using Streaming.Application.Settings;
using Streaming.Common.Extensions;
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
		private readonly IDirectoriesSettings directoriesConfig;
        private readonly IProcessVideoService processVideoService;

        private DirectoryInfo processingDirectory;
        IEnumerable<FileInfo> splittedFiles => processingDirectory
            .GetFiles()
            .Where(x => Regex.IsMatch(x.Name, @"\d+\.ts"));

        public ProcessVideo(IDirectoriesSettings directoriesConfig,
			IMongoCollection<Domain.Models.Core.Video> videoCollection,
			IVideoBlobService videoBlobService,
            IProcessVideoService processVideoService)
        {
            this.directoriesConfig = directoriesConfig;
			this.videoCollection = videoCollection;
			this.videoBlobService = videoBlobService;
            this.processVideoService = processVideoService;
        }

        void SetupProcessingEnvironment(Commands.Video.ProcessVideo command)
        {
            processingDirectory = Directory.CreateDirectory(String.Format($"{directoriesConfig.ProcessingDirectory}{{0}}", Path.DirectorySeparatorChar));
        }

        async Task<VideoManifest> CreateManifest(Guid VideoId)
        {
            var manifest = new VideoManifest();
            manifest.SetHeaders(TargetDurationSeconds: 5);

            foreach(var file in splittedFiles)
            {
                var length = await processVideoService.GetVideoLengthAsync(file.FullName);
                manifest.AddPart(VideoId, length);
            }
            return manifest;
        }

        public async Task HandleAsync(Commands.Video.ProcessVideo Command)
        {
            var timer = Stopwatch.StartNew();

            SetupProcessingEnvironment(Command);
            var videoLength = await processVideoService.GetVideoLengthAsync(Command.VideoPath);
            await processVideoService.ProcessVideoAsync(Command.VideoPath, 
                processingDirectory.FullName);
			var manifest = await CreateManifest(Command.VideoId);

            int partNum = 0;
			foreach (var file in splittedFiles)
			{
				using (var fileStream = file.OpenRead())
				{
					await videoBlobService.UploadAsync(Command.VideoId, partNum++, fileStream);
				}
			}

			timer.Stop();

            var cleanCommands = processingDirectory.GetFiles().Select(x => $"rm {x}");

            foreach (var command in cleanCommands)
                await command.ExecuteBashAsync();

            var searchFilter = Builders<Domain.Models.Core.Video>.Filter.Eq(x => x.VideoId, Command.VideoId);

            var updateDefinition = Builders<Domain.Models.Core.Video>.Update
				.Set(x => x.FinishedProcessingDate, DateTime.UtcNow)
				.Set(x => x.ProcessingInfo, $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms")
				.Set(x => x.VideoManifestHLS, manifest.ToString())
				.Set(x => x.Length, videoLength);

			await videoCollection.UpdateOneAsync(searchFilter, updateDefinition);
        }
    }
}
