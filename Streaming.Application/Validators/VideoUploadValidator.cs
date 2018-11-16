using Streaming.Domain.Models.DTO;
using FluentValidation;
using System.Threading.Tasks;

namespace Streaming.Application.Validators
{
    public class VideoUploadValidator : AbstractValidator<VideoUploadDTO>
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
