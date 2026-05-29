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
    public async Task<IActionResult> GetVenues([FromQuery] string? q, [FromQuery] FieldType? fieldType, [FromQuery] string? amenityIds, [FromQuery] double? minRating, [FromQuery] decimal? priceMin, [FromQuery] decimal? priceMax, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var parsedAmenityIds = new List<Guid>();
        if (!string.IsNullOrEmpty(amenityIds))
        {
            parsedAmenityIds = amenityIds.Split(',').Select(id => Guid.TryParse(id, out var parsedId) ? parsedId : Guid.Empty).Where(id => id != Guid.Empty).ToList();
        }

        var query = new GetVenuesQuery
        {
            Q = q,
            FieldType = fieldType,
            AmenityIds = parsedAmenityIds.Any() ? parsedAmenityIds : null,
            MinRating = minRating,
            PriceMin = priceMin,
            PriceMax = priceMax,
            Sort = sort,
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
}
