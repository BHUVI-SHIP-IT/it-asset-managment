using FluentValidation;

namespace Tracer.Application.Features.Assets.Commands.DeleteAsset;

public sealed class DeleteAssetCommandValidator : AbstractValidator<DeleteAssetCommand>
{
    public DeleteAssetCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Asset id is required.");
    }
}
