using Autofac;
using FluentValidation;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ILifetimeScope lifetimeScope;
        public CommandDispatcher(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public async Task HandleAsync<T>(T command) where T : ICommand
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                // If there is registered a validator for current command we perform validation
                scope.TryResolve(out IValidator<T> validator);
                if (validator != null)
                {
                    await validator.ValidateAndThrowAsync(command);
                }
                var handler = scope.Resolve<ICommandHandler<T>>();
                await handler.HandleAsync(command);
            }
        }
    }
}
