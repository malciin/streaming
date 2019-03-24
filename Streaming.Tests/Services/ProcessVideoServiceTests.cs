using Autofac;
using NUnit.Framework;
using Streaming.Infrastructure.IoC.Extensions;
using System.IO;

namespace Streaming.Tests.Services
{
    class ProcessVideoServiceTests
    {
        IComponentContext componentContext;
        DirectoryInfo testDirectory;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.UseDefaultModules();
            componentContext = builder.Build();

            testDirectory = Directory.CreateDirectory("Services/Tests");
            Directory.CreateDirectory("Services/ProcessVideoService/Tests");
        }

        [Test]
        public void TestMethod()
        {
        }
    }
}
