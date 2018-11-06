using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Configuration
{
    public interface IDirectoriesConfig
    {
        string ProcessingDirectory { get; }
        string ProcessedDirectory { get; }
    }
}