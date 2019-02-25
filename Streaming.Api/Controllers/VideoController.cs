using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadVideo(UploadVideoDTO UploadVideo)
        {
            await CommandDispatcher.HandleAsync(new UploadVideoCommand
            {
                Title = UploadVideo.Title,
                Description = UploadVideo.Description,
                File = UploadVideo.File,
                User = HttpContext.User
            });
            return Ok();
        }

        [HttpGet("{Id}")]
        public async Task<VideoMetadataDTO> GetById(Guid id)
        {
            return await queries.GetBasicVideoMetadataAsync(id);
        }

        [HttpPost("Search")]
        public async Task<IEnumerable<VideoMetadataDTO>> Search(VideoSearchDTO Search)
        {
			return await queries.SearchAsync(Search);
        }

        [HttpGet("{Id}/{Part}")]
        public async Task<IActionResult> GetVideoPart(Guid Id, int Part)
        {
            return File(await queries.GetVideoPartAsync(Id, Part), "video/MP2T", $"{Part}.ts");
        }

        [HttpGet("Manifest/{Id}")]
        public async Task<IActionResult> GetVideoManifest(Guid Id)
        {
			var manifest = await queries.GetVideoManifestAsync(Id);
			return File(Encoding.UTF8.GetBytes(manifest), "application/x-mpegURL", $"{Id}.m3u8");
		}

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteVideo(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}