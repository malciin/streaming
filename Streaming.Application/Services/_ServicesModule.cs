using Autofac;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Services
{
    public class _ServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<VideoService>().As<IVideoService>().InstancePerLifetimeScope();
        }
    }
}
