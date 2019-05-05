using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Attributes;
using Streaming.Api.Requests.Live;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Live;
using Streaming.Application.DTO;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Application.Models.DTO.Live;
using Streaming.Application.Query;
using Streaming.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Api.Controllers
{
    [ApiController, Route("/Live")]
    public class LiveStreamController : _ApiControllerBase
    {
        private readonly IMessageSignerService messageSigner;
        private readonly ILiveQueries queries;

        public LiveStreamController(ICommandDispatcher CommandDispatcher,
                                       IMessageSignerService messageSigner,
                                       ILiveQueries liveQueries) : base(CommandDispatcher)
        {
            this.messageSigner = messageSigner;
            this.queries = liveQueries;
        }

        [HttpGet("{Id}")]
        public LiveStreamMetadataDTO Get(Guid Id)
            => queries.Get(Id);

        [HttpPost]
        public IEnumerable<LiveStreamMetadataDTO> Search([FromBody] SearchLiveStreamsRequest request)
            => queries.Get(request.Offset, request.HowMuch);
               
        [Authorize, ClaimAuthorize(Claims.CanStream)]
        [HttpGet("Token")]
        public TokenDTO GetStreamToken()
        {
            var signedUserId = messageSigner.SignMessage(Encoding.UTF8.GetBytes(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return new TokenDTO
            {
                Token = signedUserId.ToBase32String()
            };
        }

        [HttpPost("OnConnect")]
        public async Task<IActionResult> OnConnect([FromBody] OnConnectRequest request)
        {
            var streamId = Guid.NewGuid();
            await CommandDispatcher.HandleAsync(new StartLiveStreamCommand
            {
                StreamId = streamId,
                App = request.App.ToString(),
                StreamKey = request.StreamKey,
                ManifestUri = new Uri(request.HttpUrl, $"{streamId}.m3u8")
            });

            return Ok(streamId.ToString());
        }

        [HttpPost("OnUnpublish")]
        public async Task<IActionResult> OnUnpublish([FromBody] OnUnpublishRequest request)
        {
            await CommandDispatcher.HandleAsync(new FinishLiveStreamCommand
            {
                StreamId = request.StreamId
            });

            return Ok(0);
        }
    }
}
