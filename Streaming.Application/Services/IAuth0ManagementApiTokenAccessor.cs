﻿using Streaming.Application.DTO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public interface IAuth0ManagementApiTokenAccessor
    {
        Task<TokenDTO> GetTokenAsync();
    }
}