using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;

namespace Streaming.Tests.Mocks
{
    public static class LiveStreamRepositoryMock
    {
        public static Mock<ILiveStreamRepository> CreateForData(ICollection<LiveStream> data)
        {
            var mock = new Mock<ILiveStreamRepository>();

            mock.Setup(x => x.AddAsync(It.IsAny<LiveStream>()))
                .Returns((LiveStream liveStream) =>
                {
                    if (data.FirstOrDefault(x => x.LiveStreamId == liveStream.LiveStreamId) != null)
                        throw new ArgumentException($"LiveStream with ID: {liveStream.LiveStreamId} already exists!");
                    data.Add(liveStream);
                    return Task.FromResult(0);
                });

            mock.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<LiveStream, bool>>>()))
                .ReturnsAsync((Expression<Func<LiveStream, bool>> filter) =>
                    data.Where(filter.Compile()).FirstOrDefault());
            
            mock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<LiveStream, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Expression<Func<LiveStream, bool>> expression, int skip, int limit) => 
                    data.Where(expression.Compile()).Skip(skip).Take(limit));

            return mock;
        }
    }
}