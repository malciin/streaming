using System;

namespace Streaming.Application.Interfaces.Strategies
{
    public interface IThumbnailLocalPathStrategy
    {
        string GetThumbnailPath(Guid videoId);
    }
}
