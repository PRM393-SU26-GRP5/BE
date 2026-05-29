using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

public interface IVenueRepository : IRepository<Venue>
{
    // We will add custom methods here if needed later (e.g. specialized GetVenuesAsync)
    Task<IEnumerable<Venue>> GetVenuesAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        int skip, 
        int take, 
        CancellationToken cancellationToken = default);

    Task<int> GetTotalCountAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        CancellationToken cancellationToken = default);
}
