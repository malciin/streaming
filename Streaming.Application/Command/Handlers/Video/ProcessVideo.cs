using MongoDB.Driver;
using Streaming.Application.Repository;
using Streaming.Application.Settings;
using Streaming.Common.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Handlers.Video
{
    public class ProcessVideo : ICommandHandler<Commands.Video.ProcessVideo>
    {
		private readonly IMongoCollection<Domain.Models.Core.Video> videoCollection;
		private readonly IVideoBlobRepository videoBlobRepo;
		private readonly IDirectoriesSettings directoriesConfig;

        private DirectoryInfo processingDirectory;
        private DirectoryInfo processedDirectory;
        private TimeSpan VideoLength;

        public ProcessVideo(IDirectoriesSettings directoriesConfig,
			IMongoCollection<Domain.Models.Core.Video> videoCollection,
			IVideoBlobRepository videoBlobRepo)
        {
            this.directoriesConfig = directoriesConfig;
			this.videoCollection = videoCollection;
			this.videoBlobRepo = videoBlobRepo;
        }

        void SetupProcessingEnvironment(Commands.Video.ProcessVideo command)
        {
            processingDirectory = Directory.CreateDirectory(String.Format($"{directoriesConfig.ProcessingDirectory}{{0}}", Path.DirectorySeparatorChar));
            processedDirectory = Directory.CreateDirectory(String.Format($"{directoriesConfig.ProcessedDirectory}{{0}}{command.VideoId}{{0}}", Path.DirectorySeparatorChar));
        }

        async Task<string> GetVideoLengthStringAsync(string videoPath)
        {
            var getLengthStringCommand = $"ffprobe -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 [FILEPATH]";
            return (await getLengthStringCommand.Replace("[FILEPATH]", $"{videoPath}").ExecuteBashAsync())
                    .Replace("\r\n", String.Empty).Replace("\n", String.Empty);
        }

        async Task ffmpegProcessVideoAsync(Guid videoId, string videoPath)
        {
            var copyVideoCmd = $"ffmpeg -i {videoPath} -c copy {processingDirectory.FullName}{videoId}.ts";

            await copyVideoCmd.ExecuteBashAsync();

            var length = await GetVideoLengthStringAsync($"{processingDirectory.FullName}{videoId}.ts");
            VideoLength = TimeSpan.FromSeconds(double.Parse(length));

            var splitVideoIntoPartsCmd = String.Format("ffmpeg -i " +
                $"{processingDirectory.FullName}{videoId}.ts " +
                "-c copy -map 0 -segment_time 5 -f segment " +
                $"{processedDirectory.FullName}{{0}}%03d.ts", Path.DirectorySeparatorChar);

            await splitVideoIntoPartsCmd.ExecuteBashAsync();
        }

        async Task<string> CreateGenericManifest()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("#EXTM3U");
            stringBuilder.AppendLine("#EXT-X-VERSION:3");
            stringBuilder.AppendLine("#EXT-X-TARGETDURATION:5");
            stringBuilder.AppendLine("#EXT-X-MEDIA-SEQUENCE:0");
            int part = 0;
            foreach(var file in processedDirectory.GetFiles())
            {
                var length = await GetVideoLengthStringAsync(file.FullName);
                stringBuilder.AppendLine($"#EXTINF:{length}");
                stringBuilder.AppendLine($"[ENDPOINT]/[ID]/{part++}");
            }
            stringBuilder.AppendLine("#EXT-X-ENDLIST");
            return stringBuilder.ToString();
        }

        public async Task HandleAsync(Commands.Video.ProcessVideo Command)
        {
            var timer = Stopwatch.StartNew();

            SetupProcessingEnvironment(Command);

            await ffmpegProcessVideoAsync(Command.VideoId, Command.VideoPath);
			var manifest = await CreateGenericManifest();

            int partNum = 0;
			foreach (var file in processedDirectory.GetFiles())
			{
				using (var fileStream = file.OpenRead())
				{
					await videoBlobRepo.UploadAsync(Command.VideoId, partNum++, fileStream);
				}
			}

			timer.Stop();

            var cleanCommands = new string[] {
                $"rm {processingDirectory.FullName}{Command.VideoId}.mp4",
                $"rm {processingDirectory.FullName}{Command.VideoId}.ts" }
                .Concat(processedDirectory.GetFiles().Select(x => $"rm {x}"));

            foreach (var command in cleanCommands)
                await command.ExecuteBashAsync();

            var searchFilter = Builders<Domain.Models.Core.Video>.Filter.Eq(x => x.VideoId, Command.VideoId);

            var updateDefinition = Builders<Domain.Models.Core.Video>.Update
				.Set(x => x.FinishedProcessingDate, DateTime.UtcNow)
				.Set(x => x.ProcessingInfo, $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms")
				.Set(x => x.VideoManifestHLS, manifest)
				.Set(x => x.Length, VideoLength);

			await videoCollection.UpdateOneAsync(searchFilter, updateDefinition);
        }
    }
}
