using Microsoft.AspNetCore.Http;

namespace Streaming.Application.Command.Commands.Video
{
    public class UploadVideoPartCommand : ICommand
    {
        public string UploadToken { get; set; }
        public string PartMD5Hash { get; set; }
        public IFormFile PartBytes { get; set; }
    }
}
