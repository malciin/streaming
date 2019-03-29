using System.IO;

namespace Streaming.Application.Commands.Live
{
    public class UploadTsPartCommand : ICommand
    {
        public Stream Part { get; set; }
        public string StreamKey { get; set; }
        public string FileName { get; set; }
        public string FileManifestDetails { get; set; }
    }
}
