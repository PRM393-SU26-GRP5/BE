using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByCourtIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.BookingItems.Any(i => i.Slot != null && i.Slot.FieldId == fieldId))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetPendingBookingsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.BookingStatus == CourtManager.Domain.Enums.BookingStatus.Pending
                && b.BookingItems.Any(i => i.Slot != null && i.Slot.Field != null && i.Slot.Field.Venue != null && i.Slot.Field.Venue.OwnerId == ownerId))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.BookingItems.Any(i => i.Slot != null && i.Slot.Field != null && i.Slot.Field.Venue != null && i.Slot.Field.Venue.OwnerId == ownerId))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCourtAvailableAsync(Guid fieldId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return !await _context.TimeSlots
            .AnyAsync(s => s.FieldId == fieldId
                && s.StartTime < endTime
                && s.EndTime > startTime
                && s.SlotStatus != CourtManager.Domain.Enums.SlotStatus.Available,
                cancellationToken);
    }

    private static IQueryable<Booking> IncludeDetails(IQueryable<Booking> query)
    {
        return query
            .Include(b => b.User)
            .Include(b => b.Payments)
            .Include(b => b.BookingDiscounts)
                .ThenInclude(bd => bd.Discount)
            .Include(b => b.BookingItems)
                .ThenInclude(i => i.Slot)
                    .ThenInclude(s => s!.Field)
                        .ThenInclude(f => f!.Venue);
    }
}
