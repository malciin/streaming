namespace Streaming.Application.Settings
{
    public interface IKeysSettings
    {
        string SecretServerKey { get; }
		string AzureBlobConnectionString { get; }
    }
}
