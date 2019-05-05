using System;
using Autofac;
using Moq;
using NUnit.Framework;
using Streaming.Application.Exceptions;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using Streaming.Infrastructure.IoC;

namespace Streaming.Tests.Services
{
    public class MessageSignerServiceTests
    {
        private IMessageSignerService messageSignerService;
        
        [SetUp]
        protected void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ServicesModule>();

            var secretServerKey = new Mock<ISecretServerKey>();
            secretServerKey.Setup(x => x.SecretServerKey).Returns("Some test secret server key");
            builder.Register(x => secretServerKey.Object).AsImplementedInterfaces();

            messageSignerService = builder.Build().Resolve<IMessageSignerService>();
        }

        [TestCase(new byte[] {}, TestName = "Signing message should work also on empty byte array")]
        [TestCase(new byte[] { 0x16, 0x12, 0x18 })]
        public void Is_Signed_Message_Works(byte[] messageToSign)
        {
            var signed = messageSignerService.SignMessage(messageToSign);
            var returnedMessage = messageSignerService.GetMessage(signed);
            for(int i = 0; i<messageToSign.Length; i++)
            {
                Assert.AreEqual(messageToSign[i], returnedMessage[i]);
            }
        }

        [Test]
        public void When_SignedMessage_Is_Shorter_Than_224Bits_MessageSigner_Should_Throw_ArgumentException()
        {
            // Message have only 24 bits, but the shortest safe cryptographic algorithms produce 224 bits long hashes
            // so it can't be secure signed message
            var messageToDecrypt = new byte[]
            {
                0x16,
                0x12,
                0x18
            };

            Assert.Throws<ArgumentException>(() => messageSignerService.GetMessage(messageToDecrypt));
        }

        [Test]
        public void Wrong_Message_Sign_Part_Should_Throw_MessageWrongSignatureException()
        {
            var msgToSign = new byte[]
            {
                0x16,
                0x12,
                0x18
            };
            var signed = messageSignerService.SignMessage(msgToSign);

            var mockedSecretServerKey = new Mock<ISecretServerKey>();
            mockedSecretServerKey.Setup(x => x.SecretServerKey).Returns("otherKey");
            var messageSignerServiceOtherKey = Activator.CreateInstance(messageSignerService.GetType(), mockedSecretServerKey.Object) as IMessageSignerService;
            Assert.NotNull(messageSignerServiceOtherKey, "Getting null MessageSignerService with other key " +
                                                         "check that you provide the needed constructor arguments");
            Assert.Throws<MessageWrongSignatureException>(() => messageSignerServiceOtherKey.GetMessage(signed));
        }
    }
}