using Streaming.Application.Services;
using Streaming.Application.Settings;
using Streaming.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Repository
{
    public class VideoAzureBlobRepository : IVideoBlobRepository
	{
		private readonly AzureBlobClient blobClient;
		public VideoAzureBlobRepository(IKeysSettings keysSettings)
		{
			blobClient = new AzureBlobClient(keysSettings.AzureBlobConnectionString);
		}

		public async Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber)
		{
			return await blobClient.GetFileAsync("videos", VideoBlobNameHelper.GetVideoName(VideoId, PartNumber));
		}

        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            return blobClient.GetFileLinkSASAuthorization("videos", VideoBlobNameHelper.GetVideoName(VideoId, PartNumber));
        }

        public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
		{
			await blobClient.UploadFileAsync("videos", VideoBlobNameHelper.GetVideoName(VideoId, PartNumber), Stream);
		}
	}
}