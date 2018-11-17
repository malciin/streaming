using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IEnumerable<VideoBasicMetadataDTO> GetVideo(VideoSearchDTO Search)
        {
            throw new NotImplementedException();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetVideoPart(Guid Id, int Part)
        {
            var movieBytes = await videoService.GetVideoPartAsync(Id, Part);
            return File(movieBytes, "video/MP2T");
        }

        [HttpGet("Manifest")]
        public async Task<IActionResult> GetVideoManifest(Guid Id)
        {
            var manifestStr = await videoService.GetVideoManifestAsync(Id);
            return File(Encoding.UTF8.GetBytes(manifestStr), "application/x-mpegURL", $"{Id}.m3u8");
        }
    }
}