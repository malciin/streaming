using Streaming.Application.Command.Commands.Video;
using Streaming.Application.Settings;
using Streaming.Common.Extensions;
using Streaming.Domain.Models.DTO.Video;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Handlers.Video
{
    public class ProcessVideo : ICommandHandler<Commands.Video.ProcessVideo>
    {
        private readonly IVideoService videoService;
        private readonly IDirectoriesSettings directoriesConfig;

        private DirectoryInfo processingDirectory;
        private DirectoryInfo processedDirectory;
        private TimeSpan VideoLength;

        public ProcessVideo(IVideoService videoService, IDirectoriesSettings directoriesConfig)
        {
            this.videoService = videoService;
            this.directoriesConfig = directoriesConfig;
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

        async Task FFmpegProcessVideoAsync(Guid videoId)
        {
            var copyVideoCmd = $"ffmpeg -i {processingDirectory.FullName}{videoId}.mp4 -c copy {processingDirectory.FullName}{videoId}.ts";

            await copyVideoCmd.ExecuteBashAsync();

            var length = await GetVideoLengthStringAsync($"{processingDirectory.FullName}{videoId}.ts");
            VideoLength = TimeSpan.FromSeconds(double.Parse(length));

            var splitVideoIntoPartsCmd = String.Format("ffmpeg -i " +
                $"{processingDirectory.FullName}{videoId}.ts " +
                "-c copy -map 0 -segment_time 5 -f segment " +
                $"{processedDirectory.FullName}{{0}}%03d.ts", Path.DirectorySeparatorChar);

            await splitVideoIntoPartsCmd.ExecuteBashAsync();

            foreach (var command in new string[] {
                $"rm {processingDirectory.FullName}{videoId}.mp4",
                $"rm {processingDirectory.FullName}{videoId}.ts",
            })
                await command.ExecuteBashAsync();
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
            using (var file = new FileStream($"{processingDirectory.FullName}{Command.VideoId}.mp4", FileMode.CreateNew, FileAccess.Write))
            {
                Command.Video.CopyTo(file);
            }

            await FFmpegProcessVideoAsync(Command.VideoId);

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in processedDirectory.GetFiles())
                    {
                        zipArchive.CreateEntryFromFile(String.Format($"{processedDirectory.FullName}{{0}}{file.Name}", Path.DirectorySeparatorChar), file.Name);
                    }
                }
                memoryStream.Position = 0;

                var manifest = await CreateGenericManifest();

                timer.Stop();
                await videoService.UpdateVideoAfterProcessingAsync(new VideoProcessedDataDTO
                {
                    VideoId = Command.VideoId,
                    FinishedProcessingDate = DateTime.Now,
                    VideoManifestHLS = manifest,
                    Length = VideoLength,
                    ProcessingInfo = $"Sucessfully processed after {timer.Elapsed.TotalMilliseconds}ms",
                    VideoSegmentsZip = memoryStream.ToArray()
                });
            }
        }
    }
}
