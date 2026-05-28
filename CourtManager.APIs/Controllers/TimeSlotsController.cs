using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Bookings.Commands;
using CourtManager.Application.Features.Bookings.Queries;
using CourtManager.Application.Features.TimeSlots;
using CourtManager.Application.Features.TimeSlots.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing time slots.
/// Provides CRUD operations and query endpoints for time slots.
/// </summary>
[ApiController]
[Route("api/v1/slots")]
[Authorize]
public class TimeSlotsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TimeSlotsController> _logger;

    public TimeSlotsController(IMediator mediator, ILogger<TimeSlotsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all time slots for a specific field.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <returns>List of time slots</returns>
    [NonAction]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetSlotsByField([FromQuery] Guid fieldId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching time slots for field {FieldId}", fieldId);
        var result = await _mediator.Send(new GetSlotsByFieldQuery(fieldId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific time slot by ID.
    /// </summary>
    /// <param name="id">The time slot ID</param>
    /// <returns>Time slot details</returns>
    [NonAction]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TimeSlotDto>> GetSlotById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching time slot {SlotId}", id);
        var result = await _mediator.Send(new GetTimeSlotByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets available time slots for a field on a specific date.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="date">The date</param>
    /// <returns>List of available time slots</returns>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetAvailableSlots([FromQuery] Guid fieldId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching available slots for field {FieldId} on {Date}", fieldId, date);
        var result = await _mediator.Send(new GetAvailableSlotsQuery(fieldId, date), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/lock")]
    [Authorize(Roles = "Player,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LockSlot(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Locking slot {SlotId}", id);
        var result = await _mediator.Send(new LockTimeSlotCommand(id, Guid.NewGuid()), cancellationToken);
        return Ok(new { success = result, message = "Time slot locked successfully" });
    }

    [HttpPost("{id:guid}/unlock")]
    [Authorize(Roles = "Player,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UnlockSlot(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unlocking slot {SlotId}", id);
        var result = await _mediator.Send(new UnlockTimeSlotCommand(id, "CustomerUnlock"), cancellationToken);
        return Ok(new { success = result, message = "Time slot unlocked successfully" });
    }

    /// <summary>
    /// Creates a new time slot (Manager/Admin only).
    /// </summary>
    /// <param name="slot">The time slot creation data</param>
    /// <returns>Created time slot</returns>
    [NonAction]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TimeSlotDto>> CreateSlot([FromBody] TimeSlotDto slot, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new time slot for field {FieldId}", slot.FieldId);
        var result = await _mediator.Send(new CreateTimeSlotCommand(GetCurrentUserId(), slot), cancellationToken);
        return CreatedAtAction(nameof(GetSlotById), new { id = result.SlotId }, result);
    }

    /// <summary>
    /// Updates a time slot (Manager/Admin only).
    /// </summary>
    /// <param name="id">The time slot ID</param>
    /// <param name="slot">The updated time slot data</param>
    /// <returns>Updated time slot</returns>
    [NonAction]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TimeSlotDto>> UpdateSlot(Guid id, [FromBody] TimeSlotDto slot, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating time slot {SlotId}", id);
        var result = await _mediator.Send(new UpdateTimeSlotCommand(GetCurrentUserId(), id, slot), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a time slot (soft delete - Manager/Admin only).
    /// </summary>
    /// <param name="id">The time slot ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [NonAction]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSlot(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting time slot {SlotId}", id);
        var command = new DeleteTimeSlotCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Time slot {SlotId} deleted successfully (soft delete)", id);
        return Ok(new { success = result, message = "Time slot deleted successfully" });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
