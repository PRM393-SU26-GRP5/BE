using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for FieldImage entity.
/// Inherits from base Repository and implements IFieldImageRepository.
/// </summary>
public class FieldImageRepository : Repository<FieldImage>, IFieldImageRepository
{
    public FieldImageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<FieldImage>> GetImagesByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.FieldId == fieldId)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        var images = await _dbSet
            .Where(i => i.FieldId == fieldId)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(images);
    }

    public async Task AddMultipleAsync(
        IEnumerable<FieldImage> images,
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(images, cancellationToken);
    }
}
