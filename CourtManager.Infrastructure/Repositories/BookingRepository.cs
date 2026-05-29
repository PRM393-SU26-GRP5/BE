using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    private readonly ApplicationDbContext _db;

    public BookingRepository(ApplicationDbContext context) : base(context)
    {
        _db = context;
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByCourtIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsCourtAvailableAsync(Guid fieldId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> HasActiveBookingsForVenueAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        // Active = Pending, Accepted, or Deposited (still in-flight, not yet completed/rejected/cancelled)
        var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.Accepted, BookingStatus.Deposited };

        return await _db.Bookings
            .Where(b => !b.IsDeleted && activeStatuses.Contains(b.BookingStatus))
            .AnyAsync(b => b.BookingItems.Any(bi =>
                bi.Slot != null &&
                bi.Slot.Field != null &&
                bi.Slot.Field.VenueId == venueId),
            cancellationToken);
    }
}
