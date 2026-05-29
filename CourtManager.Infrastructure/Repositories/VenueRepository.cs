using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

public class VenueRepository : Repository<Venue>, IVenueRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VenueRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }

    private IQueryable<Venue> BuildFilterQuery(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating)
    {
        var query = _dbContext.Venues
            .Include(v => v.Owner)
            .Include(v => v.FootballFields)
            .Include(v => v.Reviews)
            .Where(v => v.IsActive && !v.IsDeleted);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.ToLower();
            query = query.Where(v => v.VenueName.ToLower().Contains(search) || v.Address.ToLower().Contains(search));
        }

        if (priceMin.HasValue)
        {
            query = query.Where(v => v.FootballFields.Any() && v.FootballFields.Min(f => f.PricePerHour) >= priceMin.Value);
        }

        if (priceMax.HasValue)
        {
            query = query.Where(v => v.FootballFields.Any() && v.FootballFields.Min(f => f.PricePerHour) <= priceMax.Value);
        }

        if (minRating.HasValue)
        {
            query = query.Where(v => v.Reviews.Any() && v.Reviews.Average(r => r.Rating) >= minRating.Value);
        }

        return query;
    }

    public async Task<IEnumerable<Venue>> GetVenuesAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        int skip, 
        int take, 
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilterQuery(q, priceMin, priceMax, minRating);
        
        return await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilterQuery(q, priceMin, priceMax, minRating);
        return await query.CountAsync(cancellationToken);
    }
}
