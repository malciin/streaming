
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;

namespace Streaming.Application.Commands.Video
{
    public class DeleteVideoHandler : ICommandHandler<DeleteVideoCommand>
    {
        private readonly IVideoRepository videoRepo;
        public DeleteVideoHandler(IVideoRepository videoRepo)
        {
            this.videoRepo = videoRepo;
        }

        public async Task HandleAsync(DeleteVideoCommand Command)
        {
            await videoRepo.DeleteAsync(Command.VideoId);
            await videoRepo.CommitAsync();
        }
    }
}
