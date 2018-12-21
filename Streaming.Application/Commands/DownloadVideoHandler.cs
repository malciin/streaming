using MongoDB.Driver.GridFS;
using Streaming.Application.Settings;
using Streaming.Domain.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class DownloadVideoHandler : ICommandHandler<DownloadVideo>
    {
        private readonly IDirectoriesSettings directoriesConfig;
        private readonly IGridFSBucket bucket;

        public DownloadVideoHandler(IDirectoriesSettings directoriesConfig, IGridFSBucket bucket)
        {
            this.bucket = bucket;
            this.directoriesConfig = directoriesConfig;
        }

        public Task Handle(DownloadVideo Command)
        {
            throw new NotImplementedException();
        }
    }
}
