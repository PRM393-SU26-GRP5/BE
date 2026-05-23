using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetReviewsByFieldIdAsync(Guid fieldId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Review?> GetUserReviewAsync(Guid userId, Guid fieldId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<decimal> GetAverageRatingAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetReviewCountAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
