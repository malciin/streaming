using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.DTO.Video;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
	public class VideoRepository : AbstractSessionMongoDbRepository, IVideoRepository
	{
		private readonly IMongoCollection<Video> videoCollection;

		public VideoRepository(IMongoCollection<Video> videoCollection) : base(videoCollection.Database.Client)
		{
			this.videoCollection = videoCollection;
		}

		public async Task AddAsync(Video video)
		{
			await videoCollection.InsertOneAsync(await this.getCurrentSessionHandlerAsync(), video);
		}

		public Task<IEnumerable<Video>> SearchAsync(VideoSearchDTO filter)
		{
			throw new NotImplementedException();
		}

		public Task<Video> GetAsync(Guid videoId)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateAsync(Video video)
		{
			var idFilter = Builders<Video>.Filter.Eq(x => x.VideoId, video.VideoId);
			var updateDefinition = Builders<Video>.Update.Set(x => x.Title, video.Title);
			await videoCollection.UpdateOneAsync(await this.getCurrentSessionHandlerAsync(), idFilter, updateDefinition);
		}
	}
}
