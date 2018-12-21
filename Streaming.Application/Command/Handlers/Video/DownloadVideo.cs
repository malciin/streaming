using MongoDB.Driver.GridFS;
using Streaming.Application.Command.Commands.Video;
using Streaming.Application.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Handlers.Video
{
    public class DownloadVideo : ICommandHandler<Commands.Video.DownloadVideo>
    {
        private readonly IDirectoriesSettings directoriesConfig;
        private readonly IGridFSBucket bucket;

        public DownloadVideo(IDirectoriesSettings directoriesConfig, IGridFSBucket bucket)
        {
            this.bucket = bucket;
            this.directoriesConfig = directoriesConfig;
        }

        public Task HandleAsync(Commands.Video.DownloadVideo Command)
        {
            throw new NotImplementedException();
        }
    }
}
