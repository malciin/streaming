using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Live;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Models;
using Streaming.Domain.Models;

namespace Streaming.Application.Interfaces.Services
{
    public interface ILiveStreamManager
    {
        /// <summary>
        /// Gets single current online live stream
        /// </summary>
        LiveStreamMetadataDTO GetSingle(Guid liveStreamId);

        /// <summary>
        /// Gets multiple current online live streams
        /// </summary>
        IEnumerable<LiveStreamMetadataDTO> Get(EnumerableFilter<LiveStream> filter);

        /// <summary>
        /// Get single past live stream
        /// </summary>
        Task<PastLiveStreamMetadataDTO> GetPastSingleAsync(Guid id);
        
        /// <summary>
        /// Gets multiple past live streams
        /// </summary>
        Task<IPackage<PastLiveStreamMetadataDTO>> GetPastAsync(Expression<Func<LiveStream, bool>> filter, Expression<Func<LiveStream, object>> orderBy, int skip, int limit);
        
        Task StartNewLiveStreamAsync(NewLiveStreamDTO newLiveStream);
        Task FinishLiveStreamAsync(Guid liveStreamId);
    }
}
