using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Streaming.Application.Interfaces.Repositories
{
	public interface IFilterableRepository<T> where T : class
	{
		Task<T> GetSingleAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter, int skip = 0, int limit = 0);
	}
}
