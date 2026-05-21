using MediatR;

namespace CourtManager.Application.Features.Notifications.Commands;

/// <summary>
/// Command to delete a notification (soft delete).
/// </summary>
public class DeleteNotificationCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the notification to delete.
    /// </summary>
    public Guid NotificationId { get; set; }

    public DeleteNotificationCommand(Guid notificationId)
    {
        NotificationId = notificationId;
    }
}
