using System.Security.Claims;

namespace Streaming.Application.Command.Commands.Video
{
    public class UploadVideoCommand : ICommand
    {
        public string UploadToken { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}
