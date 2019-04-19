using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models.Repository.Video;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
	public class VideoRepository : _GenericRepository<Video>, IVideoRepository
	{
		public VideoRepository(IMongoCollection<Video> collection) : base(collection)
		{
		}

		public Task UpdateAsync(Video video)
		{
			var idFilter = Builders<Video>.Filter.Eq(x => x.VideoId, video.VideoId);
			var updateDefinition = Builders<Video>.Update.Set(x => x.Title, video.Title);

            this.addToCommit(() => collection.UpdateOneAsync(idFilter, updateDefinition));
            return Task.FromResult(0);
        }

        public Task UpdateAsync(UpdateVideoInfo updateVideoInfo)
        {
            var filters = new List<FilterDefinition<Video>>();
            filters.Add(Builders<Video>.Filter
                .Eq(x => x.VideoId, updateVideoInfo.UpdateByVideoId));

            if (!String.IsNullOrEmpty(updateVideoInfo.UpdateByUserIdentifier))
            {
                filters.Add(Builders<Video>.Filter
                    .Eq(x => x.Owner.UserId, updateVideoInfo.UpdateByUserIdentifier));
            }

            var updateDefinition = Builders<Video>.Update
                .Set(x => x.Title, updateVideoInfo.NewVideoTitle)
                .Set(x => x.Description, updateVideoInfo.NewVideoDescription);

            this.addToCommit(() => collection.UpdateOneAsync(Builders<Video>.Filter.And(filters), updateDefinition));
            return Task.FromResult(0);
        }

        public Task UpdateAsync(UpdateVideoAfterProcessing updateVideoAfterProcessing)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, updateVideoAfterProcessing.VideoId);
            var updateDefinition = Builders<Video>.Update
                .Set(x => x.FinishedProcessingDate, updateVideoAfterProcessing.FinishedProcessingDate)
                .Set(x => x.ProcessingInfo, updateVideoAfterProcessing.ProcessingInfo)
                .Set(x => x.VideoManifestHLS, updateVideoAfterProcessing.VideoManifestHLS)
                .Set(x => x.Length, updateVideoAfterProcessing.VideoLength)
                .Set(x => x.State, updateVideoAfterProcessing.VideoState);

            this.addToCommit(() => collection.UpdateOneAsync(searchFilter, updateDefinition));
            return Task.FromResult(0);
        }

        public Task DeleteAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            this.addToCommit(() => collection.DeleteOneAsync(searchFilter));
            return Task.FromResult(0);
        }
	}
}
