using Autofac;
using Streaming.Application.Query;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Streaming.Application.Modules
{
    public class QueryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var currentAssembly = typeof(QueryModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(currentAssembly)
                   .InNamespaceOf<IVideoQueries>()
                   .AsImplementedInterfaces();
        }
    }
}
