﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Application.Command;
using Streaming.Application.Command.Commands.Video;
using Streaming.Application.DTO;
using Streaming.Application.DTO.Video;
using Streaming.Application.Models;
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
        public async Task<IActionResult> UploadVideo([FromBody] UploadVideoDTO uploadVideo)
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
        public async Task<IActionResult> UploadPart(UploadVideoPartDTO videoPart)
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
        public async Task<VideoMetadataDTO> GetById(Guid id)
        {
            return await queries.GetBasicVideoMetadataAsync(id);
        }

        [HttpPost("Search")]
        public async Task<IEnumerable<VideoMetadataDTO>> Search([FromBody] VideoSearchDTO search)
        {
			return await queries.SearchAsync(search);
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

        [HttpPut]
        [ClaimAuthorize(Claims.CanEditAnyVideo, Claims.CanEditOwnVideo)]
        public async Task<IActionResult> ActionResult([FromBody] UpdateVideoDTO update)
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