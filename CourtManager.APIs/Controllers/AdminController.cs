using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using CourtManager.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IVenueRepository _venueRepository;
    private readonly ApplicationDbContext _dbContext;

    public AdminController(UserManager<User> userManager, IVenueRepository venueRepository, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _venueRepository = venueRepository;
        _dbContext = dbContext;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                phone = u.PhoneNumber,
                u.IsActive,
                u.LoyaltyPoints
            })
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpPut("users/{id:guid}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDto request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        var role = request.Role.Trim().ToLowerInvariant() switch
        {
            "admin" => "Admin",
            "owner" or "manager" => "Manager",
            "customer" or "player" => "Player",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(role)) return BadRequest(new { message = "Invalid role" });

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new { userId = id, role });
    }

    [HttpGet("venues")]
    public async Task<IActionResult> GetVenues(CancellationToken cancellationToken)
    {
        var venues = await _venueRepository.GetAllAsync(cancellationToken);
        return Ok(venues);
    }

    [HttpPut("venues/{id:guid}/status")]
    public async Task<IActionResult> UpdateVenueStatus(Guid id, [FromBody] UpdateStatusDto request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);
        if (venue == null) return NotFound();

        venue.IsActive = request.IsActive;
        venue.UpdatedAt = DateTime.UtcNow;
        await _venueRepository.UpdateAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { venueId = id, isActive = venue.IsActive });
    }

    [HttpPost("notifications/broadcast")]
    public async Task<IActionResult> BroadcastNotification([FromBody] BroadcastNotificationDto request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.Where(u => u.IsActive).Select(u => u.Id).ToListAsync(cancellationToken);
        var notification = new Notification
        {
            NotificationId = Guid.NewGuid(),
            SenderId = GetCurrentUserId(),
            Title = request.Title,
            Message = request.Message,
            Type = NotificationType.Broadcast,
            RefId = request.RefId ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            NotificationRecipients = users.Select(userId => new NotificationRecipient
            {
                RecipientId = Guid.NewGuid(),
                UserId = userId
            }).ToList()
        };

        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new { notificationId = notification.NotificationId, recipients = users.Count });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}

public class UpdateUserRoleDto
{
    public string Role { get; set; } = string.Empty;
}

public class BroadcastNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RefId { get; set; }
}
