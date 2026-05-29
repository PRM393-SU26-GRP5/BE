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

    [HttpPost]
    public async Task<IActionResult> CreateVenue([FromBody] CourtManager.Application.DTOs.CreateVenueRequestDto request)
    {
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(ownerIdStr, out var ownerId))
        {
            return Unauthorized(new { success = false, message = "Invalid user ID" });
        }

        var command = new CourtManager.Application.Features.Venues.Commands.CreateVenueCommand(
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

        return Ok(new
        {
            success = true,
            message = "Venue created successfully",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadVenueImages(Guid id, [FromForm] List<Microsoft.AspNetCore.Http.IFormFile> images)
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
            var fileDtos = images.Select(f => new CourtManager.Application.DTOs.FileUploadDto(f.OpenReadStream(), f.FileName, f.ContentType)).ToList();
            var command = new CourtManager.Application.Features.Venues.Commands.UploadVenueImagesCommand(id, userId, fileDtos);
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

    /// <summary>
    /// Updates a venue. Only the owner of the venue may update it.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] CourtManager.Application.DTOs.UpdateVenueRequestDto request)
    {
        var ownerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(ownerIdStr, out var ownerId))
        {
            return Unauthorized(new { success = false, message = "Invalid user ID" });
        }

        var command = new CourtManager.Application.Features.Venues.Commands.UpdateVenueCommand(
            id,
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

        return Ok(new
        {
            success = true,
            message = "Venue updated successfully",
            data = result,
            errors = Array.Empty<string>()
        });
    }
}
