using MongoDB.Driver;
using Streaming.Application.DTO.Video;
using Streaming.Application.Mappings;
using Streaming.Application.Services;
using Streaming.Application.Settings;
using Streaming.Common.Extensions;
using Streaming.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Streaming.Application.Query
{
    public class VideoQueries : IVideoQueries
    {
        private readonly VideoMappingService mapper;
        private readonly IMongoCollection<Video> collection;
		private readonly IDirectoriesSettings directoriesSettings;
		private readonly IVideoBlobService videoBlobService;

        public VideoQueries(VideoMappingService mapper, 
			IMongoCollection<Video> collection,
			IDirectoriesSettings directoriesSettings,
            IVideoBlobService videoBlobService,
            IThumbnailService thumbnailService)
        {
            this.mapper = mapper;
            this.collection = collection;
			this.directoriesSettings = directoriesSettings;
			this.videoBlobService = videoBlobService;
        }

        public async Task<VideoMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            return await collection.Find<Video>(searchFilter)
                .Project(x => mapper.MapVideoMetadataDTO(x))
                .FirstOrDefaultAsync();
        }

		public async Task<string> GetVideoManifestAsync(Guid VideoId)
		{
			var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
			var results = collection.Find<Video>(searchFilter).ToList();

			var rawManifest = await collection
				.Find<Video>(searchFilter)
				.Project(x => x.VideoManifestHLS).FirstOrDefaultAsync();

            var pattern = VideoManifest.EndpointPlaceholder.Replace("[", "\\[");
            var match = Regex.Match(rawManifest, pattern);
            int partNum = 0;
            while (match.Success)
            {
                rawManifest = rawManifest.Replace(match.Index, match.Length,
                    videoBlobService.GetVideoUrl(VideoId, partNum++));
                match = Regex.Match(rawManifest, pattern);
            }
            return rawManifest;
		}

		public async Task<Stream> GetVideoPartAsync(Guid VideoId, int Part)
		{
			return await videoBlobService.GetVideoAsync(VideoId, Part);
		}

		public async Task<IEnumerable<VideoMetadataDTO>> SearchAsync(VideoSearchDTO Search)
		{
			var results = await collection
				.Find(x => x.State.HasFlag(VideoState.Processed))
				.Skip(Search.Offset)
				.Limit(Search.HowMuch)
                .SortByDescending(x => x.FinishedProcessingDate)
				.ToListAsync();

			return results
				.Select(x => mapper.MapVideoMetadataDTO(x));
		}
	}
}
