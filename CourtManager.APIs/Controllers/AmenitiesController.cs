using CourtManager.Application.Features.Amenities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/amenities")]
public class AmenitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AmenitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAmenities(CancellationToken cancellationToken)
    {
        var amenities = await _mediator.Send(new GetAmenitiesQuery(), cancellationToken);
        return Ok(amenities);
    }
}
