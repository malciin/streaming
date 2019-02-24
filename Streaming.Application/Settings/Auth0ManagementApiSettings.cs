using Microsoft.Extensions.Configuration;

namespace Streaming.Application.Settings
{
    public class Auth0ManagementApiSettings : IAuth0ManagementApiSettings
    {
        public string Audience { get; set; }
        public string ClientId { get; set; }

        public Auth0ManagementApiSettings(IConfigurationRoot configuration)
        {
            Audience = configuration["Auth0_ManagementApi:Audience"];
            ClientId = configuration["Auth0_ManagementApi:ClientId"];
        }
    }
}
