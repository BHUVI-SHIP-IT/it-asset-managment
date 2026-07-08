using FluentValidation;

namespace Tracer.Application.Features.CustomFields.Commands;

/// <summary>
/// Validates the SetCustomFieldValueCommand. Enforces that both IDs are non-empty
/// and the value stays within the database column limit.
/// </summary>
public sealed class SetCustomFieldValueCommandValidator : AbstractValidator<SetCustomFieldValueCommand>
{
    public SetCustomFieldValueCommandValidator()
    {
        RuleFor(x => x.CustomFieldId)
            .NotEmpty().WithMessage("Custom field ID is required.");

        RuleFor(x => x.EntityId)
            .NotEmpty().WithMessage("Entity ID is required.");

        RuleFor(x => x.Value)
            .MaximumLength(4000).WithMessage("Value must not exceed 4,000 characters.")
            .When(x => x.Value is not null);
    }
}
