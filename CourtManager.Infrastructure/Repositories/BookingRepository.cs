using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Booking entity.
/// Inherits from base Repository and implements IBookingRepository.
/// </summary>
public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByCourtIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.FieldId == fieldId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCourtAvailableAsync(
        Guid fieldId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        // Check if there are any confirmed or pending bookings that overlap with the requested time
        var hasConflict = await _dbSet
            .AnyAsync(b =>
                b.FieldId == fieldId &&
                (b.BookingStatus == "Confirmed" || b.BookingStatus == "Pending") &&
                b.StartTime < endTime && b.EndTime > startTime,
                cancellationToken);

        return !hasConflict;
    }
}
