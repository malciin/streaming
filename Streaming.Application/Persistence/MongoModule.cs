using Autofac;
using MongoDB.Driver;

namespace Streaming.Application.Persistence
{
    public class MongoModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register<IMongoDatabase>(context => new MongoClient("mongodb://localhost:27017").GetDatabase("streaming")).SingleInstance();
        }
    }
}
