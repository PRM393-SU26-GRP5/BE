using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for FieldImage entity operations.
/// </summary>
public interface IFieldImageRepository : IRepository<FieldImage>
{
    /// <summary>
    /// Gets all images for a specific football field.
    /// </summary>
    Task<IEnumerable<FieldImage>> GetImagesByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all images associated with a field.
    /// </summary>
    Task DeleteByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple images at once.
    /// </summary>
    Task AddMultipleAsync(
        IEnumerable<FieldImage> images,
        CancellationToken cancellationToken = default);
}
