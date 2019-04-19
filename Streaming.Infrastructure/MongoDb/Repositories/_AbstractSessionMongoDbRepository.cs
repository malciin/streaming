using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Infrastructure.MongoDb.Repositories
{
	public abstract class _AbstractSessionMongoDbRepository
	{
        private List<Func<Task>> asyncCommits { get; }

        public _AbstractSessionMongoDbRepository()
        {
            this.asyncCommits = new List<Func<Task>>();
        }

        protected void addToCommit(Func<Task> commit)
        {
            asyncCommits.Add(commit);
        }

        // Mocked transactions, because of using mongoDb that have transactions available only in replica set
        // This is more for compatibility, rather than for maintain a database in stable state
		public async Task CommitAsync()
		{
            foreach(var task in asyncCommits)
            {
                await task();
            }
		}
	}
}
