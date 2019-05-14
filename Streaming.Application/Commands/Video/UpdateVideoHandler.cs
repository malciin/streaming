using System.Security.Authentication;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models;

namespace Streaming.Application.Commands.Video
{
    public class UpdateVideoHandler : ICommandHandler<UpdateVideoCommand>
    {
        private readonly IVideoRepository videoRepo;
        public UpdateVideoHandler(IVideoRepository videoRepo)
        {
            this.videoRepo = videoRepo;
        }

        public async Task HandleAsync(UpdateVideoCommand command)
        {
            var video = await videoRepo.GetSingleAsync(x => x.VideoId == command.VideoId);
            if (video.Owner.UserId == command.User.UserId || command.User.HaveClaim(Claims.CanEditAnyVideo))
            {
                video.SetTitle(command.NewTitle);
                video.SetDescription(command.NewDescription);
                await videoRepo.UpdateAsync(video);
                return;
            }

            throw new InvalidCredentialException("User cannot edit that video");
        }
    }
}
