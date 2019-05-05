using System;
using Autofac;
using Moq;

namespace Streaming.Tests.TestExtensions
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Provide mocked object for unused types just to prevent Autofac.DependencyResolutionException on unused services 
        /// (for example when we test method that should throw exception early)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterUnused(this ContainerBuilder builder, params Type[] types)
        {
            foreach(var type in types)
            {
                var creatorMockObjectType = typeof(Mock<>).MakeGenericType(type);
                var ctr = creatorMockObjectType.GetConstructor(new Type[] { });
                var creator = ctr.Invoke(new object[] { });
                var props = creator.GetType().GetProperties();
                var mockedObject = creator.GetType().GetProperty("Object", type).GetValue(creator);
                builder.Register(x => mockedObject).As(type);
            }
            return builder;
        }
    }
}
