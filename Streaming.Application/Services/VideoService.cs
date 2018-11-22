using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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
        private readonly IGridFSBucket bucket;
        private readonly IMongoDatabase mongoDatabase;
        private readonly IMongoCollection<Video> videoCollection;

        private readonly ICommandBus commandBus;
        private readonly IMapper mapper;
        private readonly IDirectoriesConfiguration directoriesConfig;

        public VideoService(IDirectoriesConfiguration directoriesConfig, ICommandBus commandBus, IMongoDatabase mongoDatabase, IMapper mapper, IGridFSBucket bucket)
        {
            this.mongoDatabase = mongoDatabase;
            videoCollection = mongoDatabase.GetCollection<Video>("Videos");
            this.commandBus = commandBus;
            this.mapper = mapper;
            this.directoriesConfig = directoriesConfig;
            this.bucket = bucket;
        }

        public async Task<bool> AddAsync(VideoUploadDTO videoUploadDTO)
        {
            var video = mapper.Map<Video>(videoUploadDTO);

            await videoCollection.InsertOneAsync(video);

            commandBus.Send(new ProcessVideo
            {
                VideoId = video.VideoId,
                Video = videoUploadDTO.File
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

            var results = videoCollection.Find<Video>(searchFilter).ToList();
            var all = videoCollection.Find<Video>(_ => true).ToList();
            return await videoCollection
                .Find<Video>(searchFilter)
                .Project(x => x.VideoManifestHLS).FirstOrDefaultAsync();
        }

        private string GetFileNameByPart(int Part)
        {
            var result = Part.ToString();
            return new string('0', 3 - result.Length) + result + ".ts";
        }

        public async Task<byte[]> GetVideoPartAsync(Guid VideoId, int Part)
        {
            try
            {
                var directory = new DirectoryInfo(String.Format($"{directoriesConfig.ProcessedDirectory}{{0}}{VideoId}{{0}}", Path.DirectorySeparatorChar));
                var file = directory.GetFiles().Where(x => x.Name == GetFileNameByPart(Part)).First();
                var memory = new MemoryStream();
                await file.OpenRead().CopyToAsync(memory);
                return memory.ToArray();
            }
            catch (FileNotFoundException)
            {

            }
            catch (DirectoryNotFoundException)
            {

            }
            return null;
        }

        public async Task<bool> UpdateVideoAfterProcessingAsync(VideoProcessedDataDTO VideoProcessedData)
        {
            var searchFilter = Builders<Video>.Filter.Eq(x => x.VideoId, VideoProcessedData.VideoId);

            await bucket.UploadFromStreamAsync(VideoProcessedData.VideoId.ToString(), new MemoryStream(VideoProcessedData.VideoSegmentsZip));

            var updateDefinition = Builders<Video>.Update
                .Set(x => x.FinishedProcessingDate, VideoProcessedData.FinishedProcessingDate)
                .Set(x => x.ProcessingInfo, VideoProcessedData.ProcessingInfo)
                .Set(x => x.VideoManifestHLS, VideoProcessedData.VideoManifestHLS)
                .Set(x => x.Length, VideoProcessedData.Length);

            var result = await videoCollection.UpdateOneAsync(searchFilter, updateDefinition);
            return result.ModifiedCount == 1;
        }
    }
}
