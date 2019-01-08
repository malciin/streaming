using System;

namespace Streaming.Application.Services
{
    public interface IThumbnailService
    {
        string GetThumbnailUrl(Guid VideoId);
    }
}
