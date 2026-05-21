using FluentValidation;

namespace CourtManager.Application.Features.Notifications.Commands;

/// <summary>
/// Validator for DeleteNotificationCommand.
/// </summary>
public class DeleteNotificationCommandValidator : AbstractValidator<DeleteNotificationCommand>
{
    public DeleteNotificationCommandValidator()
    {
        RuleFor(x => x.NotificationId)
            .NotEmpty().WithMessage("Notification ID is required");
    }
}
