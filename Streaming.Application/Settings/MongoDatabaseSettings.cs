using Microsoft.Extensions.Configuration;
using Streaming.Application.Interfaces.Settings;

namespace Streaming.Application.Settings
{
    public class MongoDatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; private set; }

        public MongoDatabaseSettings(IConfigurationRoot Configuration)
        {
            ConnectionString = Configuration["Database:ConnectionString"];
        }
    }
}
