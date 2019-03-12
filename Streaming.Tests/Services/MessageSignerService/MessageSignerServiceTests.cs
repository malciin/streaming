using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Infrastructure.IoC.Extensions;
using System;

namespace Streaming.Tests
{
    public class MessageSignerService
    {
        IComponentContext componentContext;
        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.UseDefaultModules();
            builder.Register(x => new Mock<Microsoft.Extensions.Configuration.IConfigurationRoot>().Object).AsImplementedInterfaces();
            componentContext = builder.Build();
        }

        [Test]
        public void Is_Signed_Message_Works()
        {
            var messageSingerService = componentContext.Resolve<IMessageSignerService>();
            var msgToSign = new byte[]
            {
                0x16,
                0x12,
                0x18
            };
            var signed = messageSingerService.SignMessage(msgToSign);
            var returnedMessage = messageSingerService.GetMessage(signed);
            for(int i = 0; i<msgToSign.Length; i++)
            {
                Assert.AreEqual(msgToSign[i], returnedMessage[i]);
            }
        }

        [Test]
        public void Wrong_Message_Sign_Part_Should_Throw_()
        {
            var messageSingerService = componentContext.Resolve<IMessageSignerService>();
            var msgToSign = new byte[]
            {
                0x16,
                0x12,
                0x18
            };
            var signed = messageSingerService.SignMessage(msgToSign);

            var mockedSecretServerKey = new Mock<ISecretServerKey>();
            mockedSecretServerKey.Setup(x => x.SecretServerKey).Returns("otherKey");
            var messageSignerServiceOtherKey = Activator.CreateInstance(messageSingerService.GetType(), mockedSecretServerKey.Object) as IMessageSignerService;
            Assert.Throws<MessageWrongSignatureException>(() => messageSignerServiceOtherKey.GetMessage(signed));
        }
    }
}