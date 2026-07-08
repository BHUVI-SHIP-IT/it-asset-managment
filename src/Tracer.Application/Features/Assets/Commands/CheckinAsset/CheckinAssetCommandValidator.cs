using FluentValidation;

namespace Tracer.Application.Features.Assets.Commands.CheckinAsset;

public sealed class CheckinAssetCommandValidator : AbstractValidator<CheckinAssetCommand>
{
    public CheckinAssetCommandValidator()
    {
        RuleFor(x => x.AssetId).NotEmpty().WithMessage("Asset id is required.");
    }
}
