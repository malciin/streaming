using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using Streaming.Application.Command;

namespace Streaming.Application.Modules
{
    public class CommandModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var currentAssembly = typeof(CommandModule).GetTypeInfo().Assembly;

            builder.RegisterType<CommandBus>()
                   .As<ICommandBus>()
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(currentAssembly)
                   .AsClosedTypesOf(typeof(ICommandHandler<>))
                   .InstancePerLifetimeScope();
        }
    }
}
