namespace Streaming.Application.Settings
{
    public interface ILocalStorageDirectory
    {
        string LocalStorageDirectory { get; }
    }

    public interface IDirectoriesSettings : ILocalStorageDirectory
    {
        string ProcessingDirectory { get; }
        string LogsDirectory { get; }
    }
}