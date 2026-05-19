using CourtManager.Domain.Entities;

namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Payment.
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime? PaidAt { get; set; }
}
