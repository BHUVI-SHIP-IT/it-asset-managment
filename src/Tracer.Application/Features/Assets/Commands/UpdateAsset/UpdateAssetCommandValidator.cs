using FluentValidation;

namespace Tracer.Application.Features.Assets.Commands.UpdateAsset;

public sealed class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Asset name is required.")
            .MaximumLength(255);

        RuleFor(x => x.SerialNumber).MaximumLength(255);
        RuleFor(x => x.Notes).MaximumLength(2000); // Doc 2 §2.1.6 Notes max 2000 chars.

        RuleFor(x => x.AssetModelId).NotEmpty();
        RuleFor(x => x.StatusLabelId).GreaterThan(0);
        RuleFor(x => x.PurchaseCost).GreaterThanOrEqualTo(0);
    }
}
