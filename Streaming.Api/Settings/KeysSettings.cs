using Microsoft.Extensions.Configuration;
using Streaming.Application.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Api.Configurations
{
    public partial class KeysSettings : IKeysSettings
    {
        public string SecretServerKey { get; private set; }

        public KeysSettings(IConfigurationRoot configuration)
        {
            SecretServerKey = configuration["Hosting:ServerSecretKey"];
            this.GetType().GetMethod("PerformSecretDeveloperBinding")?.Invoke(this, null);
        }
    }
}
