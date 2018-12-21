using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using Streaming.Domain.Models.Core;
using Streaming.Domain.Models.DTO.Video;

namespace Streaming.Application.Query
{
    public class VideoQueries : IVideoQueries
    {
        private readonly IMapper mapper;
        private readonly IMongoCollection<Video> collection;
        public VideoQueries(IMapper mapper, IMongoCollection<Video> collection)
        {
            this.mapper = mapper;
            this.collection = collection;
        }

        public async Task<VideoBasicMetadataDTO> GetBasicVideoMetadataAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            return await collection.Find<Video>(searchFilter)
                .Project(x => mapper.Map<VideoBasicMetadataDTO>(x))
                .FirstOrDefaultAsync();
        }
    }
}
