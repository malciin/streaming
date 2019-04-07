using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface IThumbnailService
    {
        string GetThumbnailUrl(Guid VideoId);
        Task<Stream> GetThumbnailAsync(Guid VideoId);
        Task UploadAsync(Guid VideoId, Stream Stream);
    }
}
