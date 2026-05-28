using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Bookings.Commands;
using CourtManager.Application.Features.Bookings.Queries;
using CourtManager.Application.Features.Discounts;
using CourtManager.Application.Features.Fields;
using CourtManager.Application.Features.Owner;
using CourtManager.Application.Features.Venues;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/owner")]
[Authorize(Roles = "Manager,Admin")]
public class OwnerController : ControllerBase
{
    private readonly IMediator _mediator;

    public OwnerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(OwnerStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OwnerStatsDto>> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerStatsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("venues")]
    [ProducesResponseType(typeof(IEnumerable<VenueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerVenuesQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("venues")]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<VenueDto>> CreateVenue([FromBody] CreateVenueDto venue, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateVenueCommand(GetCurrentUserId(), venue), cancellationToken);
        return Created($"/api/venues/{result.VenueId}", result);
    }

    [HttpGet("venues/{venueId}/fields")]
    [ProducesResponseType(typeof(IEnumerable<FootballFieldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FootballFieldDto>>> GetFields(Guid venueId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFieldsByVenueQuery(venueId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("venues/{venueId}/fields")]
    [ProducesResponseType(typeof(FootballFieldDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FootballFieldDto>> CreateField(Guid venueId, [FromBody] FootballFieldDto field, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateFieldCommand(GetCurrentUserId(), venueId, field), cancellationToken);
        return Created($"/api/fields/{result.Id}", result);
    }

    [HttpGet("bookings/pending")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetPendingBookings(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerPendingBookingsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPut("bookings/{id:guid}/accept")]
    public async Task<IActionResult> AcceptBooking(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AcceptBookingCommand(id, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpPut("bookings/{id:guid}/reject")]
    public async Task<IActionResult> RejectBooking(Guid id, [FromQuery] string? rejectionReason, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectBookingCommand(id, rejectionReason, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpGet("discounts")]
    [ProducesResponseType(typeof(IEnumerable<DiscountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetDiscounts(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerDiscountsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("discounts")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DiscountDto>> CreateDiscount([FromBody] DiscountDto discount, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDiscountCommand(GetCurrentUserId(), discount), cancellationToken);
        return Created($"/api/discounts/{result.DiscountId}", result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
