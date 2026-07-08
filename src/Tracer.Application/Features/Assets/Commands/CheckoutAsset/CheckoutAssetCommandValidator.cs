using FluentValidation;

namespace Tracer.Application.Features.Assets.Commands.CheckoutAsset;

public sealed class CheckoutAssetCommandValidator : AbstractValidator<CheckoutAssetCommand>
{
    public CheckoutAssetCommandValidator()
    {
        RuleFor(x => x.AssetId).NotEmpty().WithMessage("Asset id is required.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("A target user is required for checkout.");
    }
}
