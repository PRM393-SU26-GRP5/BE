namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for User.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}
