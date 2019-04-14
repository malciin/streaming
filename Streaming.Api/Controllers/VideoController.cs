using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Api.Requests.Video;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Video;
using Streaming.Application.DTO;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Video;
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
        public async Task<IActionResult> UploadVideoAsync([FromBody] UploadVideoRequest request)
        {
            await CommandDispatcher.HandleAsync(new UploadVideoCommand
            {
                UploadToken = request.UploadToken,
                Title = request.Title,
                Description = request.Description,
                User = new UserDetailsDTO
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Email = User.FindFirst(ClaimTypes.Email).Value,
                    Nickname = User.FindFirst(x => x.Type == "nickname")?.Value
                }
            });
            return NoContent();
        }

        [HttpGet("UploadToken")]
        [ClaimAuthorize(Claims.CanUploadVideo)]
        public TokenDTO GetUploadToken()
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
        public async Task<IActionResult> UploadPartAsync ([FromForm] UploadVideoPartRequest request)
        {
            await CommandDispatcher.HandleAsync(new UploadVideoPartCommand
            {
                PartMD5Hash = request.PartMD5Hash,
                UploadToken = request.UploadToken,
                PartBytes = request.PartBytes
            });
            return NoContent();
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
            return NoContent();
        }

        [HttpPut]
        [ClaimAuthorize(Claims.CanEditAnyVideo, Claims.CanEditOwnVideo)]
        public async Task<IActionResult> UpdateVideoAsync([FromBody] UpdateVideoRequest request)
        {
            await CommandDispatcher.HandleAsync(new UpdateVideoCommand
            {
                VideoId = request.VideoId,
                NewTitle = request.Title,
                NewDescription = request.Description,
                User = User
            });
            return NoContent();
        }
    }
}