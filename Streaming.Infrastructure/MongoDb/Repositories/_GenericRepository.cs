using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
    public class _GenericRepository<T> : IFilterableRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> Collection;
        public _GenericRepository(IMongoCollection<T> collection)
        {
            this.Collection = collection;
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> filter)
            => await Collection.Find(filter).FirstAsync();

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter, int skip = 0, int limit = 0)
        {
            var fluentFindDefinition = Collection.Find(filter);

            if (skip != 0)
            {
                fluentFindDefinition = fluentFindDefinition.Skip(skip);
            }
            if (limit != 0)
            {
                fluentFindDefinition = fluentFindDefinition.Limit(limit);
            }

            return await fluentFindDefinition.ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await Collection.InsertOneAsync(entity);
        }
    }
}
