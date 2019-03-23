using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Repository.Video;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
	public class VideoRepository : AbstractSessionMongoDbRepository, IVideoRepository
	{
		private readonly IMongoCollection<Video> videoCollection;

		public VideoRepository(IMongoCollection<Video> videoCollection)
		{
			this.videoCollection = videoCollection;
		}

		public async Task AddAsync(Video video)
		{
            this.addToCommit(() => videoCollection.InsertOneAsync(video));
		}

		public async Task<IEnumerable<Video>> SearchAsync(VideoSearchDTO filter)
		{
            return await videoCollection
                .Find(x => x.State.HasFlag(VideoState.Processed))
                .Skip(filter.Offset)
                .Limit(filter.HowMuch)
                .SortByDescending(x => x.FinishedProcessingDate)
                .ToListAsync();
        }

		public async Task<Video> GetAsync(Guid videoId)
		{
            var searchFilter = Builders<Domain.Models.Video>.Filter.Eq(x => x.VideoId, videoId);
            return await videoCollection.Find(searchFilter).FirstAsync();
        }

		public async Task UpdateAsync(Video video)
		{
			var idFilter = Builders<Video>.Filter.Eq(x => x.VideoId, video.VideoId);
			var updateDefinition = Builders<Video>.Update.Set(x => x.Title, video.Title);

            this.addToCommit(() => videoCollection.UpdateOneAsync(idFilter, updateDefinition));
		}

        public async Task UpdateAsync(UpdateVideoInfo updateVideoInfo)
        {
            var filters = new List<FilterDefinition<Domain.Models.Video>>();
            filters.Add(Builders<Domain.Models.Video>.Filter
                .Eq(x => x.VideoId, updateVideoInfo.UpdateByVideoId));

            if (!String.IsNullOrEmpty(updateVideoInfo.UpdateByUserIdentifier))
            {
                filters.Add(Builders<Domain.Models.Video>.Filter
                    .Eq(x => x.Owner.UserId, updateVideoInfo.UpdateByUserIdentifier));
            }

            var updateDefinition = Builders<Domain.Models.Video>.Update
                .Set(x => x.Title, updateVideoInfo.NewVideoTitle)
                .Set(x => x.Description, updateVideoInfo.NewVideoDescription);

            this.addToCommit(() => videoCollection.UpdateOneAsync(Builders<Domain.Models.Video>.Filter.And(filters), updateDefinition));
        }

        public async Task UpdateAsync(UpdateVideoAfterProcessing updateVideoAfterProcessing)
        {
            var searchFilter = Builders<Domain.Models.Video>.Filter.Eq(x => x.VideoId, updateVideoAfterProcessing.VideoId);
            var updateDefinition = Builders<Domain.Models.Video>.Update
                .Set(x => x.FinishedProcessingDate, updateVideoAfterProcessing.FinishedProcessingDate)
                .Set(x => x.ProcessingInfo, updateVideoAfterProcessing.ProcessingInfo)
                .Set(x => x.VideoManifestHLS, updateVideoAfterProcessing.VideoManifestHLS)
                .Set(x => x.Length, updateVideoAfterProcessing.VideoLength)
                .Set(x => x.State, updateVideoAfterProcessing.VideoState);

            this.addToCommit(() => videoCollection.UpdateOneAsync(searchFilter, updateDefinition));
        }

        public async Task DeleteAsync(Guid VideoId)
        {
            var searchFilter = Builders<Domain.Models.Video>.Filter.Eq(x => x.VideoId, VideoId);
            this.addToCommit(() => videoCollection.DeleteOneAsync(searchFilter));
        }
    }
}
