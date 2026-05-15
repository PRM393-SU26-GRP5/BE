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
    public PaymentStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
}
