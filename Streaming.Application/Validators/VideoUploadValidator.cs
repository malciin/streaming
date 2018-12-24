using FluentValidation;
using Streaming.Application.Command.Commands.Video;
using System.Threading.Tasks;

namespace Streaming.Application.Validators
{
    public class VideoUploadValidator : AbstractValidator<UploadVideo>
    {
        public VideoUploadValidator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(10)
                .MaximumLength(250);

            RuleFor(x => x.Description)
                .NotNull()
                .MinimumLength(10)
                .MaximumLength(250);

            RuleFor(x => x.File).Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .MustAsync((file, cancellationToken) =>
                {
                    return Task.Run<bool>(() =>
                    {
                        return true;
                    });
                });
        }
    }
}
