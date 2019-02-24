using Streaming.Application.DTO.Auth0;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public interface IAuth0ManagementApiTokenAccessor
    {
        Task<TokenDTO> GetTokenAsync();
    }
}