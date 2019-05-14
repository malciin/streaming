using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Models;

namespace Streaming.Application.Interfaces.Repositories
{
	public interface IFilterableRepository<T> where T : class
	{
		Task<T> GetSingleAsync(Expression<Func<T, bool>> filter);
        Task<IPackage<T>> GetAsync(Expression<Func<T, bool>> filter, int skip = 0, int limit = 0);
        Task<int> CountAsync(Expression<Func<T, bool>> filter);
	}
}
