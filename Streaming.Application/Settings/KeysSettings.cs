using Microsoft.Extensions.Configuration;

namespace Streaming.Application.Settings
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
