using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Repository
{
	public class VideoMongoDbBlobRepository : IVideoBlobRepository
	{
		IGridFSBucket gridBucket;

		public VideoMongoDbBlobRepository(IGridFSBucket gridBucket)
		{
			this.gridBucket = gridBucket;
		}

		public async Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber)
		{
			return await gridBucket.OpenDownloadStreamByNameAsync($"{VideoId}_{PartNumber}.ts");
		}

        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            throw new NotImplementedException();
        }

        public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
		{
			await gridBucket.UploadFromStreamAsync($"{VideoId}_{PartNumber}.ts", Stream);
		}
	}
}
