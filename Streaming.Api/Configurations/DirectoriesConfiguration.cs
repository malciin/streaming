using Microsoft.Extensions.Configuration;
using Streaming.Application.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Api.Configurations
{
    public class DirectoriesConfiguration : IDirectoriesConfiguration
    {

        public DirectoriesConfiguration(IConfigurationRoot Configuration)
        {
            ProcessingDirectory = Configuration["Directories:VideoToProcessDirectory"];
            ProcessedDirectory = Configuration["Directories:ProcessedVideoDirectory"];
        }

        public string ProcessingDirectory { get; }

        public string ProcessedDirectory { get; }
    }
}
