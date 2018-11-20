using Streaming.Application.Configuration;
using Streaming.Common.Extensions;
using Streaming.Domain.Command;
using Streaming.Domain.Models.DTO.Video;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class ProcessVideoHandler : ICommandHandler<ProcessVideo>
    {
        private readonly IVideoService videoService;
        private readonly IDirectoriesConfiguration directoriesConfig;

        private DirectoryInfo processingDirectory;
        private DirectoryInfo processedDirectory;

        public ProcessVideoHandler(IVideoService videoService, IDirectoriesConfiguration directoriesConfig)
        {
            this.videoService = videoService;
            this.directoriesConfig = directoriesConfig;
        }

        void SetupProcessingEnvironment(ProcessVideo command)
        {
            processingDirectory = Directory.CreateDirectory($"{directoriesConfig.ProcessingDirectory}/");
            processedDirectory = Directory.CreateDirectory($"{directoriesConfig.ProcessedDirectory}/{command.VideoId}/");
        }

        async Task FFmpegProcessVideoAsync(Guid videoId)
        {
            var copyVideoCmd = $"ffmpeg -i {directoriesConfig.ProcessingDirectory}/{videoId}.mp4 -c copy {directoriesConfig.ProcessingDirectory}/{videoId}.ts";
            await copyVideoCmd.ExecuteBashAsync();
            var splitVideoIntoPartsCmd = "ffmpeg -i " +
                $"{directoriesConfig.ProcessingDirectory}/{videoId}.ts " +
                "-c copy -map 0 -segment_time 5 -f segment " +
                $"{directoriesConfig.ProcessedDirectory}/{videoId}/%03d.ts";

            await splitVideoIntoPartsCmd.ExecuteBashAsync();

            foreach (var command in new string[] {
                $"rm {directoriesConfig.ProcessingDirectory}/{videoId}.mp4",
                $"rm {directoriesConfig.ProcessingDirectory}/{videoId}.ts",
            })
                await command.ExecuteBashAsync();
        }

        async Task<string> CreateGenericManifest()
        {
            var getLengthStringCommand = $"ffprobe -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 [FILEPATH]";
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("#EXTM3U");
            stringBuilder.AppendLine("#EXT-X-VERSION:3");
            stringBuilder.AppendLine("#EXT-X-TARGETDURATION:5");
            stringBuilder.AppendLine("#EXT-X-MEDIA-SEQUENCE:0");
            int part = 0;
            foreach(var file in processedDirectory.GetFiles().Where(x => x.Extension != "zip"))
            {
                var length = (await getLengthStringCommand.Replace("[FILEPATH]", $"{file.FullName}").ExecuteBashAsync())
                    .Replace("\r\n", String.Empty).Replace("\n", String.Empty);
                stringBuilder.AppendLine($"#EXTINF:{length}");
                stringBuilder.AppendLine($"[ENDPOINT]?Id=[ID]&Part={part++}");
            }
            stringBuilder.AppendLine("#EXT-X-ENDLIST");
            return stringBuilder.ToString();
        }

        public async Task Handle(ProcessVideo Command)
        {
            SetupProcessingEnvironment(Command);
            using (var file = new FileStream($"{directoriesConfig.ProcessingDirectory}/{Command.VideoId}.mp4", FileMode.CreateNew, FileAccess.Write))
            {
                Command.Video.CopyTo(file);
            }

            await FFmpegProcessVideoAsync(Command.VideoId);

            using (var str = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(str, ZipArchiveMode.Create, true))
                {
                    foreach (var file in processedDirectory.GetFiles())
                    {
                        zipArchive.CreateEntryFromFile($"{directoriesConfig.ProcessedDirectory}/{ Command.VideoId}/{file.Name}", file.Name);
                    }
                }
                str.Position = 0;
                using (var file = new FileStream($"{directoriesConfig.ProcessedDirectory}/{Command.VideoId}.zip", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await str.CopyToAsync(file);
                }
            }

            var manifest = await CreateGenericManifest();

            await videoService.UpdateVideoAfterProcessingAsync(new VideoProcessedDataDTO
            {
                VideoId = Command.VideoId,
                FinishedProcessingDate = DateTime.Now,
                VideoManifestHLS = manifest
            });
        }
    }
}
