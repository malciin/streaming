using AutoMapper;
using MongoDB.Driver;
using Streaming.Application.DTO.Video;
using Streaming.Application.Services;
using Streaming.Application.Settings;
using Streaming.Application.Strategies;
using Streaming.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Application.Query
{
    public class VideoQueries : IVideoQueries
    {
        private readonly IMapper mapper;
        private readonly IMongoCollection<Video> collection;
		private readonly IManifestEndpointStrategy manifestEndpointStrategy;
		private readonly IDirectoriesSettings directorySettings;
		private readonly IVideoBlobService videoBlobService;

        public VideoQueries(IMapper mapper, 
			IMongoCollection<Video> collection,
            IManifestEndpointStrategy manifestEndpointStrategy, 
			IDirectoriesSettings directorySettings,
            IVideoBlobService videoBlobService)
        {
            this.mapper = mapper;
            this.collection = collection;
			this.manifestEndpointStrategy = manifestEndpointStrategy;
			this.directorySettings = directorySettings;
			this.videoBlobService = videoBlobService;
        }

        public async Task<VideoMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            return await collection.Find<Video>(searchFilter)
                .Project(x => mapper.Map<VideoMetadataDTO>(x))
                .FirstOrDefaultAsync();
        }

		public async Task<string> GetVideoManifestAsync(Guid VideoId)
		{
			var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
			var results = collection.Find<Video>(searchFilter).ToList();

			var rawManifest = await collection
				.Find<Video>(searchFilter)
				.Project(x => x.VideoManifestHLS).FirstOrDefaultAsync();

			return manifestEndpointStrategy.SetEndpoints(VideoId, rawManifest);
		}

		public async Task<Stream> GetVideoPartAsync(Guid VideoId, int Part)
		{
			return await videoBlobService.GetVideoAsync(VideoId, Part);
		}

		public async Task<IEnumerable<VideoMetadataDTO>> SearchAsync(VideoSearchDTO Search)
		{
			var results = await collection
				.Find(x => x.FinishedProcessingDate != null)
				.Skip(Search.Offset)
				.Limit(Search.HowMuch)
                .SortByDescending(x => x.FinishedProcessingDate)
				.ToListAsync();

			return results
				.Select(x => mapper.Map<VideoMetadataDTO>(x));
		}
	}
}
