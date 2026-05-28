using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetProfile(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserProfileQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateUserProfileDto profile, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateUserProfileCommand(GetCurrentUserId(), profile), cancellationToken);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
