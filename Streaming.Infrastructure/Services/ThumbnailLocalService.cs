using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Common.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.Services
{
    public class ThumbnailLocalService : IThumbnailService
    {
        private readonly IThumbnailLocalPathStrategy pathStrategy;

        public ThumbnailLocalService(IThumbnailLocalPathStrategy pathStrategy)
        {
            this.pathStrategy = pathStrategy;
        }

        public Task<Stream> GetThumbnailAsync(Guid VideoId)
        {
            return Task.FromResult(File.Open(pathStrategy.GetThumbnailPath(VideoId), FileMode.Open, FileAccess.Read) as Stream);
        }

        public string GetThumbnailUrl(Guid VideoId)
        {
            return pathStrategy.GetThumbnailPath(VideoId);
        }

        public async Task UploadAsync(Guid VideoId, Stream Stream)
        {
            var thumbnailPath = pathStrategy.GetThumbnailPath(VideoId);
            Directory.CreateDirectory(thumbnailPath.SubstringToLastOccurence('/'));
            using (var file = File.Open(thumbnailPath, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                await Stream.CopyToAsync(file);
            }
        }
    }
}
