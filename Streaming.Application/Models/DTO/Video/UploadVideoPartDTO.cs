using Microsoft.AspNetCore.Http;

namespace Streaming.Application.Models.DTO.Video
{
    public class UploadVideoPartDTO
    {
        public string UploadToken { get; set; }
        public string PartMD5Hash { get; set; }
        public IFormFile PartBytes { get; set; }
    }
}
