using System;
using Moq;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;

namespace Streaming.Tests.Mocks
{
    public class TokenServiceMock
    {
        public static Mock<ITokenService> CreateForData(UploadVideoTokenDataDTO uploadTokenData)
        {
            var mock = new Mock<ITokenService>();

            mock.Setup(x => x.GetUploadVideoToken(It.IsAny<UploadVideoTokenDataDTO>()))
                .Returns(Convert.ToBase64String(new byte[] {0x00}));

            mock.Setup(x => x.GetDataFromUploadVideoToken(It.IsAny<string>()))
                .Returns(uploadTokenData);

            return mock;
        }
    }
}