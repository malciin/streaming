using Autofac;
using NUnit.Framework;
using Streaming.Application.Interfaces.Services;
using Streaming.Infrastructure.IoC;

namespace Streaming.Tests.Services
{
    class ProcessVideoServiceTests
    {
        private IProcessVideoService processVideoService;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ServicesModule>();
            processVideoService = builder.Build().Resolve<IProcessVideoService>();
        }
    }
}
