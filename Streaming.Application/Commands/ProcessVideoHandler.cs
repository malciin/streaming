using Streaming.Application.Configuration;
using Streaming.Common.Extensions;
using Streaming.Domain.Command;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class ProcessVideoHandler : ICommandHandler<ProcessVideo>
    {
        private readonly IVideoService videoService;
        private readonly IDirectoriesConfig directoriesConfig;

        public ProcessVideoHandler(IVideoService videoService, IDirectoriesConfig directoriesConfig)
        {
            this.videoService = videoService;
            this.directoriesConfig = directoriesConfig;
        }
        public async Task Handle(ProcessVideo Command)
        {
            Directory.CreateDirectory($"{directoriesConfig.ProcessedDirectory}/{Command.VideoId}/");
            var command = $"ffmpeg -i {Command.RawVideoLocalPath} -c copy {directoriesConfig.ProcessedDirectory}/{Command.VideoId}/{Command.VideoId}.ts";
            var result = await command.ExecuteBashAsync();
            $"rm {Command.RawVideoLocalPath}".ExecuteBash();
        }
    }
}
