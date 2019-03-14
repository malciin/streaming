using System;

namespace Streaming.Application.DTO.Video
{
    public class VideoFileDetailsDTO
    {
        public class AudioDetailsDTO
        {
            public string Codec { get; set; }
        }

        public class VideoDetailsDTO
        {
            public string Codec { get; set; }
            public (int xResolution, int yResolution) Resolution { get; set; }
            public int BitrateKbs { get; set; }
        }

        public VideoDetailsDTO Video { get; private set; }
        public AudioDetailsDTO Audio { get; private set; }

        public TimeSpan Duration { get; set; }

        public VideoFileDetailsDTO()
        {
            Video = new VideoDetailsDTO();
            Audio = new AudioDetailsDTO();
        }
    }
}
