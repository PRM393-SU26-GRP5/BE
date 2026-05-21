namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Booking history/list view.
/// Extends BookingDto with additional information for display purposes.
/// </summary>
public class BookingHistoryDto : BookingDto
{
    /// <summary>
    /// Name of the booked football field.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Location address of the field.
    /// </summary>
    public string? FieldLocation { get; set; }

    /// <summary>
    /// Name of the field owner/host.
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    /// Display-friendly status text (e.g., "Đã xác nhận", "Đang chờ", etc.).
    /// </summary>
    public string? StatusDisplay { get; set; }

    /// <summary>
    /// Whether the booking can be cancelled.
    /// </summary>
    public bool CanCancel { get; set; }

    /// <summary>
    /// Whether the booking can proceed to payment.
    /// </summary>
    public bool CanPayment { get; set; }

    /// <summary>
    /// List of time slot details (optional).
    /// </summary>
    public List<TimeSlotDto>? TimeSlots { get; set; }
}
