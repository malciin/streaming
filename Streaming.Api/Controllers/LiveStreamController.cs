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

            await CommandDispatcher.HandleAsync(new StartLiveCommand
            {
                ClientKey = json["app"].ToString()
            });

            return Ok("0");
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> OnFile()
        {
            var form = Request.Form;
            var file = form.Files.First();
            using (var tsStream = file.OpenReadStream())
            {
                await CommandDispatcher.HandleAsync(new UploadTsPartCommand
                {
                    FileManifestDetails = form["file_info"].ToString(),
                    FileName = file.FileName,
                    StreamKey = file.FileName.Split('.').First(),
                    Part = tsStream
                });
            }
            return Ok();
        }

        [HttpGet("{Id}/{Part}")]
        public IActionResult GetVideoPartAsync(Guid Id, int Part)
        {
            return File(queries.GetVideoPart(Id, Part), "video/MP2T", $"{Part}.ts");
        }
        
        [HttpGet("Manifest/{Id}")]
        public IActionResult GetVideoManifestAsync(Guid Id)
        {
            var manifest = queries.GetManifest(Id);
            Response.Headers.Add("Cache-Control", "no-cache, no-store");
            Response.Headers.Add("Expires", "-1");

            return File(Encoding.UTF8.GetBytes(manifest), "application/x-mpegURL", $"{Id}.m3u8");
        }
    }
}
