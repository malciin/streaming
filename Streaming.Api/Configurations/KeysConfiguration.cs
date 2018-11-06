using Microsoft.Extensions.Configuration;
using Streaming.Application.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Api.Configurations
{
    public partial class KeysConfiguration : IKeysConfiguration
    {
        public string SecretServerKey { get; private set; }

        public KeysConfiguration(IConfigurationRoot configuration)
        {
            SecretServerKey = configuration["Hosting:ServerSecretKey"];
            this.GetType().GetMethod("PerformSecretDeveloperBinding")?.Invoke(this, null);
        }
    }
}
