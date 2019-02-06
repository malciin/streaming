﻿using Microsoft.Extensions.Configuration;

namespace Streaming.Application.Settings
{
    public class DirectoriesSettings : IDirectoriesSettings
    {
        public string ProcessingDirectory { get; }
        public string ProcessedDirectory { get; }
        public string LogsDirectory { get; }

        public DirectoriesSettings(IConfigurationRoot Configuration)
        {
            ProcessingDirectory = Configuration["Directories:VideoToProcessDirectory"];
            ProcessedDirectory = Configuration["Directories:ProcessedVideoDirectory"];
            LogsDirectory = Configuration["Directories:LogsDirectory"];
        }
    }
}
