using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using System;
using Streaming.Infrastructure.IoC;

namespace Streaming.Tests.Services
{
    public class MessageSignerService
    {
        IComponentContext componentContext;
        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ServicesModule>();

            var secretServerKey = new Mock<ISecretServerKey>();
            secretServerKey.Setup(x => x.SecretServerKey).Returns("Some test secret server key");
            builder.Register(x => secretServerKey.Object).AsImplementedInterfaces();

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
        public void Wrong_Message_Sign_Part_Should_Throw_MessageWrongSignatureException()
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
            Assert.NotNull(messageSignerServiceOtherKey, "Getting null MessageSignerService with other key " +
                                                         "check that you provide the needed constructor arguments");
            Assert.Throws<MessageWrongSignatureException>(() => messageSignerServiceOtherKey.GetMessage(signed));
        }
    }
}