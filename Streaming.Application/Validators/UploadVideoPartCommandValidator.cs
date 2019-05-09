using FluentValidation;
using Streaming.Application.Commands.Video;
using Streaming.Common.Extensions;

namespace Streaming.Application.Validators
{
    public class UploadVideoPartCommandValidator : AbstractValidator<UploadVideoPartCommand>
    {
        public UploadVideoPartCommandValidator()
        {
            RuleFor(x => x.PartStream)
                .NotNull();

            RuleFor(x => x.PartMD5Hash)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(y => y.IsBase64String()).WithMessage($"MD5 hash must be base64 encoded string!");
        }
    }
}
