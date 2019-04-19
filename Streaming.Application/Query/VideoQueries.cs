using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Mappings;
using Streaming.Application.Models.DTO.Video;
using Streaming.Common.Extensions;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
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
        private readonly VideoMappings mapper;
        private readonly IFilterableRepository<Video> filterableVideos;
		private readonly IVideoFilesService videoBlobService;

        public VideoQueries(VideoMappings mapper,
            IProcessVideoService processVideoService,
            IVideoRepository filterableVideos,
            IVideoFilesService videoBlobService)
        {
            this.mapper = mapper;
            this.filterableVideos = filterableVideos;
			this.videoBlobService = videoBlobService;
        }

        public async Task<VideoMetadataDTO> GetBasicVideoMetadataAsync(Guid videoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, videoId);
            return mapper.MapVideoMetadataDTO(await filterableVideos.SingleAsync(x => x.VideoId == videoId));
        }

		public async Task<string> GetVideoManifestAsync(Guid videoId)
		{
            var rawManifest = (await filterableVideos.SingleAsync(x => x.VideoId == videoId)).VideoManifestHLS;

            var pattern = VideoManifest.EndpointPlaceholder.Replace("[", "\\[");
            var match = Regex.Match(rawManifest, pattern);
            int partNum = 0;
            while (match.Success)
            {
                rawManifest = rawManifest.Replace(match.Index, match.Length,
                    videoBlobService.GetVideoUrl(videoId, partNum++));
                match = Regex.Match(rawManifest, pattern);
            }
            return rawManifest;
		}

		public async Task<Stream> GetVideoPartAsync(Guid videoId, int part)
		{
			return await videoBlobService.GetVideoAsync(videoId, part);
		}

		public async Task<IEnumerable<VideoMetadataDTO>> SearchAsync(VideoSearchDTO search)
		{
			return (await filterableVideos.GetAsync(x => 
					x.Title.Contains(String.Join(" ", search.Keywords)) &&
                    x.State.HasFlag(VideoState.Processed), skip: search.Offset, limit: search.HowMuch))
				.Select(x => mapper.MapVideoMetadataDTO(x));
		}
    }
}
