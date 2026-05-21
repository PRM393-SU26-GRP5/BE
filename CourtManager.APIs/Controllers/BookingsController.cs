using CourtManager.Application.Features.Bookings.Commands;
using CourtManager.Application.Features.Bookings.Queries;
using CourtManager.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing bookings.
/// Provides CRUD operations and query endpoints for bookings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="command">The booking creation command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created booking</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingDto>> CreateBooking(
        [FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var currentUserId))
        {
            return Unauthorized();
        }

        // Force the command to use the logged-in user's ID
        // (Prevents IDOR: User creating booking for someone else)
        command.UserId = currentUserId;

        _logger.LogInformation(
            "Creating booking for User: {UserId}, Field: {FieldId}",
            command.UserId, command.FieldId);

        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Booking created successfully with ID: {BookingId}", result.Id);

        return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves a booking by ID.
    /// </summary>
    /// <param name="id">The booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The booking details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingDto>> GetBookingById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching booking with ID: {BookingId}", id);

        var query = new GetBookingByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        // Resource-based Authorization: Only Admin/Manager or the owner can view this booking
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

        if (!isAdminOrManager && result.UserId.ToString() != currentUserId)
        {
            _logger.LogWarning("User {UserId} attempted to access booking {BookingId} which they do not own.", currentUserId, id);
            return Forbid();
        }

        return Ok(result);
    }

    /// <summary>
    /// Accepts/confirms a pending booking.
    /// Changes booking status from "Pending" to "Confirmed".
    /// </summary>
    /// <param name="id">The booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AcceptBooking(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Accepting booking with ID: {BookingId}", id);

        var command = new AcceptBookingCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Booking {BookingId} accepted successfully", id);

        return Ok(new { success = result, message = "Booking accepted successfully" });
    }

    /// <summary>
    /// Rejects a pending booking.
    /// Changes booking status from "Pending" to "Rejected".
    /// </summary>
    /// <param name="id">The booking ID</param>
    /// <param name="rejectionReason">Optional reason for rejection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RejectBooking(
        Guid id,
        [FromQuery] string? rejectionReason = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rejecting booking with ID: {BookingId}", id);

        var command = new RejectBookingCommand(id, rejectionReason);
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Booking {BookingId} rejected successfully", id);

        return Ok(new { success = result, message = "Booking rejected successfully" });
    }

    /// <summary>
    /// Cancels an existing booking.
    /// Can cancel Pending or Confirmed bookings.
    /// </summary>
    /// <param name="id">The booking ID</param>
    /// <param name="cancellationReason">Optional reason for cancellation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelBooking(
        Guid id,
        [FromQuery] string? cancellationReason = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling booking with ID: {BookingId}", id);

        var command = new CancelBookingCommand(id, cancellationReason);
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Booking {BookingId} cancelled successfully", id);

        return Ok(new { success = result, message = "Booking cancelled successfully" });
    }

    /// <summary>
    /// Locks a time slot during payment process.
    /// Changes slot status from "Available" to "Locked".
    /// Prevents other bookings from using the slot while payment is processing.
    /// </summary>
    /// <param name="slotId">The time slot ID</param>
    /// <param name="bookingId">The associated booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("slots/{slotId}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LockTimeSlot(
        Guid slotId,
        [FromQuery] Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Locking time slot {SlotId} for booking {BookingId}", slotId, bookingId);

        var command = new LockTimeSlotCommand(slotId, bookingId);
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Time slot {SlotId} locked successfully", slotId);

        return Ok(new { success = result, message = "Time slot locked successfully" });
    }

    /// <summary>
    /// Unlocks a time slot.
    /// Changes slot status from "Locked" back to "Available".
    /// Used when payment fails, times out, or is refunded.
    /// </summary>
    /// <param name="slotId">The time slot ID</param>
    /// <param name="unlockReason">Reason for unlock (e.g., "PaymentFailed", "PaymentTimeout", "Refund")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("slots/{slotId}/unlock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlockTimeSlot(
        Guid slotId,
        [FromQuery] string unlockReason = "ManualUnlock",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unlocking time slot {SlotId} with reason: {Reason}", slotId, unlockReason);

        var command = new UnlockTimeSlotCommand(slotId, unlockReason);
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Time slot {SlotId} unlocked successfully", slotId);

        return Ok(new { success = result, message = "Time slot unlocked successfully" });
    }

    /// <summary>
    /// Health check endpoint for API.
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "API is running" });
    }
}
