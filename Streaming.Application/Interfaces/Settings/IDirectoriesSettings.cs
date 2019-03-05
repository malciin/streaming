namespace Streaming.Application.Interfaces.Settings
{
    public interface ILocalStorageDirectorySettings
    {
        string LocalStorageDirectory { get; }
    }

    public interface IProcessingDirectorySettings
    {
        string ProcessingDirectory { get; }
    }

    public interface IDirectoriesSettings : IProcessingDirectorySettings, ILocalStorageDirectorySettings
    {
        
        string LogsDirectory { get; }
    }
}