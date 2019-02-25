using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Streaming.Application.Command.Commands.Video
{
    public class UploadVideoCommand : ICommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}
