using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Venues.Commands;
using CourtManager.Application.Features.Venues.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequestDto request)
    {
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(ownerIdStr, out var ownerId))
        {
            return Unauthorized(new { success = false, message = "Invalid user ID" });
        }

        var command = new CreateVenueCommand(
            ownerId,
            request.VenueName,
            request.Address,
            request.Latitude,
            request.Longitude,
            request.Description,
            request.OpeningHours,
            request.PhoneContact
        );

        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetVenueById), new { id = result.VenueId }, new
        {
            success = true,
            message = "Venue created successfully",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpPost("{id}/images")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> UploadVenueImages(Guid id, [FromForm] List<IFormFile> images)
    {
        if (images == null || !images.Any())
        {
            return BadRequest(new { success = false, message = "No images provided" });
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(new { success = false, message = "Invalid user ID" });
        }

        try
        {
            var fileDtos = images.Select(f => new FileUploadDto(f.OpenReadStream(), f.FileName, f.ContentType)).ToList();
            var command = new UploadVenueImagesCommand(id, userId, fileDtos);
            var uploadedUrls = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                message = "Images uploaded successfully",
                data = uploadedUrls,
                errors = Array.Empty<string>()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to upload images",
                errors = new[] { ex.Message }
            });
        }
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVenueById(Guid id)
    {
        var result = await _mediator.Send(new GetVenueByIdQuery(id));
        
        if (result == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Venue not found.",
                errors = new[] { "VENUE_NOT_FOUND" }
            });
        }

        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("{id}/fields")]
    public async Task<IActionResult> GetVenueFields(Guid id)
    {
        var result = await _mediator.Send(new GetVenueFieldsQuery(id));
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("{id}/amenities")]
    public async Task<IActionResult> GetVenueAmenities(Guid id)
    {
        var result = await _mediator.Send(new GetVenueAmenitiesQuery(id));
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetVenueImages(Guid id)
    {
        var result = await _mediator.Send(new GetVenueImagesQuery(id));
        
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
