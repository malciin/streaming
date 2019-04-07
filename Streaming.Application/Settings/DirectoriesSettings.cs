using Microsoft.Extensions.Configuration;
using Streaming.Application.Interfaces.Settings;
using Streaming.Common.Extensions;

namespace Streaming.Application.Settings
{
    public class DirectoriesSettings : ILocalStorageDirectorySettings, ILogsDirectorySettings
    {
        public string LocalStorageDirectory { get; }
        public string LogsDirectory { get; }

        public DirectoriesSettings(IConfigurationRoot Configuration)
        {
            LocalStorageDirectory = Configuration["Directories:LocalStorageDirectory"].NormalizePathForOS();
            LogsDirectory = Configuration["Directories:LogsDirectory"].NormalizePathForOS();
        }
    }
}
