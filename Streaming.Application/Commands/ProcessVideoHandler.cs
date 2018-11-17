using Streaming.Application.Configuration;
using Streaming.Common.Extensions;
using Streaming.Domain.Command;
using Streaming.Domain.Models.DTO.Video;
using Streaming.Domain.Services;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class ProcessVideoHandler : ICommandHandler<ProcessVideo>
    {
        private readonly IVideoService videoService;
        private readonly IDirectoriesConfiguration directoriesConfig;

        public ProcessVideoHandler(IVideoService videoService, IDirectoriesConfiguration directoriesConfig)
        {
            this.videoService = videoService;
            this.directoriesConfig = directoriesConfig;
        }
        public async Task Handle(ProcessVideo Command)
        {
            Directory.CreateDirectory($"{directoriesConfig.ProcessedDirectory}/{Command.VideoId}/");

            var copyCommand = $"ffmpeg -i {Command.RawVideoLocalPath} -c copy {directoriesConfig.ProcessedDirectory}/{Command.VideoId}/{Command.VideoId}.ts";
            var result = await copyCommand.ExecuteBashAsync();

            var splitCommand = "ffmpeg -i " + 
                $"{directoriesConfig.ProcessedDirectory}/{Command.VideoId}/{Command.VideoId}.ts " + 
                "-c copy -map 0 -segment_time 5 -f segment " + 
                $"{directoriesConfig.ProcessedDirectory}/{Command.VideoId}/%03d.ts";

            await splitCommand.ExecuteBashAsync();
            $"rm {directoriesConfig.ProcessedDirectory}/{Command.VideoId}/{Command.VideoId}.ts".ExecuteBash();

            DirectoryInfo processingDirectory = new DirectoryInfo($"{directoriesConfig.ProcessedDirectory}/{Command.VideoId}");

            var videoProcessedData = new VideoProcessedDataDTO
            {
                VideoId = Command.VideoId
            };

            using (var str = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(str, ZipArchiveMode.Create, true))
                {
                    foreach (var file in processingDirectory.GetFiles())
                    {
                        zipArchive.CreateEntryFromFile($"{directoriesConfig.ProcessedDirectory}/{ Command.VideoId}/{file.Name}", file.Name);
                    }
                }
                str.Position = 0;
                using (var file = new FileStream($"{directoriesConfig.ProcessedDirectory}/{Command.VideoId}/{Command.VideoId}.zip", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await str.CopyToAsync(file);
                }
            }

            videoProcessedData.FinishedProcessingDate = DateTime.Now;
            await videoService.UpdateVideoAfterProcessingAsync(videoProcessedData);
            
            $"rm {Command.RawVideoLocalPath}".ExecuteBash();
        }
    }
}
