﻿using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Api.Models;
using Streaming.Application.DTO.Auth0;
using Streaming.Application.Services;
using System.Threading.Tasks;

namespace Streaming.Api.Controllers
{
    [Route("/Auth0")]
    public class Auth0Controller : ControllerBase
    {
        private readonly IAuth0ManagementApiTokenAccessor tokenAccessor;

        public Auth0Controller (IAuth0ManagementApiTokenAccessor tokenAccessor)
        {
            this.tokenAccessor = tokenAccessor;
        }

        [HttpGet]
        [ClaimAuthorize(Claims.CanAccessAuth0Api, Claims.CanDeleteVideo)]
        public async Task<TokenDTO> GetToken()
            => await tokenAccessor.GetTokenAsync();
    }
}