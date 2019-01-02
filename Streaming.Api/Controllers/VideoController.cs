using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Application.Command;
using Streaming.Application.Command.Commands.Video;
using Streaming.Application.DTO.Video;
using Streaming.Application.Query;

namespace Streaming.Api.Controllers
{
    [Route("/Video")]
    public class VideoController : _ApiControllerBase
    {
        private readonly IVideoQueries queries;
        public VideoController(ICommandDispatcher commandDispatcher, IVideoQueries queries) : base(commandDispatcher)
        {
            this.queries = queries;
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> UploadVideo(UploadVideo UploadVideo)
        {
            await CommandDispatcher.HandleAsync(UploadVideo);
            return Ok();
        }

        [HttpGet("{Id}")]
        public async Task<VideoBasicMetadataDTO> GetById(Guid id)
        {
            return await queries.GetBasicVideoMetadataAsync(id);
        }

        [HttpPost("Search")]
        public async Task<IEnumerable<VideoBasicMetadataDTO>> Search(VideoSearchDTO Search)
        {
			return await queries.SearchAsync(Search);
        }

        [HttpGet("{Id}/{Part}")]
        public async Task<IActionResult> GetVideoPart(Guid Id, int Part)
        {
			using (var movieStream = await queries.GetVideoPartAsync(Id, Part))
			{
				return File(movieStream, "video/MP2T", $"{Part}.ts");
			}
        }

        [HttpGet("Manifest/{Id}")]
        public async Task<IActionResult> GetVideoManifest(Guid Id)
        {
			var manifest = await queries.GetVideoManifestAsync(Id);
			return File(Encoding.UTF8.GetBytes(manifest), "application/x-mpegURL", $"{Id}.m3u8");
		}
    }
}