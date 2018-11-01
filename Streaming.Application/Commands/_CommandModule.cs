using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Streaming.Domain.Command;

namespace Streaming.Application.Commands
{
    public class _CommandModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.IsAssignableTo<ICommandHandler>())
                .AsImplementedInterfaces();

            builder.Register<Func<Type, ICommandHandler>>(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return t =>
                {
                    var handlerType = typeof(ICommandHandler<>).MakeGenericType(t);
                    return (ICommandHandler)ctx.Resolve(handlerType);
                };
            });
            builder.RegisterType<_CommandBus>().AsImplementedInterfaces();
        }
    }
}
