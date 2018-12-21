using Microsoft.AspNetCore.Mvc;
using Streaming.Application.Command;

namespace Streaming.Api.Controllers
{
    public class _ApiControllerBase : ControllerBase
    {
        protected readonly ICommandBus CommandBus;
        protected _ApiControllerBase (ICommandBus CommandBus)
        {
            this.CommandBus = CommandBus;
        }
    }
}
