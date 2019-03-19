using FluentValidation;
using Streaming.Application.Models.DTO.Video;

namespace Streaming.Application.Validators
{
    public class UploadVideoDTOValidator : AbstractValidator<UploadVideoDTO>
    {
        public UploadVideoDTOValidator()
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
