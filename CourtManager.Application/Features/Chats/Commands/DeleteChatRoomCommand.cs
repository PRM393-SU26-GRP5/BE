using MediatR;

namespace CourtManager.Application.Features.Chats.Commands;

/// <summary>
/// Command to delete/close a chat room (soft delete).
/// </summary>
public class DeleteChatRoomCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the chat room to delete.
    /// </summary>
    public Guid RoomId { get; set; }

    public DeleteChatRoomCommand(Guid roomId)
    {
        RoomId = roomId;
    }
}
