using Microsoft.AspNetCore.Http;

namespace Streaming.Application.DTO.Video
{
    public class UploadVideoDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
