using System.Reflection;
using Autofac;
using Streaming.Application.Command;
using Streaming.Application.Command.Bus;

namespace Streaming.Infrastructure.IoC
{
    class CommandModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(CommandDispatcher).GetTypeInfo().Assembly;

            builder.RegisterType<CommandDispatcher>()
                   .As<ICommandDispatcher>()
                   .InstancePerLifetimeScope();

			builder.RegisterType<CommandBus>()
				   .As<ICommandBus>()
				   .SingleInstance();

            builder.RegisterAssemblyTypes(assembly)
                   .AsClosedTypesOf(typeof(ICommandHandler<>))
                   .InstancePerLifetimeScope();
        }
    }
}
