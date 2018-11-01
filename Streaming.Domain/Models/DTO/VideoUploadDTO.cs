using Microsoft.AspNetCore.Http;

namespace Streaming.Domain.Models.DTO
{
    public class VideoUploadDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public IFormFile File { get; set; }
    }
}
