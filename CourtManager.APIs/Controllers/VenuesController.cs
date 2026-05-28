using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Venues;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/venues")]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<VenueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues([FromQuery] VenueQueryDto query, CancellationToken cancellationToken)
    {
        var venues = await _mediator.Send(new GetVenuesQuery(query), cancellationToken);
        return Ok(venues);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<VenueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VenueDto>>> SearchVenues([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var venues = await _mediator.Send(new GetVenuesQuery(new VenueQueryDto { Q = q }), cancellationToken);
        return Ok(venues);
    }

    [HttpGet("map/nearby")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<VenueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetNearbyVenues(
        [FromQuery] decimal lat,
        [FromQuery] decimal lng,
        [FromQuery] decimal radiusKm = 10,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var venues = await _mediator.Send(new GetNearbyVenuesQuery(lat, lng, radiusKm, pageNumber, pageSize), cancellationToken);
        return Ok(venues);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VenueDto>> GetVenueById(Guid id, CancellationToken cancellationToken)
    {
        var venue = await _mediator.Send(new GetVenueByIdQuery(id), cancellationToken);
        return Ok(venue);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<VenueDto>> CreateVenue([FromBody] CreateVenueDto venue, CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var created = await _mediator.Send(new CreateVenueCommand(ownerId, venue), cancellationToken);
        return CreatedAtAction(nameof(GetVenueById), new { id = created.VenueId }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VenueDto>> UpdateVenue(Guid id, [FromBody] UpdateVenueDto venue, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateVenueCommand(id, GetCurrentUserId(), venue), cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DeleteVenue(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteVenueCommand(id, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
