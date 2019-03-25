using Autofac;
using Streaming.Application.Events;
using System.Reflection;

namespace Streaming.Infrastructure.IoC
{
    public class EventModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = typeof(EventEmmiter).GetTypeInfo().Assembly;

            builder.RegisterType<EventEmmiter>()
                   .As<IEventEmmiter>()
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assembly)
                   .AsClosedTypesOf(typeof(IEventReceiver<>))
                   .InstancePerLifetimeScope();
        }
    }
}
