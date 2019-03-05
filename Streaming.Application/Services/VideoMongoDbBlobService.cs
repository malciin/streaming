using Microsoft.AspNetCore.Http;
using MongoDB.Driver.GridFS;
using Streaming.Application.Interfaces.Services;
using Streaming.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
	public class VideoMongoDbBlobService : IVideoBlobService
	{
		private readonly IGridFSBucket gridBucket;
        private readonly IHttpContextAccessor httpContextAccessor;

		public VideoMongoDbBlobService(IGridFSBucket gridBucket, IHttpContextAccessor httpContextAccessor)
		{
			this.gridBucket = gridBucket;
            this.httpContextAccessor = httpContextAccessor;
		}

		public async Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber)
		{
			return await gridBucket.OpenDownloadStreamByNameAsync($"{VideoId}_{PartNumber}.ts");
		}

        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            return UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) +
                $"/Video/{VideoId}/{PartNumber}";
        }

        public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
		{
			await gridBucket.UploadFromStreamAsync($"{VideoId}_{PartNumber}.ts", Stream);
		}
	}
}
