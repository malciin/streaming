using Microsoft.AspNetCore.Mvc;
using Streaming.Application.Command;

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
