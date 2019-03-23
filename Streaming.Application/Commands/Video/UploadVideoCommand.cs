using Streaming.Application.Models.DTO.Video;

namespace Streaming.Application.Commands.Video
{
    public class UploadVideoCommand : ICommand
    {
        public string UploadToken { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UserDetailsDTO User { get; set; }
    }
}
