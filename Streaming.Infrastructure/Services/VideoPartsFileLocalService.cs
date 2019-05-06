using Microsoft.AspNetCore.Http;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Common.Extensions;
using Streaming.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.Services
{
    public class VideoPartsFileLocalService : IVideoPartsFileService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IVideoFilesPathStrategy pathStrategy;

        public VideoPartsFileLocalService(IHttpContextAccessor httpContextAccessor, IVideoFilesPathStrategy pathStrategy)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.pathStrategy = pathStrategy;
        }

        public Task<Stream> GetVideoAsync(Guid videoId, int partNumber)
        {
            return Task.FromResult(File.OpenRead(pathStrategy.TransportStreamFilePath(videoId, partNumber)) as Stream);
        }

        public string GetVideoUrl(Guid videoId, int partNumber)
        {
            return UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) +
                $"/Video/{videoId}/{partNumber}";
        }

        public async Task UploadAsync(Guid videoId, int partNumber, Stream stream)
        {
            Directory.CreateDirectory(pathStrategy.TransportStreamFilePath(videoId, 0).SubstringToLastOccurence(Path.DirectorySeparatorChar));
            using (var writeStream = File.OpenWrite(pathStrategy.TransportStreamFilePath(videoId, partNumber)))
            {
                await stream.CopyToAsync(writeStream);
            }
        }
    }
}
