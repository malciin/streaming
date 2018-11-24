using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Common.Helpers;
using Streaming.Domain.Models.DTO;
using Streaming.Domain.Models.DTO.Video;
using Streaming.Domain.Services;

namespace Streaming.Api.Controllers
{
    [Route("/Video"), ApiController]
    public class VideoController : ControllerBase
    {
        private IVideoService videoService;

        public VideoController(IVideoService videoService)
        {
            this.videoService = videoService;
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadDTO Video)
        {
            await videoService.AddAsync(Video);
            return Ok();
        }

        [HttpGet("{Id}")]
        public async Task<VideoBasicMetadataDTO> GetById(Guid id)
        {
            return await videoService.GetAsync(id);
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