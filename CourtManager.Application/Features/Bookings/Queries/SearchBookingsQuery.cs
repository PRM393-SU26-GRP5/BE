using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to search bookings with advanced filtering.
/// Filters by status, date range, and user ID.
/// </summary>
public class SearchBookingsQuery : IRequest<IEnumerable<BookingDto>>
{
    /// <summary>
    /// Optional user ID filter.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Optional field ID filter.
    /// </summary>
    public Guid? FieldId { get; set; }

    /// <summary>
    /// Optional booking status filter (e.g., "Pending", "Confirmed", "Completed", "Cancelled").
    /// </summary>
    public string? BookingStatus { get; set; }

    /// <summary>
    /// Optional start date for filtering (inclusive).
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Optional end date for filtering (inclusive).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Page number for pagination (default 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size for pagination (default 10).
    /// </summary>
    public int PageSize { get; set; } = 10;
}
