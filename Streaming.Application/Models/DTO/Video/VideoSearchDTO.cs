namespace Streaming.Application.Models.DTO.Video
{
    public class VideoSearchDTO
    {
        public string[] Keywords { get; set; }
        public int Offset { get; set; }
        public int HowMuch { get; set; }
    }
}
