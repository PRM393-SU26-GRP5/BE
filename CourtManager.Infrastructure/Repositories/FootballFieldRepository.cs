using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for FootballField entity.
/// Inherits from base Repository and implements IFootballFieldRepository.
/// </summary>
public class FootballFieldRepository : Repository<FootballField>, IFootballFieldRepository
{
    public FootballFieldRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<FootballField>> GetAvailableFieldsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Venue)
            .Where(f => f.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<FootballField?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Venue)
            .FirstOrDefaultAsync(f => f.FieldName == name, cancellationToken);
    }

    public override async Task<FootballField?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Venue)
            .Include(f => f.TimeSlots)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<FootballField>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Venue)
            .Where(f => f.VenueId == venueId)
            .OrderBy(f => f.FieldName)
            .ToListAsync(cancellationToken);
    }
}
