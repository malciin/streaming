using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Common.Exceptions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Video
{
    public class UploadVideoPartHandler : ICommandHandler<UploadVideoPartCommand>
    {
        private readonly IPathStrategy pathStrategy;
        private readonly IMessageSignerService messageSigner;
        public UploadVideoPartHandler(IPathStrategy pathStrategy, IMessageSignerService messageSigner)
        {
            this.pathStrategy = pathStrategy;
            this.messageSigner = messageSigner;
        }

        public Guid getVideoIdFromUploadToken(string uploadToken)
        {
            var signedMessage = Convert.FromBase64String(uploadToken);
            var message = messageSigner.GetMessage(signedMessage);
            return new Guid(message);
        }

        public async Task HandleAsync(UploadVideoPartCommand Command)
        {
            var videoId = getVideoIdFromUploadToken(Command.UploadToken);

            var hasher = MD5.Create();

            using (var partStream = Command.PartBytes.OpenReadStream())
            {
                var hash = Convert.ToBase64String(hasher.ComputeHash(partStream));
                if (!String.Equals(hash, Command.PartMD5Hash))
                {
                    throw new HashesNotEqualException();
                }
                Directory.CreateDirectory(pathStrategy.VideoProcessingMainDirectoryPath());
                using (var fileStream = File.Open(pathStrategy.VideoProcessingFilePath(videoId), FileMode.Append))
                {
                    partStream.Position = 0;
                    await partStream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
