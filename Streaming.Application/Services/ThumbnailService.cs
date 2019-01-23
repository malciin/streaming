using Streaming.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly IAzureBlobClient blobClient;
        private readonly static string blobContainerName = "thumbnails";

        public ThumbnailService(IAzureBlobClient blobClient)
        {
            this.blobClient = blobClient;
        }

        public string GetThumbnailUrl(Guid VideoId)
        {
            return blobClient.GetFileLink(blobContainerName, BlobNameHelper.GetThumbnailFilename(VideoId));
        }

        public async Task<Stream> GetThumbnailAsync(Guid VideoId)
        {
            return await blobClient.GetFileAsync(blobContainerName, BlobNameHelper.GetThumbnailFilename(VideoId));
        }

        public async Task UploadAsync(Guid VideoId, Stream Stream)
        {
            await blobClient.UploadFileAsync(blobContainerName, BlobNameHelper.GetThumbnailFilename(VideoId), Stream);
        }

        public string GetPlaceholderThumbnailUrl()
        {
            throw new NotImplementedException();
        }
    }
}
