using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Models;
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

            mock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<Video, bool>>>()))
                .ReturnsAsync((Expression<Func<Video, bool>> expression) => data.Where(expression.Compile()).First());
            
            mock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Video, bool>>>(), It.IsAny<Expression<Func<Video, object>>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Expression<Func<Video, bool>> filter, Expression<Func<Video, object>> orderBy, int skip, int limit) =>
                    {
                        var total = data.Where(filter.Compile()).OrderByDescending(orderBy.Compile());
                        return Package<Video>.CreatePackage(total.Skip(skip).Take(limit), total.Count());
                    });

            return mock;
        }
    }
}