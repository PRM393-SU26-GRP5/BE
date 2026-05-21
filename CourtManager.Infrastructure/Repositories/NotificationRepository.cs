using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Notification entity.
/// Inherits from base Repository and implements INotificationRepository.
/// </summary>
public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(
        Guid userId,
        bool unreadOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdPaginatedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        bool unreadOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _dbSet
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId, cancellationToken);

        if (notification != null)
        {
            notification.IsRead = true;
            _dbSet.Update(notification);
        }
    }

    public async Task MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        _dbSet.UpdateRange(notifications);
    }

    public async Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync(cancellationToken);
    }
}
