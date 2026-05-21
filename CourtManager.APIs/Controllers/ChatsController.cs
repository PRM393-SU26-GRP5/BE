using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Chats.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing chat and messaging.
/// Provides endpoints for real-time communication between users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ChatsController> _logger;

    public ChatsController(IMediator mediator, ILogger<ChatsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all chat rooms for the current user.
    /// </summary>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <returns>Paginated list of chat rooms</returns>
    [HttpGet("rooms")]
    [ProducesResponseType(typeof(IEnumerable<ChatRoomDto>), StatusCodes.Status200OK)]
    public IActionResult GetChatRooms([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Fetching chat rooms for user {UserId}", userId);
        return Ok(new { message = "Get chat rooms endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets or creates a chat room with another user.
    /// </summary>
    /// <param name="otherUserId">The other user's ID</param>
    /// <returns>Chat room details</returns>
    [HttpGet("room/{otherUserId}")]
    [ProducesResponseType(typeof(ChatRoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrCreateChatRoom(Guid otherUserId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Getting or creating chat room between users {UserId} and {OtherUserId}", userId, otherUserId);
        return Ok(new { message = "Get or create chat room endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets messages from a specific chat room.
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 20)</param>
    /// <returns>Paginated list of messages</returns>
    [HttpGet("rooms/{roomId}/messages")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetMessages(Guid roomId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Fetching messages for room {RoomId}", roomId);
        return Ok(new { message = "Get messages endpoint - implementation pending" });
    }

    /// <summary>
    /// Sends a message in a chat room.
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="message">The message data</param>
    /// <returns>Created message</returns>
    [HttpPost("rooms/{roomId}/messages")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult SendMessage(Guid roomId, [FromBody] MessageDto message)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Sending message to room {RoomId} from user {UserId}", roomId, userId);
        return Ok(new { message = "Send message endpoint - implementation pending" });
    }

    /// <summary>
    /// Deletes a message from a chat room (soft delete).
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="messageId">The message ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("rooms/{roomId}/messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteMessage(Guid roomId, Guid messageId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting message {MessageId} from room {RoomId}", messageId, roomId);
        var command = new DeleteMessageCommand(messageId);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Message {MessageId} deleted successfully (soft delete)", messageId);
        return Ok(new { success = result, message = "Message deleted successfully" });
    }

    /// <summary>
    /// Closes/archives a chat room (soft delete).
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("rooms/{roomId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CloseChatRoom(Guid roomId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Closing chat room {RoomId}", roomId);
        var command = new DeleteChatRoomCommand(roomId);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Chat room {RoomId} closed successfully (soft delete)", roomId);
        return Ok(new { success = result, message = "Chat room closed successfully" });
    }
}
