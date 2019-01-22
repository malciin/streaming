using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Strategies
{
    public interface IVideoUrlStrategy
    {
        string GetVideoUrl(Guid VideoId, int PartNumber);
    }
}
