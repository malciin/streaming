using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

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
