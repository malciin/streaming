using System;
using System.Net;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using NUnit.Framework;
using Streaming.Application.DTO;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Models;
using Streaming.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Streaming.Tests.EndToEnd.Auth0Controller
{
    public class Auth0ControllerTests : EndToEndTestClass
    {
        [Test]
        public void GetValidAuth0ApiToken()
        {
            WebHost.DontUseDatabase()
                   .ConfigureTestUser(Claims.CanAccessAuth0Api)
                   .Start();

            var response = Client.GetAsync($"{WebHost.ApiUri}Auth0").GetAwaiter().GetResult();
            Assert.True(response.IsSuccessStatusCode, $"Not success status code! Status code was: {response.StatusCode} " +
                "- firstly check that you've got an internet connection");
            var token = response.Content.ReadFromJsonAsObject<TokenDTO>();

            var auth0Settings = WebHost.Services.GetService<IAuth0ManagementApiSettings>();
            var client = new ManagementApiClient(token.Token, new Uri(auth0Settings.Audience));
            Assert.DoesNotThrowAsync(() => client.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo()),
                "Token was generated, however cannot communicate with Auth0 api");
        }

        [Test]
        public void GetAuth0ApiToken_SecureTest()
        {
            WebHost.DontUseDatabase()
                   .Start();

            var response = Client.GetAsync($"{WebHost.ApiUri}Auth0").GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized,
                $"Expected 401/403 status code for unauthorized user but status code was '{response.StatusCode}'");

            WebHost.ConfigureTestUser(Claims.CanUploadVideo, Claims.CanDeleteVideo);
            response = Client.GetAsync($"{WebHost.ApiUri}Auth0").GetAwaiter().GetResult();
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            WebHost.ConfigureTestUser(Claims.CanAccessAuth0Api);
            response = Client.GetAsync($"{WebHost.ApiUri}Auth0").GetAwaiter().GetResult();
            Assert.IsTrue(response.IsSuccessStatusCode);
        }
    }
}
