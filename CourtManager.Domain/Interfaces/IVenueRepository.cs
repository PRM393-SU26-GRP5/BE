using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Interfaces;

public interface IVenueRepository : IRepository<Venue>
{
    Task<Venue?> GetDetailsAsync(Guid venueId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venue>> SearchAsync(
        string? query,
        FieldType? fieldType,
        decimal? minPrice,
        decimal? maxPrice,
        decimal? minRating,
        string? amenity,
        string? sortBy,
        decimal? lat,
        decimal? lng,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Venue>> GetNearbyAsync(decimal lat, decimal lng, decimal radiusKm, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venue>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
