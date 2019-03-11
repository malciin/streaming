using MongoDB.Driver;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
	public abstract class AbstractSessionMongoDbRepository
	{
		private readonly IMongoClient client;
		private IClientSessionHandle handle;

		protected async ValueTask<IClientSessionHandle> getCurrentSessionHandlerAsync()
		{
			if (handle == null)
			{
				handle = await client.StartSessionAsync();
			}
			return handle;
		}

		public AbstractSessionMongoDbRepository(IMongoClient client)
		{
			this.client = client;
		}

		public async Task CommitAsync()
		{
			if (handle == null)
			{
				throw new MongoException("Session is not started...");
			}
			await handle.CommitTransactionAsync();
		}
	}
}
