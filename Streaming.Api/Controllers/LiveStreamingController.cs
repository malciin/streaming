using Microsoft.AspNetCore.Mvc;
using Streaming.Application.DTO;
using System.IO;
using System.Text;

namespace Streaming.Api.Controllers
{
    [ApiController, Route("/Live")]
    public class LiveStreamingController : ControllerBase
    {
        [HttpGet("Token")]
        public TokenDTO GetStreamToken()
        {
            return new TokenDTO
            {

            };
        }

        [HttpPost("Publish")]
        public IActionResult Publish()
        {
            var request = HttpContext.Request;
            var formData = HttpContext.Request.Form;
            var asd = HttpContext.Request.Query;

            string x = "";
            using (var memStream = new MemoryStream())
            {
                HttpContext.Request.Body.CopyTo(memStream);
                x = Encoding.UTF8.GetString(memStream.ToArray());
            }
            return Ok("Ok");
        }

        [HttpPost("UploadStreamPart")]
        public IActionResult UploadStreamPart()
        {
            return Ok();
        }

        [HttpPost("StopPublish")]
        public IActionResult StopPublish()
        {
            return Ok();
        }
    }
}
