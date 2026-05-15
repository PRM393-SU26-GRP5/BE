using Microsoft.AspNetCore.Identity;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a user in the system (player or admin).
/// </summary>
public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}
