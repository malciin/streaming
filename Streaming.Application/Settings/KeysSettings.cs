﻿using Microsoft.Extensions.Configuration;

namespace Streaming.Application.Settings
{
    public partial class KeysSettings : IKeysSettings
    {
        public string SecretServerKey { get; private set; }
		public string AzureBlobConnectionString { get; private set; }

		public KeysSettings(IConfigurationRoot configuration)
        {
			SecretServerKey = configuration["Hosting:ServerSecretKey"];
			AzureBlobConnectionString = configuration["Hosting:AzureBlobConnectionString"];
			this.GetType().GetMethod("PerformSecretDeveloperBinding")?.Invoke(this, null);
        }
    }
}
