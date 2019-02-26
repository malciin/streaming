using Streaming.Application.Command.Commands.Video;
using Streaming.Application.Services;
using Streaming.Application.Settings;
using Streaming.Common.Exceptions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Handlers.Video
{
    public class UploadVideoPartHandler : ICommandHandler<UploadVideoPartCommand>
    {
        private readonly string processingDir;
        private readonly IMessageSignerService messageSigner;
        public UploadVideoPartHandler(IProcessingDirectorySettings processingDirectory, IMessageSignerService messageSigner)
        {
            processingDir = processingDirectory.ProcessingDirectory;
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
                var filePath = String.Format($"{processingDir}{{0}}{videoId}", Path.DirectorySeparatorChar);
                Directory.CreateDirectory($"{processingDir}");
                using (var fileStream = File.Open(filePath, FileMode.Append))
                {
                    partStream.Position = 0;
                    await partStream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
