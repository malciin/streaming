using AutoMapper;
using MongoDB.Driver;
using Streaming.Application.Commands;
using Streaming.Application.Configuration;
using Streaming.Domain.Command;
using Streaming.Domain.Models.Core;
using Streaming.Domain.Models.DTO;
using Streaming.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly IMongoDatabase mongoDatabase;
        private readonly ICommandBus commandBus;
        private readonly IMapper mapper;
        private readonly IDirectoriesConfiguration directoriesConfig;

        public VideoService(IDirectoriesConfiguration directoriesConfig, ICommandBus commandBus, IMongoDatabase mongoDatabase, IMapper mapper)
        {
            this.mongoDatabase = mongoDatabase;
            this.commandBus = commandBus;
            this.mapper = mapper;
            this.directoriesConfig = directoriesConfig;
        }

        public async Task<bool> AddAsync(VideoUploadDTO videoUploadDTO)
        {
            var video = mapper.Map<Video>(videoUploadDTO);

            Directory.CreateDirectory(directoriesConfig.ProcessingDirectory);
            using (var file = new FileStream($"{directoriesConfig.ProcessingDirectory}/{video.VideoId}", FileMode.CreateNew, FileAccess.Write))
            {
                await videoUploadDTO.File.CopyToAsync(file);
            }

            commandBus.Send(new ProcessVideo
            {
                RawVideoLocalPath = $"{directoriesConfig.ProcessingDirectory}/{video.VideoId}",
                VideoId = video.VideoId
            });
            return false;
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

        public Task<bool> UpdateBaseVideo(Guid VideoId, string Manifest)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateVideoAfterProcessed(VideoProcessedDataDTO VideoProcessedData)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoProcessedData.VideoId);
            var updateDefinition = Builders<Video>.Update
                .Set(x => x.FinishedProcessingDate, VideoProcessedData.FinishedProcessingDate)
                .Set(x => x.VideoSegmentsZip, VideoProcessedData.VideoSegmentsZip)
                .Set(x => x.ProcessingInfo, VideoProcessedData.ProcessingInfo)
                .Set(x => x.VideoManifestHLS, VideoProcessedData.VideoManifestHLS)
                .Set(x => x.Length, VideoProcessedData.Length);

            var result = await mongoDatabase.GetCollection<Video>(typeof(Video).Name).UpdateOneAsync(searchFilter, updateDefinition);
            return result.ModifiedCount == 1;
        }

        public Task<bool> UpdateVideoMetadata(VideoBasicMetadataDTO Video)
        {
            throw new NotImplementedException();
        }
    }
}
