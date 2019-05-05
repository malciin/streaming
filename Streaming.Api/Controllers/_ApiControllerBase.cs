using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streaming.Api.Extensions;
using Streaming.Application.Commands;
using Streaming.Domain.Models;

namespace Streaming.Api.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        private readonly ICommandDispatcher commandDispatcher;
        
        protected ApiControllerBase (ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public Task DispatchAsync(ICommand command)
        {
            if (command is IAuthenticatedCommand authenticatedCommand)
            {
                authenticatedCommand.User.Details = new UserDetails
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Email = User.FindFirst(ClaimTypes.Email).Value,
                    Nickname = User.FindFirst(x => x.Type == "nickname")?.Value
                };

                authenticatedCommand.User.Claims = User.GetStreamingClaims();
            }

            return commandDispatcher.HandleAsync(command);
        }
    }
}
