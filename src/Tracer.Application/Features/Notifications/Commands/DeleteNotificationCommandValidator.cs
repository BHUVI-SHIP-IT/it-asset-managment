using FluentValidation;

namespace Tracer.Application.Features.Notifications.Commands;

/// <summary>Validates the DeleteNotificationCommand before the handler executes.</summary>
public sealed class DeleteNotificationCommandValidator : AbstractValidator<DeleteNotificationCommand>
{
    public DeleteNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Notification ID is required.");
    }
}
