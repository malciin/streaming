﻿namespace Streaming.Application.Models.DTO.Video
{
    public class VideoSearchDTO
    {
        public int Offset { get; set; }
        public int HowMuch { get; set; }
        public string[] Keywords { get; set; }
    }
}
