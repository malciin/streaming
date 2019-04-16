using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.Services
{
    class Auth0ClientWrapper : IAuth0Client
    {
        private readonly IAuth0ManagementApiSettings managementApiSettings;
        private readonly IAuth0SecretClientKey secretKey;
        private object locker = new object();

        private DateTime lastTokenGeneratedDateTime;
        private ManagementApiClient client;

        ManagementApiClient GetClient()
        {
            if (client == null || DateTime.UtcNow.Subtract(lastTokenGeneratedDateTime).TotalHours > 12)
            {
                lock (locker)
                {
                    var token = GetTokenAsync().GetAwaiter().GetResult();
                    lastTokenGeneratedDateTime = DateTime.UtcNow;
                    client = new ManagementApiClient(token, new Uri(managementApiSettings.Audience));
                }
            }
            return client;
        }

        public Auth0ClientWrapper(IAuth0ManagementApiSettings managementApiSettings, IAuth0SecretClientKey secretKey)
        {
            this.managementApiSettings = managementApiSettings;
            this.secretKey = secretKey;
        }

        public Task<User> GetInfoAsync(string userIdentifier)
        {
            var client = GetClient();
            return client.Users.GetAsync(userIdentifier);
        }

        public async Task<string> GetTokenAsync()
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
