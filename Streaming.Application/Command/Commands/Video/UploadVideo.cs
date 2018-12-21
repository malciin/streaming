using Microsoft.AspNetCore.Http;

namespace Streaming.Application.Command.Commands.Video
{
    public class UploadVideo : ICommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
