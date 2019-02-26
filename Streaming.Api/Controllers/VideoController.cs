using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Api.Models;
using Streaming.Application.Command;
using Streaming.Application.Command.Commands.Video;
using Streaming.Application.DTO.Video;
using Streaming.Application.Query;
using Streaming.Application.Services;

namespace Streaming.Api.Controllers
{
    [Route("/Video")]
    public class VideoController : _ApiControllerBase
    {
        private readonly IVideoQueries queries;
        private readonly IMessageSignerService messageSigner;
        public VideoController(ICommandDispatcher commandDispatcher, IVideoQueries queries, IMessageSignerService messageSigner) : base(commandDispatcher)
        {
            this.queries = queries;
            this.messageSigner = messageSigner;
        }

        [HttpPost]
        [ClaimAuthorize(Claims.CanUploadVideo)]
        public async Task<IActionResult> UploadVideo([FromBody] UploadVideoDTO UploadVideo)
        {
            await CommandDispatcher.HandleAsync(new UploadVideoCommand
            {
                UploadToken = UploadVideo.UploadToken,
                Title = UploadVideo.Title,
                Description = UploadVideo.Description,
                User = HttpContext.User
            });
            return Ok();
        }

        [HttpGet("UploadToken")]
        //[ClaimAuthorize(Claims.CanUploadVideo)]
        public string GetUploadToken()
        {
            // Too tired when I wrote this, to move this to dedicated serivice or something...
            var signedMessage = messageSigner.SignMessage(Guid.NewGuid().ToByteArray());
            return Convert.ToBase64String(signedMessage);
        }

        [HttpPost("UploadPart")]
        [ClaimAuthorize(Claims.CanUploadVideo)]
        public async Task<IActionResult> UploadPart(UploadVideoPartDTO VideoPart)
        {
            await CommandDispatcher.HandleAsync(new UploadVideoPartCommand
            {
                PartMD5Hash = VideoPart.PartMD5Hash,
                UploadToken = VideoPart.UploadToken,
                PartBytes = VideoPart.PartBytes
            });
            return Ok();
        }

        [HttpGet("{Id}")]
        public async Task<VideoMetadataDTO> GetById(Guid id)
        {
            return await queries.GetBasicVideoMetadataAsync(id);
        }

        [HttpPost("Search")]
        public async Task<IEnumerable<VideoMetadataDTO>> Search([FromBody] VideoSearchDTO Search)
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

        [HttpDelete("{Id}")]
        [ClaimAuthorize(Claims.CanDeleteVideo)]
        public async Task<IActionResult> DeleteVideo(Guid Id)
        {
            await CommandDispatcher.HandleAsync(new DeleteVideoCommand
            {
                VideoId = Id
            });
            return Ok();
        }
    }
}