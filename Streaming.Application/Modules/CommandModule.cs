using System.Reflection;
using Autofac;
using Streaming.Application.Command;
using Streaming.Application.Command.Bus;

namespace Streaming.Application.Modules
{
    public class CommandModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var currentAssembly = typeof(CommandModule).GetTypeInfo().Assembly;

            builder.RegisterType<CommandDispatcher>()
                   .As<ICommandDispatcher>()
                   .InstancePerLifetimeScope();

			builder.RegisterType<CommandBus>()
				   .As<ICommandBus>()
				   .SingleInstance();

            builder.RegisterAssemblyTypes(currentAssembly)
                   .AsClosedTypesOf(typeof(ICommandHandler<>))
                   .InstancePerLifetimeScope();
        }
    }
}
