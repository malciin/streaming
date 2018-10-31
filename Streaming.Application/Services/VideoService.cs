using MongoDB.Driver;
using Streaming.Domain.Command;
using Streaming.Domain.Models.DTO;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly IMongoDatabase mongoClient;
        private readonly ICommandBus commandBus;

        public VideoService(ICommandBus commandBus, IMongoDatabase mongoClient)
        {
            this.mongoClient = mongoClient;
            this.commandBus = commandBus;
        }

        public Task<bool> AddAsync(VideoUploadDTO VideoUploadDTO)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VideoBasicMetadataDTO>> GetRangeAsync(int Offset, int HowMuch)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetVideoManifestAsync(Guid VideoId)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetVideoPartAsync(Guid VideoId, int Part)
        {
            throw new NotImplementedException();
        }
    }
}
