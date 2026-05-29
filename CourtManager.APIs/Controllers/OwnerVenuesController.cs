using CourtManager.Application.Features.Venues.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// Owner-facing venue management endpoints.
/// </summary>
[Route("api/v1/owner/venues")]
[ApiController]
[Authorize(Roles = "Owner")]
public class OwnerVenuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public OwnerVenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the list of venues owned by the currently logged-in owner.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyVenues(
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(ownerIdStr, out var ownerId))
        {
            return Unauthorized(new { success = false, message = "Invalid user ID" });
        }

        var query = new GetOwnerVenuesQuery
        {
            OwnerId = ownerId,
            IsActive = isActive,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }
}
