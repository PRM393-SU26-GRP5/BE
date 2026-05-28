using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourtManager.APIs.Attributes;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// Controller for admin operations requiring specific roles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IBookingRepository _bookingRepository;
    private readonly IFootballFieldRepository _fieldRepository;

    public AdminController(UserManager<User> userManager, IBookingRepository bookingRepository, IFootballFieldRepository fieldRepository)
    {
        _userManager = userManager;
        _bookingRepository = bookingRepository;
        _fieldRepository = fieldRepository;
    }

    /// <summary>
    /// Get user statistics (Admin only).
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
    {
        var totalUsers = await _userManager.Users.CountAsync(cancellationToken);
        var totalBookings = (await _bookingRepository.GetAllAsync(cancellationToken)).Count();

        return Ok(new
        {
            message = "Admin statistics",
            totalUsers,
            totalBookings
        });
    }

    /// <summary>
    /// Manage courts (Admin or Manager only).
    /// </summary>
    [HttpGet("courts")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ManageCourts(CancellationToken cancellationToken)
    {
        var fields = await _fieldRepository.GetAllAsync(cancellationToken);

        return Ok(new
        {
            message = "Court management interface",
            courts = fields.Select(f => new
            {
                f.Id,
                f.VenueId,
                f.FieldName,
                fieldType = f.FieldType.ToString(),
                f.PricePerHour,
                f.IsActive
            })
        });
    }

    /// <summary>
    /// Example of checking role programmatically.
    /// </summary>
    [HttpPost("test-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult TestRole()
    {
        var userRole = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

        if (userRole == "Admin")
        {
            return Ok(new { message = "You are an Admin!" });
        }

        return Forbid();
    }
}
