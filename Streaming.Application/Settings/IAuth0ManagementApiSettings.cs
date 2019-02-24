namespace Streaming.Application.Settings
{
    public interface IAuth0ManagementApiSettings
    {
        string Audience { get; set; }
        string ClientId { get; set; }
    }
}
