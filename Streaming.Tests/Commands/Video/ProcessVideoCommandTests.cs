using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Video;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Models.Enum;
using Streaming.Common.Extensions;
using Streaming.Domain.Models;
using Streaming.Infrastructure.IoC;
using Streaming.Tests.Mocks;

namespace Streaming.Tests.Commands.Video
{
    public class ProcessVideoCommandTests
    {
        #region TestSetup

        private Guid processVideoId;
        private int howManyTsFilesToGenerate = 5;
        private DirectoryInfo tsFilesDir;
        private DirectoryInfo processingVideosDir;

        private Mock<IVideoPartsFileService> videoPartsFileServiceMock;
        private Mock<IThumbnailService> thumbnailServiceMock;
        private Mock<IProcessVideoService> processVideoServiceMock;
        private Mock<IVideoRepository> videoRepositoryMock;
        private Mock<IVideoFileInfoService> videoFileInfoServiceMock;

        private string SampleMp4Video => Path.Combine(processingVideosDir.FullName, "input.mp4");

        private string GetTsFilePath(Guid videoId, int part) =>
            Path.Combine(tsFilesDir.FullName, $"{videoId}_{part}.ts");

        private string GetProcessedMp4VideoFilePath(Guid videoId) =>
            Path.Combine(processingVideosDir.FullName, $"{videoId}.mp4");

        private string GetThumbnailPath(Guid guid)
            => Path.Combine(processingVideosDir.FullName, $"{guid}.jpg");

        private ICommandDispatcher CommandDispatcher { get; set; }


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            try {
                Directory.Delete("_Data/Processing", true);
            }
            catch (DirectoryNotFoundException) { }
        }

        [SetUp]
        public void Setup()
        {
            processVideoId = Guid.NewGuid();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ServicesModule>();
            containerBuilder.RegisterModule<CommandModule>();

            var messageSignerServiceMock = MessageSignerServiceMock.CreateForRandomGuid();
            containerBuilder.Register(x => messageSignerServiceMock.Object).AsImplementedInterfaces();
            
            processingVideosDir = Directory.CreateDirectory("_Data/Processing");
            tsFilesDir = Directory.CreateDirectory(Path.Combine(processingVideosDir.FullName, "TSFiles"));
            File.Copy(new DirectoryInfo("_Data/VideoSamples").GetFiles().Where(x => x.Extension == ".mp4")
                .Select(x => x.FullName).First(), SampleMp4Video);
            
            var videoProcessingFilePathStrategyMock = new Mock<IVideoProcessingFilesPathStrategy>();
            videoProcessingFilePathStrategyMock.Setup(x => x.TransportStreamDirectoryPath(It.IsAny<Guid>()))
                .Returns((Guid guid) => GetTsFilePath(guid, 0).SubstringToLastOccurence(Path.DirectorySeparatorChar));
            videoProcessingFilePathStrategyMock.Setup(x => x.Mp4ConvertedFilePath(It.IsAny<Guid>()))
                .Returns((Guid guid) => GetProcessedMp4VideoFilePath(guid));
            videoProcessingFilePathStrategyMock.Setup(x => x.ThumbnailFilePath(It.IsAny<Guid>()))
                .Returns((Guid guid) => GetThumbnailPath(guid));
            
            containerBuilder.Register(x => videoProcessingFilePathStrategyMock.Object).InstancePerLifetimeScope();
            
            
            videoPartsFileServiceMock = new Mock<IVideoPartsFileService>();
            containerBuilder.Register(x => videoPartsFileServiceMock.Object).InstancePerLifetimeScope();

            thumbnailServiceMock = new Mock<IThumbnailService>();
            containerBuilder.Register(x => thumbnailServiceMock.Object).InstancePerLifetimeScope();

            videoRepositoryMock = new Mock<IVideoRepository>();
            containerBuilder.Register(x => videoRepositoryMock.Object).InstancePerLifetimeScope();

            processVideoServiceMock = ProcessVideoServiceMock.CreateForData(howManyTsFilesToGenerate);
            containerBuilder.Register(x => processVideoServiceMock.Object).InstancePerLifetimeScope();

            videoFileInfoServiceMock = new Mock<IVideoFileInfoService>();
            videoFileInfoServiceMock.Setup(x => x.GetVideoLengthAsync(It.IsAny<string>()))
                .ReturnsAsync(TimeSpan.FromSeconds(5));
            videoFileInfoServiceMock.Setup(x => x.GetDetailsAsync(It.IsAny<string>()))
                .ReturnsAsync(new VideoFileDetailsDTO {Video = {Codec = VideoCodec.h264}});
            containerBuilder.Register(x => videoFileInfoServiceMock.Object).InstancePerLifetimeScope();
            
            var container = containerBuilder.Build();
            CommandDispatcher = container.Resolve<ICommandDispatcher>();
        }
        
        [TearDown]
        public void Teardown()
            => processingVideosDir.Delete(true);

        #endregion

        [Test]
        public async Task Process_Video_Should_Convert_Video_To_Mp4()
        {
            await PushProcessCommand(processVideoId);
            
            processVideoServiceMock.Verify(x => 
                x.ConvertVideoToMp4Async(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<double>>()), Times.Once);
        }

        [Test]
        public async Task Process_Video_Should_Use_ProcessVideoService_SplitToTsFiles()
        {
            await PushProcessCommand(processVideoId);
            
            processVideoServiceMock.Verify(x => 
                x.SplitMp4FileIntoTsFilesAsync(GetProcessedMp4VideoFilePath(processVideoId), It.IsAny<Func<int, string>>()), Times.Once);
        }

        [Test]
        public async Task Process_Video_Should_Upload_TsFiles()
        {
            await PushProcessCommand(processVideoId);
            
            videoPartsFileServiceMock.Verify(x => x.UploadAsync(processVideoId, It.IsAny<int>(), It.IsAny<Stream>()), Times.Exactly(howManyTsFilesToGenerate));
        }

        [Test]
        public async Task Process_Video_Should_Create_Thumbnail()
        {
            await PushProcessCommand(processVideoId);
            
            processVideoServiceMock.Verify(x =>
                x.TakeVideoScreenshotAsync(GetProcessedMp4VideoFilePath(processVideoId), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public async Task Process_Video_Should_Upload_Thumbnail()
        {
            await PushProcessCommand(processVideoId);
            
            thumbnailServiceMock.Verify(x => 
                x.UploadAsync(processVideoId, It.IsAny<Stream>()));
        }

        [Test]
        public async Task Process_Video_Should_Call_UpdateVideoAfterProcessing_On_Repository()
        {
            await PushProcessCommand(processVideoId);
            
            videoRepositoryMock.Verify(x =>
                x.UpdateAsync(It.IsAny<Streaming.Domain.Models.Video>()));
        }

        #region HelperMethods

        private async Task PushProcessCommand(Guid videoId)
        {
            await CommandDispatcher.HandleAsync(new ProcessVideoCommand
            {
                Video = new Streaming.Domain.Models.Video(videoId, "test", "test", new UserDetails()),
                InputFilePath = SampleMp4Video
            });
        }

        #endregion  
    }
}