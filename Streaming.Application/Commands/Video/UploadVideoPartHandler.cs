using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Common.Exceptions;
using Streaming.Common.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Video
{
    public class UploadVideoPartHandler : ICommandHandler<UploadVideoPartCommand>
    {
        private readonly ITokenService tokenService;
        private readonly IVideoProcessingFilesPathStrategy pathStrategy;
        public UploadVideoPartHandler(IVideoProcessingFilesPathStrategy pathStrategy, ITokenService tokenService)
        {
            this.pathStrategy = pathStrategy;
            this.tokenService = tokenService;
        }

        public async Task HandleAsync(UploadVideoPartCommand command)
        {
            Guid videoId = tokenService.GetDataFromUploadVideoToken(command.UploadToken).VideoId;

            var hasher = MD5.Create();

            using (var partStream = command.PartStream)
            {
                var hash = Convert.ToBase64String(hasher.ComputeHash(partStream));
                if (!String.Equals(hash, command.PartMD5Hash))
                {
                    throw new HashesNotEqualException();
                }

                Directory.CreateDirectory(pathStrategy.RawUploadedVideoFilePath(videoId).SubstringToLastOccurence(Path.DirectorySeparatorChar));

                using (var fileStream = File.Open(pathStrategy.RawUploadedVideoFilePath(videoId), FileMode.Append))
                {
                    partStream.Position = 0;
                    await partStream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
