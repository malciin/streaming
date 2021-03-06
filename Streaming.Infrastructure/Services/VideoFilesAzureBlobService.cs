﻿using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.Services
{
    public class VideoFilesAzureBlobService : IVideoFilesService
	{
		private readonly IAzureBlobClient blobClient;
        private readonly IFileNameStrategy fileNameStrategy;
        private readonly static string blobContainerName = "videos";
		public VideoFilesAzureBlobService(IAzureBlobClient blobClient, IFileNameStrategy fileNameStrategy)
		{
            this.blobClient = blobClient;
            this.fileNameStrategy = fileNameStrategy;
		}

		public async Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber)
		{
			return await blobClient.GetFileAsync(blobContainerName, fileNameStrategy.GetSplittedVideoFileName(VideoId, PartNumber));
		}

        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            return blobClient.GetFileLinkSecuredSAS(blobContainerName, fileNameStrategy.GetSplittedVideoFileName(VideoId, PartNumber));
        }

        public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
		{
			await blobClient.UploadFileAsync(blobContainerName, fileNameStrategy.GetSplittedVideoFileName(VideoId, PartNumber), Stream);
		}
	}
}