using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetReviewsByFieldIdAsync(Guid fieldId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var venueId = await _context.FootballFields
            .Where(f => f.Id == fieldId)
            .Select(f => (Guid?)f.VenueId)
            .FirstOrDefaultAsync(cancellationToken);

        return venueId.HasValue
            ? await GetReviewsByVenueIdAsync(venueId.Value, pageNumber, pageSize, cancellationToken)
            : [];
    }

    public async Task<IEnumerable<Review>> GetReviewsByVenueIdAsync(Guid venueId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Venue)
            .Where(r => r.VenueId == venueId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((Math.Max(pageNumber, 1) - 1) * Math.Max(pageSize, 1))
            .Take(Math.Max(pageSize, 1))
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetUserReviewAsync(Guid userId, Guid fieldId, CancellationToken cancellationToken = default)
    {
        var venueId = await _context.FootballFields
            .Where(f => f.Id == fieldId)
            .Select(f => (Guid?)f.VenueId)
            .FirstOrDefaultAsync(cancellationToken);

        return venueId.HasValue
            ? await _dbSet.FirstOrDefaultAsync(r => r.UserId == userId && r.VenueId == venueId.Value, cancellationToken)
            : null;
    }

    public async Task<Review?> GetUserReviewForBookingAsync(Guid userId, Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Venue)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.BookingId == bookingId, cancellationToken);
    }

    public async Task<decimal> GetAverageRatingAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        var venueId = await _context.FootballFields
            .Where(f => f.Id == fieldId)
            .Select(f => (Guid?)f.VenueId)
            .FirstOrDefaultAsync(cancellationToken);

        return venueId.HasValue
            ? await GetAverageRatingByVenueIdAsync(venueId.Value, cancellationToken)
            : 0;
    }

    public async Task<decimal> GetAverageRatingByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        var ratings = await _dbSet
            .Where(r => r.VenueId == venueId)
            .Select(r => r.Rating)
            .ToListAsync(cancellationToken);

        return ratings.Count == 0 ? 0 : (decimal)ratings.Average();
    }

    public async Task<int> GetReviewCountAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        var venueId = await _context.FootballFields
            .Where(f => f.Id == fieldId)
            .Select(f => (Guid?)f.VenueId)
            .FirstOrDefaultAsync(cancellationToken);

        return venueId.HasValue
            ? await _dbSet.CountAsync(r => r.VenueId == venueId.Value, cancellationToken)
            : 0;
    }

    public override async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Venue)
            .FirstOrDefaultAsync(r => r.ReviewId == id, cancellationToken);
    }
}
