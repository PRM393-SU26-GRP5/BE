using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

public class AmenityRepository : Repository<Amenity>, IAmenityRepository
{
    public AmenityRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Amenity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }
}

