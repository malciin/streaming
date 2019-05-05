using System;
using System.Collections.Generic;
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
    public class VideoController : ApiControllerBase
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
            await DispatchAsync(new UploadVideoCommand
            {
                UploadToken = request.UploadToken,
                Title = request.Title,
                Description = request.Description
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
            await DispatchAsync(new UploadVideoPartCommand
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
        public async Task<IActionResult> GetVideoPartAsync (Guid id, int part)
        {
            return File(await queries.GetVideoPartAsync(id, part), "video/MP2T", $"{part}.ts");
        }

        [HttpGet("Manifest/{Id}")]
        public async Task<IActionResult> GetVideoManifestAsync (Guid id)
        {
			var manifest = await queries.GetVideoManifestAsync(id);
			return File(Encoding.UTF8.GetBytes(manifest), "application/x-mpegURL", $"{id}.m3u8");
		}

        [HttpDelete("{Id}")]
        [ClaimAuthorize(Claims.CanDeleteVideo)]
        public async Task<IActionResult> DeleteVideoAsync (Guid id)
        {
            await DispatchAsync(new DeleteVideoCommand
            {
                VideoId = id
            });
            return NoContent();
        }

        [HttpPut]
        [ClaimAuthorize(Claims.CanEditAnyVideo, Claims.CanEditOwnVideo)]
        public async Task<IActionResult> UpdateVideoAsync([FromBody] UpdateVideoRequest request)
        {
            await DispatchAsync(new UpdateVideoCommand
            {
                VideoId = request.VideoId,
                NewTitle = request.Title,
                NewDescription = request.Description,
            });
            return NoContent();
        }
    }
}