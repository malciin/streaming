using System.Threading.Tasks;
using Moq;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;

namespace Streaming.Tests.Mocks
{
    public static class VideoFileInfoServiceMock
    {
        public static Mock<IVideoFileInfoService> CreateForData(VideoFileDetailsDTO returnedDetails)
        {
            var mock = new Mock<IVideoFileInfoService>();
            mock.Setup(x => x.GetDetailsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(returnedDetails));
            return mock;
        }
    }
}