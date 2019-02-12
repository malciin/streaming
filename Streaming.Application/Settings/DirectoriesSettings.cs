﻿using Microsoft.Extensions.Configuration;
using Streaming.Common.Extensions;
using System.IO;

namespace Streaming.Application.Settings
{
    public class DirectoriesSettings : IDirectoriesSettings
    {
        public string LocalStorageDirectory { get; }
        public string ProcessingDirectory { get; }
        public string LogsDirectory { get; }

        public DirectoriesSettings(IConfigurationRoot Configuration)
        {
            LocalStorageDirectory = Configuration["Directories:LocalStorageDirectory"].NormalizePathForOS();
            ProcessingDirectory = Configuration["Directories:VideoProcessingDirectory"].NormalizePathForOS();
            LogsDirectory = Configuration["Directories:LogsDirectory"].NormalizePathForOS();
        }
    }
}
