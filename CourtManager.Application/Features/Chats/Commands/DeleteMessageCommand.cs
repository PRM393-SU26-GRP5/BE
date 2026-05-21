using MediatR;

namespace CourtManager.Application.Features.Chats.Commands;

/// <summary>
/// Command to delete a message (soft delete).
/// </summary>
public class DeleteMessageCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the message to delete.
    /// </summary>
    public Guid MessageId { get; set; }

    public DeleteMessageCommand(Guid messageId)
    {
        MessageId = messageId;
    }
}
