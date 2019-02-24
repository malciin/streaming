using Microsoft.Extensions.Configuration;

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
