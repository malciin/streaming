using Streaming.Application.Models.Enum;
using System;

namespace Streaming.Application.Models.DTO.Video
{
    public class VideoFileDetailsDTO
    {
        public class AudioDetailsDTO
        {
            public AudioCodec Codec { get; set; }
        }

        public class VideoDetailsDTO
        {
            public VideoCodec Codec { get; set; }
            public (int xResolution, int yResolution) Resolution { get; set; }
            public int BitrateKbs { get; set; }
        }

        public VideoDetailsDTO Video { get; set; }
        public AudioDetailsDTO Audio { get; set; }

        public TimeSpan Duration { get; set; }

        public VideoFileDetailsDTO()
        {
            Video = new VideoDetailsDTO();
            Audio = new AudioDetailsDTO();
        }
    }
}
