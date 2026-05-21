using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Review entity.
/// Inherits from base Repository and implements IReviewRepository.
/// </summary>
public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetReviewsByFieldIdAsync(
        Guid fieldId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.FieldId == fieldId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetUserReviewAsync(
        Guid userId,
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.UserId == userId && r.FieldId == fieldId, cancellationToken);
    }

    public async Task<decimal> GetAverageRatingAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        var reviews = await _dbSet
            .Where(r => r.FieldId == fieldId)
            .ToListAsync(cancellationToken);

        if (reviews.Count == 0)
            return 0m;

        return (decimal)reviews.Average(r => r.Rating);
    }

    public async Task<int> GetReviewCountAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.FieldId == fieldId)
            .CountAsync(cancellationToken);
    }
}
