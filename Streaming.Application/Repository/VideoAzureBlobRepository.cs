﻿using Newtonsoft.Json;
using Streaming.Application.Services;
using Streaming.Application.Settings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
			return await blobClient.GetFileAsync("videos", $"{VideoId}_{PartNumber}.ts");
		}

		public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
		{
			await blobClient.UploadFileAsync("videos", $"{VideoId}_{PartNumber}.ts", Stream);
		}
	}
}