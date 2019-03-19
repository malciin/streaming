using FluentValidation;
using Streaming.Application.Models.DTO.Video;
using Streaming.Common.Extensions;

namespace Streaming.Application.Validators
{
    public class UploadVideoPartDTOValidator : AbstractValidator<UploadVideoPartDTO>
    {
        public UploadVideoPartDTOValidator()
        {
            RuleFor(x => x.PartBytes)
                .NotNull();

            RuleFor(x => x.PartMD5Hash)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(y => y.IsBase64String()).WithMessage($"MD5 hash must be base64 encoded string!");

            RuleFor(x => x.PartBytes)
                .NotNull();
        }
    }
}
