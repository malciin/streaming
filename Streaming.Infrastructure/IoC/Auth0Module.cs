using Autofac;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Infrastructure.Auth0;

namespace Streaming.Infrastructure.IoC
{
    public class Auth0Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Auth0ManagementTokenAccessor>()
                .As<IAuth0ManagementTokenAccessor>()
                .SingleInstance();

            builder.RegisterType<Auth0UserRepository>()
                .As<IUserRepository>();
        }
    }
}