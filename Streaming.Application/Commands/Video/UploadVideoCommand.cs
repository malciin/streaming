using Streaming.Application.Models;

namespace Streaming.Application.Commands.Video
{
    public class UploadVideoCommand : IAuthenticatedCommand
    {
        public string UploadToken { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UserInfo User { get; set; }
    }
}
