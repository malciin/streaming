using System;
using Moq;
using Streaming.Application.Interfaces.Services;

namespace Streaming.Tests.Mocks
{
    public static class MessageSignerServiceMock
    {
        public static Mock<IMessageSignerService> CreateForData(Guid returnedGuidMessage)
        {
            var mock = new Mock<IMessageSignerService>();
            mock.Setup(x => x.SignMessage(It.IsAny<byte[]>())).Returns(returnedGuidMessage.ToByteArray());
            mock.Setup(x => x.GetMessage(It.IsAny<byte[]>())).Returns(returnedGuidMessage.ToByteArray());
            return mock;
        }
        
        public static Mock<IMessageSignerService> CreateForRandomGuid()
        {
            var mock = new Mock<IMessageSignerService>();
            mock.Setup(x => x.SignMessage(It.IsAny<byte[]>())).Returns(Guid.NewGuid().ToByteArray());
            mock.Setup(x => x.GetMessage(It.IsAny<byte[]>())).Returns(Guid.NewGuid().ToByteArray());
            return mock;
        }
    }
}