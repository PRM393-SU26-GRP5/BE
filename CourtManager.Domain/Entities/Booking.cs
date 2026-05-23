using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a booking of a football field by a user.
/// </summary>
public class Booking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public ICollection<BookingItem> BookingItems { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
    public Review? Review { get; set; }
    public ICollection<BookingDiscount> BookingDiscounts { get; set; } = [];
}
