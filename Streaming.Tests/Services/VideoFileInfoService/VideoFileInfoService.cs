using Autofac;
using NUnit.Framework;
using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Infrastructure.IoC.Extensions;
using System.IO;

namespace Streaming.Tests.Services.VideoFileInfoService
{
    class VideoFileInfoService
    {
        private IComponentContext componentContext;

        [SetUp]
        public void Setup()
        {
            var container = new ContainerBuilder();
            container.UseDefaultModules();
            componentContext = container.Build();
        }

        [Test]
        public void File_Not_Found_Exception_Should_Be_Thrown_When_File_Not_Exists()
        {
            var videoFileInfo = componentContext.Resolve<IVideoFileInfoService>();
            Assert.ThrowsAsync<FileNotFoundException>(() => videoFileInfo.GetDetailsAsync("Unexisting/Path"));
            Assert.ThrowsAsync<FileNotFoundException>(() => videoFileInfo.GetVideoLengthAsync("Unexisting/Path"));
        }

        [Test]
        public void Not_Video_File_Exception_Should_Be_Thrown_When_File_Is_Not_Video()
        {
            var videoFileInfo = componentContext.Resolve<IVideoFileInfoService>();
            Assert.ThrowsAsync<NotVideoFileException>(() => videoFileInfo.GetDetailsAsync("_Data/VideoSamples/Not_Video_File.dat"));
        }
    }
}
