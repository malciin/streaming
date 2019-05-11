using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Streaming.Application.Interfaces.Settings;

namespace Streaming.Infrastructure.Auth0
{
    public class Auth0ManagementTokenAccessor : IAuth0ManagementTokenAccessor
    {
        private readonly IAuth0ManagementApiSettings managementApiSettings;
        private readonly IAuth0SecretClientKey secretKey;

        public Auth0ManagementTokenAccessor(IAuth0ManagementApiSettings managementApiSettings, IAuth0SecretClientKey secretKey)
        {
            this.managementApiSettings = managementApiSettings;
            this.secretKey = secretKey;
        }

        public async Task<string> GetManagementTokenAsync()
        {
            var audience = new Uri(managementApiSettings.Audience);
            var bodyString = JsonConvert.SerializeObject(new
            {
                grant_type = "client_credentials",
                client_id = managementApiSettings.ClientId,
                client_secret = secretKey.ClientSecret,
                audience = audience.AbsoluteUri
            });

            var content = new StringContent(bodyString, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                var url = $"{audience.Scheme}://{audience.Authority}/oauth/token";
                var response = await httpClient.PostAsync(url, content);
                var contentBody = await response.Content.ReadAsStringAsync();
                return JObject.Parse(contentBody)["access_token"].ToString();
            }
        }
    }
}