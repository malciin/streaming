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
using Streaming.Application.Interfaces.Models;
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
        private readonly ITokenService tokenService;
        
        public VideoController(ICommandDispatcher commandDispatcher, IVideoQueries queries, ITokenService tokenService) : base(commandDispatcher)
        {
            this.queries = queries;
            this.tokenService = tokenService;
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
            var token = tokenService.GetUploadVideoToken(new UploadVideoTokenDataDTO
            {
                VideoId = Guid.NewGuid()
            });
            return new TokenDTO {
                Token = token
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
                PartStream = request.PartBytes.OpenReadStream()
            });
            return NoContent();
        }

        [HttpGet("{Id}")]
        public async Task<VideoMetadataDTO> GetByIdAsync (Guid id)
        {
            return await queries.GetBasicVideoMetadataAsync(id);
        }

        [HttpPost("Search")]
        public async Task<IPackage<VideoMetadataDTO>> SearchAsync ([FromBody] VideoSearchDTO search)
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