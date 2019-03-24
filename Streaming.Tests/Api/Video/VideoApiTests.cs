using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Streaming.Api.Attributes;
using Streaming.Api.Controllers;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Video;
using Streaming.Application.DTO;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Video;
using Streaming.Application.Query;
using Streaming.Domain.Models;
using Streaming.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Streaming.Tests
{
    class VideoApiTests
    {
        private ContainerBuilder containerBuilder;
        private Mock<ICommandDispatcher> commandDispatcherMock;
        private Mock<IVideoQueries> videoQueriesMock;

        private IContainer _container;
        private IContainer container
        {
            get
            {
                if (_container == null)
                {
                    _container = containerBuilder.Build();
                }
                return _container;
            }
        }

        [SetUp]
        public void Setup()
        {
            containerBuilder = new ContainerBuilder();
            commandDispatcherMock = new Mock<ICommandDispatcher>();
            containerBuilder.Register(x => commandDispatcherMock.Object).AsImplementedInterfaces();

            videoQueriesMock = new Mock<IVideoQueries>();
            containerBuilder.Register(x => videoQueriesMock.Object).AsImplementedInterfaces();

            containerBuilder.RegisterUnused(typeof(IMessageSignerService));

            containerBuilder.RegisterType<VideoController>();
        }

        private MethodInfo getMethodFromControllerThatTakeParameter(ControllerBase controller, Type parameter)
        {
            var uploadVideoMethod = controller.GetType().GetMethods()
                                              .Where(x => x.GetParameters().Select(y => y.ParameterType)
                                              .Contains(parameter)).First();
            return uploadVideoMethod;
        }

        private MethodInfo getMethodFromControllerThatReturnType(ControllerBase controller, Type returnedType)
        {
            var uploadVideoMethod = controller.GetType().GetMethods()
                                              .Where(x => x.ReturnType == returnedType).First();
            return uploadVideoMethod;
        }

        private void testThatMethodHaveFilterClaim(MethodInfo methodInfo, string expectedClaim)
        {
            var attribute = methodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(ClaimAuthorizeAttribute)).FirstOrDefault();
            Assert.NotNull(attribute, "Method doesn't have ClaimAuthorize attribute!");

            var claims = attribute.ConstructorArguments
                                  .Where(x => x.ArgumentType == typeof(String[]))
                                  .Select(x => x.Value).First() as ReadOnlyCollection<CustomAttributeTypedArgument>;
            var containClaim = claims.FirstOrDefault(x => x.Value as string == expectedClaim);
            Assert.NotNull(containClaim, $"ClaimAuthorize doesn't have {expectedClaim} claim!");
        }

        [Test]
        public void CanUploadVideo_Claim_On_UploadVideo_Endpoint()
        {
            var videoController = container.Resolve<VideoController>();
            var uploadVideoMethod = getMethodFromControllerThatTakeParameter(videoController, typeof(UploadVideoDTO));
            testThatMethodHaveFilterClaim(uploadVideoMethod, Claims.CanUploadVideo);
        }

        [Test]
        public void CanUploadVideo_Claim_On_GetUploadToken_Endpoint()
        {
            var videoController = container.Resolve<VideoController>();
            var uploadVideoMethod = getMethodFromControllerThatReturnType(videoController, typeof(TokenDTO));
            testThatMethodHaveFilterClaim(uploadVideoMethod, Claims.CanUploadVideo);
        }

        [Test]
        public void CanUploadVideo_Claim_On_UploadVideoPart_Endpoint()
        {
            var videoController = container.Resolve<VideoController>();
            var uploadVideoMethod = getMethodFromControllerThatTakeParameter(videoController, typeof(UploadVideoPartDTO));
            testThatMethodHaveFilterClaim(uploadVideoMethod, Claims.CanUploadVideo);
        }

        [Test]
        public void Is_Adding_New_Video_Works()
        {
            var videoController = container.Resolve<VideoController>();
            videoController.ControllerContext.HttpContext = new DefaultHttpContext();
            videoController.HttpContext.User = new ClaimsPrincipal();
            videoController.User.AddIdentity(new System.Security.Claims.ClaimsIdentity(new List<Claim>
            {
                new Claim(Claims.ClaimsNamespace, Claims.CanUploadVideo),
                new Claim(ClaimTypes.NameIdentifier, "testUser"),
                new Claim(ClaimTypes.Email, "testEmail@email.co")
            }, JwtBearerDefaults.AuthenticationScheme));

            videoController.UploadVideoAsync(new UploadVideoDTO
            {
                Title = "Title",
                Description = "Description",
                UploadToken = videoController.GetUploadToken().Token
            }).GetAwaiter().GetResult();

            commandDispatcherMock.Verify(x => x.HandleAsync(It.IsAny<UploadVideoCommand>()), Times.Once);
        }
    }
}