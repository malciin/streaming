using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Application.DTO;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using System.Threading.Tasks;

namespace Streaming.Api.Controllers
{
    [Route("/Auth0")]
    public class Auth0Controller : ControllerBase
    {
        private readonly IAuth0Client auth0Client;

        public Auth0Controller (IAuth0Client auth0Client)
        {
            this.auth0Client = auth0Client;
        }

        [HttpGet]
        [ClaimAuthorize(Claims.CanAccessAuth0Api)]
        public async Task<TokenDTO> GetToken()
            => new TokenDTO
            {
                Token = await auth0Client.GetTokenAsync()
            };
    }
}
