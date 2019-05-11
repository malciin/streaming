using System.Collections.Generic;
using System.Threading.Tasks;
using Streaming.Domain.Models;

namespace Streaming.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserDetails> GetSingleAsync(string userId);
        Task<IEnumerable<UserDetails>> GetAsync(params string[] userIds);
    }
}