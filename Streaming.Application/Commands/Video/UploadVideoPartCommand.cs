using System.IO;

namespace Streaming.Application.Commands.Video
{
    public class UploadVideoPartCommand : ICommand
    {
        public string UploadToken { get; set; }
        public string PartMD5Hash { get; set; }
        public Stream PartStream { get; set; }
    }
}
