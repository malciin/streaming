using Streaming.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Services
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly IAzureBlobClient blobClient;
        private readonly static string blobContainerName = "thumbnails";
        public string GetThumbnailUrl(Guid VideoId)
        {
            return blobClient.GetFileLink(blobContainerName, BlobNameHelper.GetThumbnailFilename(VideoId));
        }
    }
}
