using Microsoft.AspNetCore.Http;
using Streaming.Application.Models;

namespace Streaming.Application.Commands.Video
{
    public class UploadVideoPartCommand : IAuthenticatedCommand
    {
        public string UploadToken { get; set; }
        public string PartMD5Hash { get; set; }
        public IFormFile PartBytes { get; set; }
        public UserInfo User { get; set; }
    }
}
