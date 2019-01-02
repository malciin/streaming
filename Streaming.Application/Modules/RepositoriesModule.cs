using Autofac;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Streaming.Application.Repository;
using Streaming.Domain.Models.Core;

namespace Streaming.Application.Modules
{
	public class RepositoriesModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<VideoAzureBlobRepository>()
				   .As<IVideoBlobRepository>()
				   .InstancePerLifetimeScope();
		}
	}
}
