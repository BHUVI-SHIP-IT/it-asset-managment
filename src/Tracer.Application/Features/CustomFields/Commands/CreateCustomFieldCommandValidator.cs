using FluentValidation;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Application.Features.CustomFields.Commands;

/// <summary>
/// Validates the CreateCustomFieldCommand. Enforces name length, field-type enum range,
/// and the Dropdown-requires-Options invariant at the Application layer (defence-in-depth
/// on top of the domain aggregate guard).
/// </summary>
public sealed class CreateCustomFieldCommandValidator : AbstractValidator<CreateCustomFieldCommand>
{
    public CreateCustomFieldCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Custom field name is required.")
            .MaximumLength(255).WithMessage("Custom field name must not exceed 255 characters.");

        RuleFor(x => x.FieldType)
            .IsInEnum().WithMessage("Field type is not a valid value.");

        RuleFor(x => x.Options)
            .NotEmpty().WithMessage("Dropdown fields require at least one option.")
            .When(x => x.FieldType == CustomFieldType.Dropdown);

        RuleFor(x => x.Options)
            .MaximumLength(4000).WithMessage("Options must not exceed 4,000 characters.")
            .When(x => x.Options is not null);
    }
}
