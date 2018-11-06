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
        private readonly IMongoDatabase mongoClient;
        private readonly ICommandBus commandBus;
        private readonly IMapper mapper;
        private readonly IDirectoriesConfig directoriesConfig;

        public VideoService(IDirectoriesConfig directoriesConfig, ICommandBus commandBus, IMongoDatabase mongoClient, IMapper mapper)
        {
            this.mongoClient = mongoClient;
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

        public Task<bool> UpdateVideoMetadata(VideoBasicMetadataDTO Video)
        {
            throw new NotImplementedException();
        }
    }
}
