using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
    public class _GenericRepository<T> : _AbstractSessionMongoDbRepository, IFilterableRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> collection;
        public _GenericRepository(IMongoCollection<T> collection)
        {
            this.collection = collection;
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> filter)
            => await collection.Find(filter).FirstAsync();

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter, int skip = 0, int limit = 0)
        {
            var fluentFindDefinition = collection.Find(filter);

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

        public virtual Task AddAsync(T entity)
        {
            this.addToCommit(() => collection.InsertOneAsync(entity));
            return Task.FromResult(0);
        }
    }
}
