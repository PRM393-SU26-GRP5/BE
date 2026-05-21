using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Message entity.
/// Inherits from base Repository and implements IMessageRepository.
/// </summary>
public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Message>> GetMessagesByRoomIdAsync(
        Guid roomId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetLastMessageAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetMessageCountAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.RoomId == roomId)
            .CountAsync(cancellationToken);
    }
}
