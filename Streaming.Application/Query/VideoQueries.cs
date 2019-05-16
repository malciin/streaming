using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Video;
using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Models;

namespace Streaming.Application.Query
{
    public class VideoQueries : IVideoQueries
    {
        private readonly Mapper mapper;
        private readonly IFilterableRepository<Video> filterableVideos;
		private readonly IVideoPartsFileService videoPartsFileService;

        public VideoQueries(Mapper mapper,
            IVideoRepository filterableVideos,
            IVideoPartsFileService videoPartsFileService)
        {
            this.mapper = mapper;
            this.filterableVideos = filterableVideos;
			this.videoPartsFileService = videoPartsFileService;
        }

        public async Task<VideoMetadataDTO> GetBasicVideoMetadataAsync(Guid videoId)
            => mapper.MapVideoMetadataDTO(await filterableVideos.GetSingleAsync(x => x.VideoId == videoId));

        public async Task<IPackage<VideoMetadataDTO>> SearchAsync(VideoSearchDTO search, Expression<Func<Video, object>> orderByDescending)
        {
	        return (await filterableVideos.GetAsync(x =>
			        x.Title.Contains(String.Join(" ", search.Keywords)) &&
			        x.State.HasFlag(VideoState.Processed), orderByDescending, skip: search.Offset, limit: search.HowMuch))
		        .Map(mapper.MapVideoMetadataDTO);
        }

        public async Task<string> GetVideoManifestAsync(Guid videoId)
		    => (await filterableVideos.GetSingleAsync(x => x.VideoId == videoId))
                .VideoManifest.GenerateManifest(ctx => videoPartsFileService.GetVideoUrl(ctx.VideoId, ctx.PartNumber));

		public async Task<Stream> GetVideoPartAsync(Guid videoId, int part)
			=> await videoPartsFileService.GetVideoAsync(videoId, part);
    }
}
