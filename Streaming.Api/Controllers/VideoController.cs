using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Application.Command;
using Streaming.Application.Command.Commands.Video;
using Streaming.Application.Query;
using Streaming.Common.Helpers;
using Streaming.Domain.Models.DTO.Video;

namespace Streaming.Api.Controllers
{
    [Route("/Video")]
    public class VideoController : _ApiControllerBase
    {
        private readonly IVideoQueries queries;
        private VideoController(ICommandBus commandBus, IVideoQueries queries) : base(commandBus)
        {
            this.queries = queries;
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> UploadVideo(UploadVideo UploadVideo)
        {
            await CommandBus.HandleAsync(UploadVideo);
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
            return await videoService.GetAsync(Search);
        }

        [HttpGet("{Id}/{Part}")]
        public async Task<IActionResult> GetVideoPart(Guid Id, int Part)
        {
            var movieBytes = await videoService.GetVideoPartAsync(Id, Part);
            return File(movieBytes, "video/MP2T", $"{Part}.ts");
        }

        [HttpGet("Manifest/{Id}")]
        public async Task<IActionResult> GetVideoManifest(Guid Id)
        {
            var getVideoPartEndpoint = UrlHelper.GetHostUrl(HttpContext) + "/Video";
            var manifestStr = await videoService.GetVideoManifestAsync(Id);
            manifestStr = manifestStr.Replace("[ENDPOINT]", getVideoPartEndpoint).Replace("[ID]", Id.ToString());
            return File(Encoding.UTF8.GetBytes(manifestStr), "application/x-mpegURL", $"{Id}.m3u8");
        }
    }
}