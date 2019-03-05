namespace Streaming.Application.Interfaces.Settings
{
    public interface IAzureBlobConnectionString
    {
        string AzureBlobConnectionString { get; }
    }

    public interface ISecretServerKey
    {
        string SecretServerKey { get; }
    }

    public interface IAuth0SecretClientKey
    {
        string ClientSecret { get; }
    }

    public interface IKeysSettings : IAzureBlobConnectionString, ISecretServerKey, IAuth0SecretClientKey
    {
		
    }
}
