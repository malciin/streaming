using Microsoft.AspNetCore.Http;
using Streaming.Application.Settings;
using Streaming.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class LocalStorageVideoBlobService : IVideoBlobService
    {
        private readonly string localStorageDirectoryPath;
        private readonly IHttpContextAccessor httpContextAccessor;
        public LocalStorageVideoBlobService(ILocalStorageDirectorySettings localStorageDirectory, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            localStorageDirectoryPath = $"{localStorageDirectory.LocalStorageDirectory}{Path.DirectorySeparatorChar}localStorageVideoBlob";
            Directory.CreateDirectory(localStorageDirectoryPath);
        }

        private string getVideoPath(Guid videoId, int partNumber)
            => $"{localStorageDirectoryPath}{Path.DirectorySeparatorChar}{BlobNameHelper.GetVideoFilename(videoId, partNumber)}";

        public Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber)
        {
            return Task.FromResult(File.OpenRead(getVideoPath(VideoId, PartNumber)) as Stream);
        }

        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            return UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) +
                $"/Video/{VideoId}/{PartNumber}";
        }

        public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
        {
            using (var writeStream = File.OpenWrite(getVideoPath(VideoId, PartNumber)))
            {
                await Stream.CopyToAsync(writeStream);
            }
        }
    }
}
