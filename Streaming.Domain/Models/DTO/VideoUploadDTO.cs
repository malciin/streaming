using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Domain.Models.DTO
{
    public class VideoUploadDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
