using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Video;
using Streaming.Domain.Models;

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
