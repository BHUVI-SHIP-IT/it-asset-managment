using FluentValidation;

namespace Tracer.Application.Features.Settings.Commands;

/// <summary>Validates the DeleteSettingCommand before the handler executes.</summary>
public sealed class DeleteSettingCommandValidator : AbstractValidator<DeleteSettingCommand>
{
    public DeleteSettingCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Setting key is required.")
            .MaximumLength(255).WithMessage("Setting key must not exceed 255 characters.");
    }
}
