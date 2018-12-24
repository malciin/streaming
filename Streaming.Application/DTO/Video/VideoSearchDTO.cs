using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.DTO.Video
{
    public class VideoSearchDTO
    {
        public int Offset { get; set; }
        public int HowMuch { get; set; }
        public string[] Keywords { get; set; }
    }
}
