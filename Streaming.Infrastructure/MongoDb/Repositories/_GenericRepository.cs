using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Models;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
    public class GenericRepository<T> : IFilterableRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> Collection;

        protected GenericRepository(IMongoCollection<T> collection)
        {
            this.Collection = collection;
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> filter)
            => await Collection.Find(filter).FirstAsync();

        public async Task<IPackage<T>> GetAsync(Expression<Func<T, bool>> filter, int skip = 0, int limit = 0)
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

            return Package<T>.CreatePackage(await fluentFindDefinition.ToListAsync(), await CountAsync(filter));
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            return (int)(await Collection.CountDocumentsAsync(filter));
        }

        public virtual async Task AddAsync(T entity)
        {
            await Collection.InsertOneAsync(entity);
        }
    }
}
