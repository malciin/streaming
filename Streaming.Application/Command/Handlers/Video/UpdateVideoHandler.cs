using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Command.Commands.Video;
using Streaming.Application.Models;
using Streaming.Common.Extensions;

namespace Streaming.Application.Command.Handlers.Video
{
    public class UpdateVideoHandler : ICommandHandler<UpdateVideoCommand>
    {
        private readonly IMongoCollection<Domain.Models.Video> videoCollection;
        public UpdateVideoHandler(IMongoCollection<Domain.Models.Video> videoCollection)
        {
            this.videoCollection = videoCollection;
        }

        public async Task HandleAsync(UpdateVideoCommand Command)
        {
            var filters = new List<FilterDefinition<Domain.Models.Video>>();
            filters.Add(Builders<Domain.Models.Video>.Filter
                .Eq(x => x.VideoId, Command.VideoId));

            if (!Command.User.HasStreamingClaim(Claims.CanEditAnyVideo))
            {
                filters.Add(Builders<Domain.Models.Video>.Filter
                    .Eq(x => x.Owner.Identifier, Command.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            }

            var updateDefinition = Builders<Domain.Models.Video>.Update
                .Set(x => x.Title, Command.NewTitle)
                .Set(x => x.Description, Command.NewDescription);

            await videoCollection.UpdateOneAsync(Builders<Domain.Models.Video>.Filter.And(filters), updateDefinition);
        }
    }
}
