using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Domain.Models.DTO;
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

        [HttpGet("Get")]
        public async Task<IActionResult> Get(Guid Id, int Part)
        {
            var movieBytes = await videoService.GetVideoPartAsync(Id, Part);
            return File(movieBytes, "video/MP2T");
        }
    }
}