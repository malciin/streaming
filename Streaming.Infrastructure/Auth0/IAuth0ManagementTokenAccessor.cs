using System.Threading.Tasks;

namespace Streaming.Infrastructure.Auth0
{
    public interface IAuth0ManagementTokenAccessor
    {
        Task<string> GetManagementTokenAsync();
    }
}