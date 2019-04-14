using Microsoft.AspNetCore.Http;

namespace Streaming.Api.Requests.Video
{
    public class UploadVideoPartRequest
    {
        public string UploadToken { get; set; }
        public string PartMD5Hash { get; set; }
        public IFormFile PartBytes { get; set; }
    }
}
