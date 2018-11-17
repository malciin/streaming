using AutoMapper;
using MongoDB.Driver;
using Streaming.Application.Commands;
using Streaming.Application.Configuration;
using Streaming.Domain.Command;
using Streaming.Domain.Models.Core;
using Streaming.Domain.Models.DTO.Video;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly IMongoDatabase mongoDatabase;
        private readonly IMongoCollection<Video> videoCollection;

        private readonly ICommandBus commandBus;
        private readonly IMapper mapper;
        private readonly IDirectoriesConfiguration directoriesConfig;

        public VideoService(IDirectoriesConfiguration directoriesConfig, ICommandBus commandBus, IMongoDatabase mongoDatabase, IMapper mapper)
        {
            this.mongoDatabase = mongoDatabase;
            videoCollection = mongoDatabase.GetCollection<Video>("Videos");
            this.commandBus = commandBus;
            this.mapper = mapper;
            this.directoriesConfig = directoriesConfig;
        }

        public async Task<bool> AddAsync(VideoUploadDTO videoUploadDTO)
        {
            var video = mapper.Map<Video>(videoUploadDTO);

            // Move below to ProcessVideo command
            Directory.CreateDirectory(directoriesConfig.ProcessingDirectory);
            using (var file = new FileStream($"{directoriesConfig.ProcessingDirectory}/{video.VideoId}", FileMode.CreateNew, FileAccess.Write))
            {
                await videoUploadDTO.File.CopyToAsync(file);
            }

            await videoCollection.InsertOneAsync(video);

            commandBus.Send(new ProcessVideo
            {
                RawVideoLocalPath = $"{directoriesConfig.ProcessingDirectory}/{video.VideoId}",
                VideoId = video.VideoId
            });
            return false;
        }

        public async Task<VideoBasicMetadataDTO> GetAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            return await videoCollection
                .Find<Video>(searchFilter)
                .Project(x => mapper.Map<VideoBasicMetadataDTO>(x))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<VideoBasicMetadataDTO>> GetAsync(VideoSearchDTO Search)
        {
            var results = await videoCollection.Find(_ => true).Skip(Search.Offset).Limit(Search.HowMuch).ToListAsync();
            return results.Select(x => mapper.Map<VideoBasicMetadataDTO>(x));
        }

        public async Task<string> GetVideoManifestAsync(Guid VideoId)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoId);
            return await videoCollection
                .Find<Video>(searchFilter)
                .Project(x => x.VideoManifestHLS).FirstOrDefaultAsync();
        }

        public Task<byte[]> GetVideoPartAsync(Guid VideoId, int Part)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateVideoAfterProcessingAsync(VideoProcessedDataDTO VideoProcessedData)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoProcessedData.VideoId);
            var updateDefinition = Builders<Video>.Update
                .Set(x => x.FinishedProcessingDate, VideoProcessedData.FinishedProcessingDate)
                .Set(x => x.VideoSegmentsZip, VideoProcessedData.VideoSegmentsZip)
                .Set(x => x.ProcessingInfo, VideoProcessedData.ProcessingInfo)
                .Set(x => x.VideoManifestHLS, VideoProcessedData.VideoManifestHLS)
                .Set(x => x.Length, VideoProcessedData.Length);

            var result = await videoCollection.UpdateOneAsync(searchFilter, updateDefinition);
            return result.ModifiedCount == 1;
        }
    }
}
