using System;
using Streaming.Application.DTO.Video;

namespace Streaming.Application.Commands.Video
{
    public class ProcessVideoCommand : ICommand
    {
        public Guid VideoId { get; set; }
        public string InputFilePath { get; set; }
        public VideoFileDetailsDTO InputFileInfo { get; set; }
    }
}
