using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streaming.Application.Commands;

namespace Streaming.Api.Controllers
{
    public class UserController : _ApiControllerBase
    {
        public UserController(ICommandDispatcher CommandDispatcher) : base(CommandDispatcher)
        {
        }

        [HttpGet("/")]
        [Authorize]
        public string TestIsAuthorized()
        {
            var headers = Request.Headers["Authorization"];
            if (User.Identity.IsAuthenticated)
                return "You are authenticated!";
            else
                return "You are NOT authetnicated!";
        }
    }
}
