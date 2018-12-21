using Microsoft.Extensions.Configuration;
using Streaming.Application.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Api.Configurations
{
    public class DirectoriesSettings : IDirectoriesSettings
    {
        public string ProcessingDirectory { get; }
        public string ProcessedDirectory { get; }
        
        public DirectoriesSettings(IConfigurationRoot Configuration)
        {
            ProcessingDirectory = Configuration["Directories:VideoToProcessDirectory"];
            ProcessedDirectory = Configuration["Directories:ProcessedVideoDirectory"];
        }
    }
}
