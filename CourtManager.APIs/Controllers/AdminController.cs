using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Admin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new GetAdminUsersQuery(), cancellationToken);
        return Ok(users);
    }

    [HttpPut("users/{id:guid}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDto request)
    {
        var result = await _mediator.Send(new UpdateAdminUserRoleCommand(id, request));
        return Ok(result);
    }

    [HttpGet("venues")]
    public async Task<IActionResult> GetVenues(CancellationToken cancellationToken)
    {
        var venues = await _mediator.Send(new GetAdminVenuesQuery(), cancellationToken);
        return Ok(venues);
    }

    [HttpPut("venues/{id:guid}/status")]
    public async Task<IActionResult> UpdateVenueStatus(Guid id, [FromBody] UpdateStatusDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAdminVenueStatusCommand(id, request), cancellationToken);
        return Ok(result);
    }

    [HttpPost("notifications/broadcast")]
    public async Task<IActionResult> BroadcastNotification([FromBody] BroadcastNotificationDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new BroadcastNotificationCommand(GetCurrentUserId(), request), cancellationToken);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
