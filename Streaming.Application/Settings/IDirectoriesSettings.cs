using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Settings
{
    public interface IDirectoriesSettings
    {
        string ProcessingDirectory { get; }
        string ProcessedDirectory { get; }
    }
}