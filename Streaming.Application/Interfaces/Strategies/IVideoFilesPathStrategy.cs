using System;

namespace Streaming.Application.Interfaces.Strategies
{
    public interface IVideoFilesPathStrategy
    {
        string TransportStreamFilePath(Guid videoId, int partNumber);
    }
}
