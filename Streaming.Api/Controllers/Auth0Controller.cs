using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Application.DTO;
using Streaming.Application.Models;
using System.Threading.Tasks;
using Streaming.Infrastructure.Auth0;

namespace Streaming.Api.Controllers
{
    [Route("/Auth0")]
    public class Auth0Controller : ControllerBase
    {
        private readonly IAuth0ManagementTokenAccessor auth0ManagementApiTokenAccessor;

        public Auth0Controller(IAuth0ManagementTokenAccessor auth0ManagementApiTokenAccessor)
        {
            this.auth0ManagementApiTokenAccessor = auth0ManagementApiTokenAccessor;
        }

        [HttpGet]
        [ClaimAuthorize(Claims.CanAccessAuth0Api)]
        public async Task<TokenDTO> GetToken()
            => new TokenDTO
            {
                Token = await auth0ManagementApiTokenAccessor.GetManagementTokenAsync()
            };
    }
}
