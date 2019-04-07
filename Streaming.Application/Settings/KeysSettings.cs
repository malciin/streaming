using Microsoft.Extensions.Configuration;
using Streaming.Application.Interfaces.Settings;
using System.Reflection;

namespace Streaming.Application.Settings
{
    public partial class KeysSettings : IKeysSettings
    {
        public string SecretServerKey { get; private set; }
		public string AzureBlobConnectionString { get; private set; }
        public string ClientSecret { get; private set; }

        public KeysSettings(IConfigurationRoot configuration)
        {
			SecretServerKey = configuration["Hosting:ServerSecretKey"];
			AzureBlobConnectionString = configuration["Hosting:AzureBlobConnectionString"];
            ClientSecret = configuration["Auth0_ManagementApi:ClientSecret"];
			this.GetType().GetMethod("PerformSecretDeveloperBinding", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(this, null);
        }
    }
}
