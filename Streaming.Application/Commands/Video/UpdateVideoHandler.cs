using System.Security.Claims;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models;
using Streaming.Application.Models.Repository.Video;
using Streaming.Common.Extensions;

namespace Streaming.Application.Commands.Video
{
    public class UpdateVideoHandler : ICommandHandler<UpdateVideoCommand>
    {
        private readonly IVideoRepository videoRepo;
        public UpdateVideoHandler(IVideoRepository videoRepo)
        {
            this.videoRepo = videoRepo;
        }

        public async Task HandleAsync(UpdateVideoCommand Command)
        {
            var updateVideoInfo = new UpdateVideoInfo
            {
                UpdateByVideoId = Command.VideoId,
                NewVideoTitle = Command.NewTitle,
                NewVideoDescription = Command.NewDescription
            };

            if (!Command.User.HasStreamingClaim(Claims.CanEditAnyVideo))
            {
                updateVideoInfo.UpdateByUserIdentifier = Command.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            await videoRepo.UpdateAsync(updateVideoInfo);
            await videoRepo.CommitAsync();
        }
    }
}
