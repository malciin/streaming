using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Video;
using Streaming.Application.Interfaces.Strategies;
using Streaming.Common.Exceptions;
using Streaming.Infrastructure.IoC;
using Streaming.Tests.Mocks;

namespace Streaming.Tests.Commands.Video
{
    public class UploadVideoPartCommandTests
    {
        private Guid returnedGuidFromUploadToken;
        private DirectoryInfo rawVideosUploadDir;
        private DirectoryInfo videoSamplesDir;

        private string SampleMp4Video => videoSamplesDir.GetFiles().Where(x => x.Extension == ".mp4")
            .Select(x => x.FullName).First();
        private string VideoPartPath => Path.Combine(rawVideosUploadDir.FullName, returnedGuidFromUploadToken.ToString());
        private ICommandDispatcher CommandDispatcher { get; set; }
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            try {
                Directory.Delete("_Data/RawVideos", true);
            }
            catch (DirectoryNotFoundException) { }
        }
        
        [SetUp]
        public void Setup()
        {
            returnedGuidFromUploadToken = Guid.NewGuid();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<CommandModule>();
            containerBuilder.RegisterModule<StrategiesModule>();

            var messageSignerServiceMock = MessageSignerServiceMock.CreateForRandomGuid();
            containerBuilder.Register(x => messageSignerServiceMock.Object).AsImplementedInterfaces();
            
            rawVideosUploadDir = new DirectoryInfo("_Data/RawVideos");
            videoSamplesDir = new DirectoryInfo("_Data/VideoSamples");
            
            var videoProcessingFilePathStrategyMock = new Mock<IVideoProcessingFilesPathStrategy>();
            videoProcessingFilePathStrategyMock.Setup(x => x.RawUploadedVideoFilePath(It.IsAny<Guid>()))
                .Returns(VideoPartPath);
            containerBuilder.Register(x => videoProcessingFilePathStrategyMock.Object).InstancePerLifetimeScope();
            
            var container = containerBuilder.Build();
            CommandDispatcher = container.Resolve<ICommandDispatcher>();
        }

        [Test]
        public void Wrong_MD5_Hash_Should_Throw_HashesNotEqualException()
        {
            Assert.Throws<HashesNotEqualException>(() => CommandDispatcher.HandleAsync(new UploadVideoPartCommand
            {
                UploadToken = Convert.ToBase64String(new byte[] {0x15}),
                PartStream = new MemoryStream(new byte[] {0x16}),
                PartMD5Hash = Convert.ToBase64String(MD5.Create().ComputeHash(new MemoryStream(new byte[] {0x15})))
            }).GetAwaiter().GetResult());
        }

        [Test]
        public async Task Should_Produce_Valid_Final_File()
        {
            byte[] fileBytes;
            using (var inputFile = File.OpenRead(SampleMp4Video))
            {
                fileBytes = new byte[inputFile.Length];
                await inputFile.ReadAsync(fileBytes, 0, (int)inputFile.Length);
            }

            for (int i = 0; i < fileBytes.Length; i += 100_000)
            {
                var command = GetUploadVideoPartCommandFor(fileBytes.Skip(i).Take(100_000).ToArray());
                await CommandDispatcher.HandleAsync(command);
            }

            using (var outputFile = File.OpenRead(VideoPartPath))
            {
                Assert.AreEqual(outputFile.Length, fileBytes.Length, $"Output video file have different length!");
                foreach (byte @byte in fileBytes)
                    Assert.AreEqual(@byte, (byte)outputFile.ReadByte());
            }
        }

        private UploadVideoPartCommand GetUploadVideoPartCommandFor(byte[] bytes)
        {
            var command = new UploadVideoPartCommand
            {
                UploadToken = Convert.ToBase64String(new byte[] {0x15}),
                PartStream = new MemoryStream(bytes),
                PartMD5Hash = Convert.ToBase64String(MD5.Create().ComputeHash(bytes))
            };
            return command;
        }
    }
}