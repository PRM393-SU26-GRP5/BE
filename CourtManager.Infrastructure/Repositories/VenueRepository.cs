using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

public class VenueRepository : Repository<Venue>, IVenueRepository
{
    public VenueRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetDetailsAsync(id, cancellationToken);
    }

    public async Task<Venue?> GetDetailsAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .FirstOrDefaultAsync(v => v.VenueId == venueId, cancellationToken);
    }

    public async Task<IEnumerable<Venue>> SearchAsync(
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
        CancellationToken cancellationToken = default)
    {
        var venues = IncludeDetails(_dbSet).Where(v => v.IsActive);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var keyword = query.Trim().ToLower();
            venues = venues.Where(v => v.VenueName.ToLower().Contains(keyword) || v.Address.ToLower().Contains(keyword));
        }

        if (fieldType.HasValue)
        {
            venues = venues.Where(v => v.FootballFields.Any(f => f.FieldType == fieldType && f.IsActive));
        }

        if (minPrice.HasValue)
        {
            venues = venues.Where(v => v.FootballFields.Any(f => f.IsActive && f.PricePerHour >= minPrice.Value));
        }

        if (maxPrice.HasValue)
        {
            venues = venues.Where(v => v.FootballFields.Any(f => f.IsActive && f.PricePerHour <= maxPrice.Value));
        }

        if (minRating.HasValue)
        {
            venues = venues.Where(v => v.Reviews.Any() && (decimal)v.Reviews.Average(r => r.Rating) >= minRating.Value);
        }

        if (!string.IsNullOrWhiteSpace(amenity))
        {
            var amenityKeyword = amenity.Trim().ToLower();
            venues = venues.Where(v => v.VenueAmenities.Any(va => va.Amenity != null && va.Amenity.Name.ToLower().Contains(amenityKeyword)));
        }

        var list = await venues.ToListAsync(cancellationToken);

        list = sortBy?.Trim().ToLowerInvariant() switch
        {
            "rating" or "rating_desc" => list.OrderByDescending(v => v.Reviews.Any() ? v.Reviews.Average(r => r.Rating) : 0).ToList(),
            "price" or "price_asc" => list.OrderBy(v => v.FootballFields.Where(f => f.IsActive).Select(f => (decimal?)f.PricePerHour).Min() ?? decimal.MaxValue).ToList(),
            "nearest" when lat.HasValue && lng.HasValue => list.OrderBy(v => DistanceKm(lat.Value, lng.Value, v.Latitude, v.Longitude)).ToList(),
            _ => list.OrderBy(v => v.VenueName).ToList()
        };

        return list
            .Skip((Math.Max(pageNumber, 1) - 1) * Math.Max(pageSize, 1))
            .Take(Math.Max(pageSize, 1))
            .ToList();
    }

    public async Task<IEnumerable<Venue>> GetNearbyAsync(decimal lat, decimal lng, decimal radiusKm, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var venues = await IncludeDetails(_dbSet)
            .Where(v => v.IsActive)
            .ToListAsync(cancellationToken);

        return venues
            .Select(v => new { Venue = v, Distance = DistanceKm(lat, lng, v.Latitude, v.Longitude) })
            .Where(v => v.Distance <= (double)radiusKm)
            .OrderBy(v => v.Distance)
            .Skip((Math.Max(pageNumber, 1) - 1) * Math.Max(pageSize, 1))
            .Take(Math.Max(pageSize, 1))
            .Select(v => v.Venue)
            .ToList();
    }

    public async Task<IEnumerable<Venue>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(v => v.OwnerId == ownerId)
            .OrderBy(v => v.VenueName)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Venue> IncludeDetails(IQueryable<Venue> query)
    {
        return query
            .Include(v => v.Owner)
            .Include(v => v.FootballFields)
            .Include(v => v.VenueImages)
            .Include(v => v.Reviews)
            .Include(v => v.VenueAmenities)
                .ThenInclude(va => va.Amenity);
    }

    private static double DistanceKm(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
    {
        const double earthRadiusKm = 6371;
        var dLat = ToRadians((double)(lat2 - lat1));
        var dLng = ToRadians((double)(lng2 - lng1));
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2))
            * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
