using FluentValidation;

namespace Tracer.Application.Features.Assets.Commands.CreateAsset;

/// <summary>Payload validation (Doc 2 §2.1.6: mandatory fields, string length constraints).</summary>
public sealed class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.AssetTag)
            .NotEmpty().WithMessage("Asset tag is required.")
            .MaximumLength(100);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Asset name is required.")
            .MaximumLength(255);

        RuleFor(x => x.SerialNumber)
            .MaximumLength(255);

        RuleFor(x => x.AssetModelId)
            .NotEmpty().WithMessage("An asset model is required.");

        RuleFor(x => x.StatusLabelId)
            .GreaterThan(0).WithMessage("A valid status label is required.");

        RuleFor(x => x.PurchaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("Purchase cost cannot be negative.");
    }
}
