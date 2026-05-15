using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Court entity.
/// Inherits from base Repository and implements ICourtRepository.
/// </summary>
public class CourtRepository : Repository<Court>, ICourtRepository
{
    public CourtRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Court>> GetAvailableCourtsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    public async Task<Court?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }
}
