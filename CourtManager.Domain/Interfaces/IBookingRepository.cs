using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Booking entity with specific queries.
/// </summary>
public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsByCourtIdAsync(Guid courtId, CancellationToken cancellationToken = default);
    Task<bool> IsCourtAvailableAsync(Guid courtId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
}
