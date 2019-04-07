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
    public class VideoFilesLocalService : IVideoFilesService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IVideoFilesPathStrategy pathStrategy;

        public VideoFilesLocalService(IHttpContextAccessor httpContextAccessor, IVideoFilesPathStrategy pathStrategy)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.pathStrategy = pathStrategy;
        }

        public Task<Stream> GetVideoAsync(Guid VideoId, int PartNumber)
        {
            return Task.FromResult(File.OpenRead(pathStrategy.TransportStreamFilePath(VideoId, PartNumber)) as Stream);
        }

        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            return UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) +
                $"/Video/{VideoId}/{PartNumber}";
        }

        public async Task UploadAsync(Guid VideoId, int PartNumber, Stream Stream)
        {
            Directory.CreateDirectory(pathStrategy.TransportStreamFilePath(VideoId, 0).SubstringToLastOccurence(Path.DirectorySeparatorChar));
            using (var writeStream = File.OpenWrite(pathStrategy.TransportStreamFilePath(VideoId, PartNumber)))
            {
                await Stream.CopyToAsync(writeStream);
            }
        }
    }
}
