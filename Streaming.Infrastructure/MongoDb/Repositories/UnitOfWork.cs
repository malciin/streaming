using System;
using System.Threading.Tasks;
using Autofac;
using MongoDB.Driver;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Settings;
using Streaming.Domain.Models;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IComponentContext componentContext;

        private readonly Lazy<ILiveStreamRepository> liveStreamRepository;
        private readonly Lazy<IVideoRepository> videoRepository;

        private readonly IClientSessionHandle clientSessionHandle;

        public ILiveStreamRepository LiveStreams => liveStreamRepository.Value;
        public IVideoRepository Videos => videoRepository.Value;

        private Func<T> GetLazyRepositoryInstantiation<T, TDomainObj>()
        {
            return () =>
            {
                var sesionTransactionCollection = clientSessionHandle.Client.GetDatabase(MongoDbNames.DatabaseName)
                    .GetCollection<TDomainObj>(MongoDbNames.CollectionNames[typeof(TDomainObj)]);
                return componentContext.Resolve<T>(new TypedParameter(typeof(IMongoCollection<TDomainObj>), sesionTransactionCollection));
            };
        }

        public UnitOfWork(IClientSessionHandle clientSessionHandle, IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            this.clientSessionHandle = clientSessionHandle;
            liveStreamRepository = new Lazy<ILiveStreamRepository>(GetLazyRepositoryInstantiation<ILiveStreamRepository, LiveStream>());
            videoRepository = new Lazy<IVideoRepository>(GetLazyRepositoryInstantiation<IVideoRepository, Video>());
        }

        public async Task CommitAsync()
        {
            await clientSessionHandle.CommitTransactionAsync();
        }
    }
}
