using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Settings;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.Auth0
{
    public class Auth0UserRepository : IUserRepository
    {
        private static volatile ManagementApiClient _client;
        private static DateTime _lastTokenGeneratedDateTime;
        private static readonly object _locker = new object();
        
        private readonly IAuth0ManagementTokenAccessor managementTokenAccessor;
        private readonly IAuth0ManagementApiSettings managementApiSettings;

        private static bool Auth0ClientNotInstantiatedOrExpired() =>
            _client == null || DateTime.UtcNow.Subtract(_lastTokenGeneratedDateTime).TotalHours > 12;

        ManagementApiClient GetClient()
        {
            if (Auth0ClientNotInstantiatedOrExpired())
            {
                lock (_locker)
                {
                    if (Auth0ClientNotInstantiatedOrExpired())
                    {
                        var token = managementTokenAccessor.GetManagementTokenAsync().GetAwaiter().GetResult();
                        _lastTokenGeneratedDateTime = DateTime.UtcNow;
                        _client = new ManagementApiClient(token, new Uri(managementApiSettings.Audience));
                    }
                }
            }
            return _client;
        }

        public Auth0UserRepository(IAuth0ManagementApiSettings managementApiSettings, IAuth0ManagementTokenAccessor managementTokenAccessor)
        {
            this.managementApiSettings = managementApiSettings;
            this.managementTokenAccessor = managementTokenAccessor;
        }

        public async Task<UserDetails> GetSingleAsync(string userIdentifier)
        {
            var client = GetClient();
            var user = await client.Users.GetAsync(userIdentifier);
            return new UserDetails
            {
                Email = user.Email,
                Nickname = user.NickName,
                UserId = userIdentifier
            };
        }

        public async Task<IEnumerable<UserDetails>> GetAsync(params string[] userIdentifiers)
        {
            var client = GetClient();
            var users = await client.Users.GetAllAsync(new GetUsersRequest
            {
                Fields = "email,nickname,user_id",
                Query = $"(user_id:({String.Join(" OR ", userIdentifiers)})"
            }, new PaginationInfo(1, 100, false));
            return users.Select(x => new UserDetails
            {
                UserId = x.UserId,
                Email = x.Email,
                Nickname = x.NickName
            });
        }
    }
}