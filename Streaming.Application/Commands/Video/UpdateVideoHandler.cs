using System.Security.Claims;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models;
using Streaming.Application.Models.Repository.Video;
using Streaming.Application.Extensions;

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
            var updateVideoInfo = new UpdateVideoInfo
            {
                UpdateByVideoId = command.VideoId,
                NewVideoTitle = command.NewTitle,
                NewVideoDescription = command.NewDescription
            };

            if (!command.User.HasStreamingClaim(Claims.CanEditAnyVideo))
            {
                updateVideoInfo.UpdateByUserIdentifier = command.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            await videoRepo.UpdateAsync(updateVideoInfo);
        }
    }
}
