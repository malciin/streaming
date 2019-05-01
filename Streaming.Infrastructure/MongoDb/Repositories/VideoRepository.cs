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

        public async Task UpdateAsync(UpdateVideoInfo updateVideoInfo)
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

            await Collection.UpdateOneAsync(Builders<Video>.Filter.And(filters), updateDefinition);
        }

        public async Task UpdateAsync(UpdateVideoAfterProcessing updateVideoAfterProcessing)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, updateVideoAfterProcessing.VideoId);
            var updateDefinition = Builders<Video>.Update
                .Set(x => x.FinishedProcessingDate, updateVideoAfterProcessing.FinishedProcessingDate)
                .Set(x => x.ProcessingInfo, updateVideoAfterProcessing.ProcessingInfo)
                .Set(x => x.VideoManifest, updateVideoAfterProcessing.VideoManifest)
                .Set(x => x.Length, updateVideoAfterProcessing.VideoLength)
                .Set(x => x.State, updateVideoAfterProcessing.VideoState);

            await Collection.UpdateOneAsync(searchFilter, updateDefinition);
        }

        public async Task DeleteAsync(Guid videoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, videoId);
            await Collection.DeleteOneAsync(searchFilter);
        }
	}
}
