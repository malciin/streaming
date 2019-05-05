
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

        public async Task HandleAsync(DeleteVideoCommand command)
        {
            await videoRepo.DeleteAsync(command.VideoId);
        }
    }
}
