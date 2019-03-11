using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Streaming.Api.Controllers;
using Streaming.Application.Commands;
using Streaming.Application.DTO.Video;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Application.Mappings;
using Streaming.Application.Models;
using Streaming.Application.Query;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using Streaming.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Streaming.Tests
{
    public class VideoTests
    {
        public ContainerBuilder GetBaseMockedContainerBuilder()
        {
            var builder = new ContainerBuilder();
            //builder.RegisterModule<Infrastructure.IoC.CommandModule>();

            var secretServerKey = new Mock<ISecretServerKey>();
            secretServerKey.Setup(x => x.SecretServerKey).Returns("test");
            builder.Register(x => secretServerKey.Object).AsImplementedInterfaces();

            var assembly = typeof(CommandDispatcher).GetTypeInfo().Assembly;
            builder.RegisterType<CommandDispatcher>()
                   .As<ICommandDispatcher>()
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assembly)
                   .AsClosedTypesOf(typeof(ICommandHandler<>))
                   .InstancePerLifetimeScope();

            var commandBus = new Mock<ICommandBus>();
            commandBus.Setup(x => x.Push(It.IsAny<ICommand>())).Callback(() => { });
            builder.Register(x => commandBus.Object).AsImplementedInterfaces();

            var directoriesSettings = new Mock<IDirectoriesSettings>();
            builder.Register(x => directoriesSettings.Object).AsImplementedInterfaces();

            var thumbnailService = new Mock<IThumbnailService>();
            thumbnailService.Setup(x => x.GetThumbnailUrl(It.IsAny<Guid>())).Returns("https://testurl.com/thumb.jpg");
            thumbnailService.Setup(x => x.GetPlaceholderThumbnailUrl()).Returns("https://placeholder-testurl.com/thumb.jpg");
            builder.Register(x => thumbnailService.Object).AsImplementedInterfaces();

            var videoBlobService = new Mock<IVideoBlobService>();
            builder.Register(x => videoBlobService.Object).AsImplementedInterfaces();

            builder.Register(x => new SHA256MessageSignerService(x.Resolve<ISecretServerKey>()))
                .As<IMessageSignerService>();

            builder.Register(x => new VideoQueries(new VideoMappings(
                x.Resolve<IThumbnailService>()),
                x.Resolve<IVideoRepository>(),
                x.Resolve<IDirectoriesSettings>(),
                x.Resolve<IVideoBlobService>()))
                .As<IVideoQueries>();

            builder.Register<VideoController>(context =>
            {
                var controller = new VideoController(
                    context.Resolve<ICommandDispatcher>(),
                    context.Resolve<IVideoQueries>(),
                    context.Resolve<IMessageSignerService>());
                controller.ControllerContext.HttpContext = new DefaultHttpContext();
                return controller;
            });

            return builder;
        }

        [SetUp]
        public void Setup()
        {            
        }

        [Test]
        public void Adding_New_Video()
        {
            var videos = new List<Video>();
            var container = GetBaseMockedContainerBuilder();

            var videoRepository = new Mock<IVideoRepository>();
            videoRepository.Setup(x => x.AddAsync(It.IsAny<Video>())).Returns((Video vid) =>
            {
                videos.Add(vid);
                return Task.FromResult(0);
            });
            container.Register(x => videoRepository.Object).AsImplementedInterfaces();

            var componentContext = container.Build();

            var signer = componentContext.Resolve<IMessageSignerService>();

            var videoController = componentContext.Resolve<VideoController>();
            videoController.HttpContext.User = new ClaimsPrincipal();
            videoController.User.AddIdentity(new System.Security.Claims.ClaimsIdentity(new List<Claim>
            {
                new Claim("http://streaming.com/claims", Claims.CanUploadVideo),
                new Claim(ClaimTypes.NameIdentifier, "testUser"),
                new Claim(ClaimTypes.Email, "testEmail@email.co")
            }, JwtBearerDefaults.AuthenticationScheme));

            videoController.UploadVideoAsync(new UploadVideoDTO
            {
                Title = "Title",
                Description = "Description",
                UploadToken = videoController.GetUploadToken().Token
            }).GetAwaiter().GetResult();

            Assert.AreEqual(1, videos.Count);
            Assert.AreEqual("Title", videos[0].Title);
            Assert.AreEqual("Description", videos[0].Description);
            Assert.AreEqual((VideoState)0, videos[0].State);
            Assert.AreEqual("testUser", videos[0].Owner.Identifier);
            Assert.AreEqual("testEmail@email.co", videos[0].Owner.Email);

            Assert.IsNull(videos[0].ProcessingInfo);
            Assert.IsNull(videos[0].VideoManifestHLS);
            Assert.IsNull(videos[0].FinishedProcessingDate);
            Assert.IsNull(videos[0].Length);
            Assert.IsTrue(DateTime.UtcNow.Subtract(videos.First().CreatedDate).TotalSeconds < 10);  // check if the correct date is setted
        }
    }
}