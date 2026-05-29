using CourtManager.Application.Features.Venues.Queries;
using CourtManager.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetVenues([FromQuery] GetVenuesQuery query)
    {
        var result = await _mediator.Send(query);
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchVenues([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetVenuesQuery
        {
            Q = q,
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

    [HttpGet("map/nearby")]
    public async Task<IActionResult> GetNearbyVenues([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radius = 5.0)
    {
        var query = new GetNearbyVenuesQuery
        {
            Latitude = lat,
            Longitude = lng,
            RadiusInKm = radius
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
