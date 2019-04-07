using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Streaming.Api.Attributes;
using Streaming.Application.Commands;
using Streaming.Application.Commands.Live;
using Streaming.Application.DTO;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Application.Query;
using Streaming.Common.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Api.Controllers
{
    [ApiController, Route("/Live")]
    public class LiveController : _ApiControllerBase
    {
        private readonly IMessageSignerService messageSigner;
        private readonly ILiveQueries queries;

        public LiveController(ICommandDispatcher CommandDispatcher,
                                       IMessageSignerService messageSigner,
                                       ILiveQueries liveQueries) : base(CommandDispatcher)
        {
            this.messageSigner = messageSigner;
            this.queries = liveQueries;
        }

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
        public async Task<IActionResult> OnConnect()
        {
            var request = Request;
            JObject json;
            using (var str = new MemoryStream())
            {
                Request.Body.CopyTo(str);
                json = JObject.Parse(Encoding.UTF8.GetString(str.ToArray()));
            }

            var streamId = Guid.NewGuid();
            await CommandDispatcher.HandleAsync(new ConnectLiveServerCommand
            {
                StreamId = streamId,
                App = json["app"].ToString(),
                ClientKey = json["stream_key"].ToString()
            });

            return Ok(streamId.ToString());
        }

        [HttpPost("OnPublish")]
        public async Task<IActionResult> OnPublish()
        {
            var request = Request;
            JObject json;
            using (var str = new MemoryStream())
            {
                Request.Body.CopyTo(str);
                json = JObject.Parse(Encoding.UTF8.GetString(str.ToArray()));
            }

            await CommandDispatcher.HandleAsync(new StartLiveCommand
            {
                StreamId = Guid.Parse(json["stream"].ToString()),
                ManifestUrl = $"{json["tcUrl"].ToString()}{json["stream"].ToString()}.m3u8".Replace("rtmp", "http")
            });

            return Ok(0);
        }

        [HttpPost("OnUnpublish")]
        public async Task<IActionResult> OnUnpublish()
        {
            var request = Request;
            JObject json;
            using (var str = new MemoryStream())
            {
                Request.Body.CopyTo(str);
                json = JObject.Parse(Encoding.UTF8.GetString(str.ToArray()));
            }

            await CommandDispatcher.HandleAsync(new FinishLiveCommand
            {
                StreamId = Guid.Parse(json["stream"].ToString())
            });

            return Ok(0);
        }
    }
}
