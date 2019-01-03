using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Strategies
{
    public interface IManifestEndpointStrategy
    {
        string SetEndpoints(Guid VideoId, string ManifestString);
    }
}
