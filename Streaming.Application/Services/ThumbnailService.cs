﻿using Streaming.Application.Strategies;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly IAzureBlobClient blobClient;
        private readonly IFileNameStrategy fileNameStrategy;
        private readonly static string blobContainerName = "thumbnails";

        public ThumbnailService(IAzureBlobClient blobClient, IFileNameStrategy fileNameStrategy)
        {
            this.blobClient = blobClient;
            this.fileNameStrategy = fileNameStrategy;
        }

        public string GetThumbnailUrl(Guid VideoId)
        {
            return blobClient.GetFileLink(blobContainerName, fileNameStrategy.GetThumbnailFileName(VideoId));
        }

        public async Task<Stream> GetThumbnailAsync(Guid VideoId)
        {
            return await blobClient.GetFileAsync(blobContainerName, fileNameStrategy.GetThumbnailFileName(VideoId));
        }

        public async Task UploadAsync(Guid VideoId, Stream Stream)
        {
            await blobClient.UploadFileAsync(blobContainerName, fileNameStrategy.GetThumbnailFileName(VideoId), Stream);
        }

        public string GetPlaceholderThumbnailUrl()
        {
            throw new NotImplementedException();
        }
    }
}
