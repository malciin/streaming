using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Configuration
{
    public interface IDirectoriesConfiguration
    {
        string ProcessingDirectory { get; }
        string ProcessedDirectory { get; }
    }
}