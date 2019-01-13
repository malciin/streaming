using Streaming.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class VideoAzureBlobService : IVideoBlobService
	{
		private readonly IAzureBlobClient blobClient;
        private readonly static string blobContainerName = "videos";
		public VideoAzureBlobService(IAzureBlobClient blobClient)
		{
            this.blobClient = blobClient;
		}

		public async Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber)
		{
			return await blobClient.GetFileAsync(blobContainerName, BlobNameHelper.GetVideoFilename(VideoId, PartNumber));
		}

        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            return blobClient.GetFileLinkSecuredSAS(blobContainerName, BlobNameHelper.GetVideoFilename(VideoId, PartNumber));
        }

        public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
		{
			await blobClient.UploadFileAsync(blobContainerName, BlobNameHelper.GetVideoFilename(VideoId, PartNumber), Stream);
		}
	}
}