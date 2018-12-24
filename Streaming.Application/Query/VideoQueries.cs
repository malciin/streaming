using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Streaming.Application.DTO.Video;
using Streaming.Application.Repository;
using Streaming.Application.Settings;
using Streaming.Common.Helpers;
using Streaming.Domain.Models.Core;

namespace Streaming.Application.Query
{
    public class VideoQueries : IVideoQueries
    {
        private readonly IMapper mapper;
        private readonly IMongoCollection<Video> collection;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IDirectoriesSettings directorySettings;
		private readonly IVideoBlobRepository videoBlobRepo;

        public VideoQueries(IMapper mapper, 
			IMongoCollection<Video> collection,
			IHttpContextAccessor httpContextAccessor, 
			IDirectoriesSettings directorySettings,
			IVideoBlobRepository videoBlobRepo)
        {
            this.mapper = mapper;
            this.collection = collection;
			this.httpContextAccessor = httpContextAccessor;
			this.directorySettings = directorySettings;
			this.videoBlobRepo = videoBlobRepo;
        }

        public async Task<VideoBasicMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            return await collection.Find<Video>(searchFilter)
                .Project(x => mapper.Map<VideoBasicMetadataDTO>(x))
                .FirstOrDefaultAsync();
        }

		public async Task<string> GetVideoManifestAsync(Guid VideoId)
		{
			var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
			var results = collection.Find<Video>(searchFilter).ToList();
			var all = collection.Find<Video>(_ => true).ToList();

			var rawManifest = await collection
				.Find<Video>(searchFilter)
				.Project(x => x.VideoManifestHLS).FirstOrDefaultAsync();

			var getVideoPartEndpoint = UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) + "/Video";
			var manifestStr = rawManifest
				.Replace("[ENDPOINT]", getVideoPartEndpoint)
				.Replace("[ID]", VideoId.ToString());

			return manifestStr;
		}

		private string GetFileNameByPart(int Part)
		{
			var result = Part.ToString();
			return new string('0', 3 - result.Length) + result + ".ts";
		}

		public async Task<Stream> GetVideoPartAsync(Guid VideoId, int Part)
		{
			return await videoBlobRepo.GetVideoAsync(VideoId, Part);
		}

		public async Task<IEnumerable<VideoBasicMetadataDTO>> SearchAsync(VideoSearchDTO Search)
		{
			var results = await collection
				.Find(_ => true)
				.Skip(Search.Offset)
				.Limit(Search.HowMuch)
				.ToListAsync();

			return results
				.Select(x => mapper.Map<VideoBasicMetadataDTO>(x));
		}
	}
}
