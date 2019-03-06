﻿
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Streaming.Application.Commands.Video
{
    public class DeleteVideoHandler : ICommandHandler<DeleteVideoCommand>
    {
        private readonly IMongoCollection<Domain.Models.Video> videoCollection;
        public DeleteVideoHandler(IMongoCollection<Domain.Models.Video> videoCollection)
        {
            this.videoCollection = videoCollection;
        }

        public async Task HandleAsync(DeleteVideoCommand Command)
        {
            var idFilter = Builders<Domain.Models.Video>.Filter.Eq(x => x.VideoId, Command.VideoId);
            await videoCollection.DeleteOneAsync(idFilter);
        }
    }
}