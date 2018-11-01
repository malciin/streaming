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

        public ProcessVideoHandler(IVideoService videoService)
        {
            this.videoService = videoService;
        }
        public async Task Handle(ProcessVideo Command)
        {
            Directory.CreateDirectory($"processed/{Command.VideoId}/");
            var command = $"ffmpeg -i {Command.RawVideoLocalPath} -c copy processed/{Command.VideoId}/{Command.VideoId}.ts";
            var result = await command.ExecuteBashAsync();
            $"rm toProcess/{Command.VideoId}".ExecuteBash();
        }
    }
}
