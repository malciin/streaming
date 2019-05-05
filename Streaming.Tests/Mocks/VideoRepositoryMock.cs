using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models.Repository.Video;
using Streaming.Domain.Models;

namespace Streaming.Tests.Mocks
{
    public static class VideoRepositoryMock
    {
        public static Mock<IVideoRepository> CreateForData(ICollection<Video> data)
        {
            var mock = new Mock<IVideoRepository>();
            
            mock.Setup(x => x.AddAsync(It.IsAny<Video>())).Returns((Video vid) =>
            {
                data.Add(vid);
                return Task.FromResult(0);
            });
            mock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).Returns((Guid id) =>
            {
                data = data.Where(x => x.VideoId != id).ToList();
                return Task.FromResult(0);
            });

            mock.Setup(x => x.UpdateAsync(It.IsAny<UpdateVideoInfo>())).Returns((UpdateVideoInfo vid) =>
            {
                var video = data.FirstOrDefault(x => x.VideoId == vid.UpdateByVideoId && vid.UpdateByUserIdentifier == x.Owner.UserId);
                if (video != null)
                {
                    video.Title = vid.NewVideoTitle;
                    video.Description = vid.NewVideoDescription;
                }
                return Task.FromResult(0);
            });

            return mock;
        }
    }
}