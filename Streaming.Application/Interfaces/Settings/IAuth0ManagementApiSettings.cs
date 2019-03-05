namespace Streaming.Application.Interfaces.Settings
{
    public interface IAuth0ManagementApiSettings
    {
        string Audience { get; set; }
        string ClientId { get; set; }
    }
}
