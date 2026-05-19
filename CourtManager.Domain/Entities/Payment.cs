namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a payment transaction for one or more bookings.
/// </summary>
public class Payment
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public string TransactionCode { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }

    // Navigation property
    public Booking? Booking { get; set; }
}

/// <summary>
/// Enumeration for payment methods.
/// </summary>
public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    BankTransfer = 2,
    Cash = 3,
    Wallet = 4
}
