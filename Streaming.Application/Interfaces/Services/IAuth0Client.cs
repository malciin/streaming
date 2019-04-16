using Auth0.ManagementApi.Models;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Services
{
    public interface IAuth0Client
    {
        Task<User> GetInfoAsync(string userIdentifier);
        Task<string> GetTokenAsync();
    }
}
