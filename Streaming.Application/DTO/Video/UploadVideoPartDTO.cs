using Microsoft.AspNetCore.Http;
using System;

namespace Streaming.Application.DTO.Video
{
    public class UploadVideoPartDTO
    {
        public string UploadToken { get; set; }
        public string PartMD5Hash { get; set; }
        public IFormFile PartBytes { get; set; }

        internal bool IsBase64String()
        {
            throw new NotImplementedException();
        }
    }
}
