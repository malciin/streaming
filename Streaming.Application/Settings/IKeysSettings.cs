using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Settings
{
    public interface IKeysSettings
    {
        string SecretServerKey { get; }
    }
}
