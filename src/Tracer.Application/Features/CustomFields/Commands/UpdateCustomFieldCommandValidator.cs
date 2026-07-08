using FluentValidation;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Application.Features.CustomFields.Commands;

/// <summary>
/// Validates the UpdateCustomFieldCommand. Mirrors CreateCustomFieldCommandValidator
/// with the addition of a non-empty Id guard.
/// </summary>
public sealed class UpdateCustomFieldCommandValidator : AbstractValidator<UpdateCustomFieldCommand>
{
    public UpdateCustomFieldCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Custom field ID is required.");

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
