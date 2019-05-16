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
    public static class LiveStreamRepositoryMock
    {
        public static Mock<IPastLiveStreamRepository> CreateForData(ICollection<LiveStream> data)
        {
            var mock = new Mock<IPastLiveStreamRepository>();

            mock.Setup(x => x.AddAsync(It.IsAny<LiveStream>()))
                .Returns((LiveStream liveStream) =>
                {
                    if (data.FirstOrDefault(x => x.LiveStreamId == liveStream.LiveStreamId) != null)
                        throw new ArgumentException($"LiveStream with ID: {liveStream.LiveStreamId} already exists!");
                    data.Add(liveStream);
                    return Task.FromResult(0);
                });

            mock.Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<LiveStream, bool>>>()))
                .ReturnsAsync((Expression<Func<LiveStream, bool>> filter) =>
                    data.Where(filter.Compile()).FirstOrDefault());
            
            mock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<LiveStream, bool>>>(), It.IsAny<Expression<Func<LiveStream, object>>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Expression<Func<LiveStream, bool>> expression, Expression<Func<LiveStream, object>> orderBy,  int skip, int limit) =>
                    {
                        var filteredData = data.Where(expression.Compile()).OrderByDescending(orderBy.Compile()).ToList();
                        var totalResult = filteredData.Count();
                        return Package<LiveStream>.CreatePackage(filteredData.Skip(skip).Take(limit), totalResult);
                    });

            return mock;
        }
    }
}