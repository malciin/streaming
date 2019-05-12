namespace Streaming.Application.Commands.Video
{
    public class ProcessVideoCommand : ICommand
    {
        public Domain.Models.Video Video { get; set; }
        public string InputFilePath { get; set; }
    }
}