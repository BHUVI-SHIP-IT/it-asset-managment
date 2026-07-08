using FluentValidation;

namespace Tracer.Application.Features.Notifications.Commands;

/// <summary>Validates the MarkNotificationReadCommand before the handler executes.</summary>
public sealed class MarkNotificationReadCommandValidator : AbstractValidator<MarkNotificationReadCommand>
{
    public MarkNotificationReadCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Notification ID is required.");
    }
}
