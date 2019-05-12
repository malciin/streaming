using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Streaming.Application.Interfaces.Repositories;
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

            mock.Setup(x => x.UpdateAsync(It.IsAny<Video>())).Returns((Video vid) =>
            {
                var video = data.FirstOrDefault(x => x.VideoId != vid.VideoId);
                
                if (video != null)
                {
                    data.Remove(video);
                    data.Add(video);
                }
                return Task.FromResult(0);
            });

            return mock;
        }
    }
}