using FluentValidation;
using Streaming.Application.Commands.Video;

namespace Streaming.Application.Validators
{
    public class UploadVideoCommandValidator : AbstractValidator<UploadVideoCommand>
    {
        public UploadVideoCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .MinimumLength(10)
                .MaximumLength(250);

            RuleFor(x => x.Description)
                .NotNull()
                .MinimumLength(10)
                .MaximumLength(250);

            RuleFor(x => x.UploadToken)
                .NotNull();
        }
    }
}
