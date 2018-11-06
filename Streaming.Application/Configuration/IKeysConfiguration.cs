using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Configuration
{
    public interface IKeysConfiguration
    {
        string SecretServerKey { get; }
    }
}
