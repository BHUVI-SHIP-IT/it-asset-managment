using FluentValidation;

namespace Tracer.Application.Features.Settings.Commands;

/// <summary>
/// Validates the UpsertSettingCommand. Keys follow a dot-notation convention
/// (e.g. "Notifications.Slack.Enabled") and values are capped at 4,000 chars
/// to match the database column length in TenantSettingConfiguration.
/// </summary>
public sealed class UpsertSettingCommandValidator : AbstractValidator<UpsertSettingCommand>
{
    public UpsertSettingCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Setting key is required.")
            .MaximumLength(255).WithMessage("Setting key must not exceed 255 characters.")
            .Matches(@"^[a-zA-Z0-9_.]+$")
            .WithMessage("Setting key may only contain letters, digits, underscores, and dots.");

        RuleFor(x => x.Value)
            .MaximumLength(4000).WithMessage("Setting value must not exceed 4,000 characters.")
            .When(x => x.Value is not null);
    }
}
