using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Court entity with specific queries.
/// </summary>
public interface ICourtRepository : IRepository<Court>
{
    Task<IEnumerable<Court>> GetAvailableCourtsAsync(CancellationToken cancellationToken = default);
    Task<Court?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
