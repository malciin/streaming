using Autofac;
using FluentValidation;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext componentContext;
        public CommandDispatcher(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public async Task HandleAsync<T>(T command) where T : ICommand
        {
            // If there is registered a validator for current command we perform validation
            componentContext.TryResolve<IValidator<T>>(out IValidator<T> validator);
            if (validator != null)
            {
                await validator.ValidateAndThrowAsync(command);
            }
            var handler = componentContext.Resolve<ICommandHandler<T>>();
            await handler.HandleAsync(command);
        }
    }
}
