using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Fields;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/fields")]
[Authorize]
public class FieldsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FieldsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [NonAction]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<FootballFieldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FootballFieldDto>>> GetFieldsByVenue(Guid venueId, CancellationToken cancellationToken)
    {
        var fields = await _mediator.Send(new GetFieldsByVenueQuery(venueId), cancellationToken);
        return Ok(fields);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FootballFieldDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FootballFieldDto>> GetFieldById(Guid id, CancellationToken cancellationToken)
    {
        var field = await _mediator.Send(new GetFieldByIdQuery(id), cancellationToken);
        return Ok(field);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(FootballFieldDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FootballFieldDto>> CreateField(Guid venueId, [FromBody] FootballFieldDto field, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateFieldCommand(GetCurrentUserId(), venueId, field), cancellationToken);
        return CreatedAtAction(nameof(GetFieldById), new { id = created.Id }, created);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(FootballFieldDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FootballFieldDto>> UpdateField(Guid id, [FromBody] FootballFieldDto field, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateFieldCommand(GetCurrentUserId(), id, field), cancellationToken);
        return Ok(updated);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> DeleteField(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteFieldCommand(GetCurrentUserId(), id), cancellationToken);
        return Ok(new { success = result });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
