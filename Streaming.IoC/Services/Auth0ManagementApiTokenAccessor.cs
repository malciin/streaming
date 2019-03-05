using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Streaming.Application.DTO;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.Services
{
    public class Auth0ManagementApiTokenAccessor : IAuth0ManagementApiTokenAccessor
    {
        private readonly Uri audience;
        private readonly string clientId;
        private readonly string clientSecret;

        public Auth0ManagementApiTokenAccessor(IAuth0ManagementApiSettings managementApiSettings, IAuth0SecretClientKey secretKey)
        {
            audience = new Uri(managementApiSettings.Audience);
            clientId = managementApiSettings.ClientId;
            clientSecret = secretKey.ClientSecret;
        }

        public async Task<TokenDTO> GetTokenAsync()
        {
            var bodyString = JsonConvert.SerializeObject(new
            {
                grant_type = "client_credentials",
                client_id = clientId,
                client_secret = clientSecret,
                audience = audience
            });
            var content = new StringContent(bodyString, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                var url = $"{audience.Scheme}://{audience.Authority}/oauth/token";
                var response = await httpClient.PostAsync(url, content);
                var contentBody = await response.Content.ReadAsStringAsync();
                return new TokenDTO
                {
                    Token = JObject.Parse(contentBody)["access_token"].ToString()
                };
            }
            
        }
    }
}
