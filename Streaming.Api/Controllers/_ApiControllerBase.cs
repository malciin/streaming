using Microsoft.AspNetCore.Mvc;
using Streaming.Application.Commands;

namespace Streaming.Api.Controllers
{
    public class _ApiControllerBase : ControllerBase
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected _ApiControllerBase (ICommandDispatcher CommandDispatcher)
        {
            this.CommandDispatcher = CommandDispatcher;
        }
    }
}
