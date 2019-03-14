using MongoDB.Driver;
using Streaming.Application.DTO.Video;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Mappings;
using Streaming.Common.Extensions;
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
        private readonly IVideoRepository videoRepo;
		private readonly IDirectoriesSettings directoriesSettings;
		private readonly IVideoBlobService videoBlobService;
        private readonly IProcessVideoService processVideoService;

        public VideoQueries(VideoMappings mapper,
            IProcessVideoService processVideoService,
            IVideoRepository videoRepo,
			IDirectoriesSettings directoriesSettings,
            IVideoBlobService videoBlobService)
        {
            this.mapper = mapper;
            this.videoRepo = videoRepo;
			this.directoriesSettings = directoriesSettings;
			this.videoBlobService = videoBlobService;
            this.processVideoService = processVideoService;
        }

        public async Task<VideoMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            return mapper.MapVideoMetadataDTO(await videoRepo.GetAsync(VideoId));
        }

		public async Task<string> GetVideoManifestAsync(Guid VideoId)
		{
            var rawManifest = (await videoRepo.GetAsync(VideoId)).VideoManifestHLS;

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
			return (await videoRepo.SearchAsync(Search))
				.Select(x => mapper.MapVideoMetadataDTO(x));
		}

        public IEnumerable<(string Extension, string Codec)> SupportedVideoTypes()
            => processVideoService.SupportedVideoTypes();
    }
}
