using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
	public class VideoRepository : GenericRepository<Video>, IVideoRepository, IRepositoryMarker
    {
		public VideoRepository(IMongoCollection<Video> collection) : base(collection)
		{
		}

        public async Task UpdateAsync(Video video)
			=> await Collection.ReplaceOneAsync(x => x.VideoId == video.VideoId, video);

        public async Task DeleteAsync(Guid videoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, videoId);
            await Collection.DeleteOneAsync(searchFilter);
        }
	}
}
