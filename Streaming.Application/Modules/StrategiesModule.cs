using Autofac;
using Streaming.Application.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Modules
{
    public class StrategiesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<BlobManifestEndpointStrategy>()
                   .As<IManifestEndpointStrategy>()
                   .SingleInstance();
        }
    }
}
