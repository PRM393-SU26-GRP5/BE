using CourtManager.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/amenities")]
public class AmenitiesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AmenitiesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAmenities(CancellationToken cancellationToken)
    {
        var amenities = await _dbContext.Amenities
            .OrderBy(a => a.Name)
            .Select(a => new
            {
                a.AmenityId,
                a.Name,
                a.Icon
            })
            .ToListAsync(cancellationToken);

        return Ok(amenities);
    }
}
