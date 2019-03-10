using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Video;
using Streaming.Application.DTO;
using Streaming.Application.DTO.Video;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Application.Query;

namespace Streaming.Api.Controllers
{
    [Route("/Video")]
    [ApiController]
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
        public async Task<IActionResult> UploadVideoAsync([FromBody] UploadVideoDTO uploadVideo)
        {
            await CommandDispatcher.HandleAsync(new UploadVideoCommand
            {
                UploadToken = uploadVideo.UploadToken,
                Title = uploadVideo.Title,
                Description = uploadVideo.Description,
                User = HttpContext.User
            });
            return Ok();
        }

        [HttpGet("UploadToken")]
        [ClaimAuthorize(Claims.CanUploadVideo)]
        public TokenDTO GetUploadTokenAsync()
        {
            // Too tired when I wrote this, to move this to dedicated serivice or something...
            var signedMessage = messageSigner.SignMessage(Guid.NewGuid().ToByteArray());
            return new TokenDTO
            {
                Token = Convert.ToBase64String(signedMessage)
            };
        }

        [HttpPost("UploadPart")]
        [ClaimAuthorize(Claims.CanUploadVideo)]
        public async Task<IActionResult> UploadPartAsync ([FromForm] UploadVideoPartDTO videoPart)
        {
            await CommandDispatcher.HandleAsync(new UploadVideoPartCommand
            {
                PartMD5Hash = videoPart.PartMD5Hash,
                UploadToken = videoPart.UploadToken,
                PartBytes = videoPart.PartBytes
            });
            return Ok();
        }

        [HttpGet("{Id}")]
        public async Task<VideoMetadataDTO> GetByIdAsync (Guid id)
        {
            return await queries.GetBasicVideoMetadataAsync(id);
        }

        [HttpPost("Search")]
        public async Task<IEnumerable<VideoMetadataDTO>> SearchAsync ([FromBody] VideoSearchDTO search)
        {
			return await queries.SearchAsync(search);
        }

        [HttpGet("{Id}/{Part}")]
        public async Task<IActionResult> GetVideoPartAsync (Guid Id, int Part)
        {
            return File(await queries.GetVideoPartAsync(Id, Part), "video/MP2T", $"{Part}.ts");
        }

        [HttpGet("Manifest/{Id}")]
        public async Task<IActionResult> GetVideoManifestAsync (Guid Id)
        {
			var manifest = await queries.GetVideoManifestAsync(Id);
			return File(Encoding.UTF8.GetBytes(manifest), "application/x-mpegURL", $"{Id}.m3u8");
		}

        [HttpDelete("{Id}")]
        [ClaimAuthorize(Claims.CanDeleteVideo)]
        public async Task<IActionResult> DeleteVideoAsync (Guid Id)
        {
            await CommandDispatcher.HandleAsync(new DeleteVideoCommand
            {
                VideoId = Id
            });
            return Ok();
        }

        [HttpPut]
        [ClaimAuthorize(Claims.CanEditAnyVideo, Claims.CanEditOwnVideo)]
        public async Task<IActionResult> UpdateVideoAsync([FromBody] UpdateVideoDTO update)
        {
            await CommandDispatcher.HandleAsync(new UpdateVideoCommand
            {
                VideoId = update.VideoId,
                NewTitle = update.Title,
                NewDescription = update.Description,
                User = User
            });
            return Ok();
        }
    }
}