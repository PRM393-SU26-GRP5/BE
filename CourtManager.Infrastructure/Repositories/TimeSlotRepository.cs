using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TimeSlot entity.
/// Inherits from base Repository and implements ITimeSlotRepository.
/// </summary>
public class TimeSlotRepository : Repository<TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(
        Guid fieldId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.FieldId == fieldId
                && s.StartTime.Date == date.Date
                && (s.SlotStatus == "Available" || s.SlotStatus == "Locked"))
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetSlotsByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.FieldId == fieldId)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateSlotStatusAsync(
        Guid slotId,
        string status,
        CancellationToken cancellationToken = default)
    {
        var slot = await _dbSet.FirstOrDefaultAsync(s => s.SlotId == slotId, cancellationToken);
        if (slot != null)
        {
            slot.SlotStatus = status;
            slot.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(slot);
        }
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SlotStatus == "Locked")
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsExpiredAsync(
        CancellationToken cancellationToken = default)
    {
        // Slots locked for more than 15 minutes are considered expired
        var expiryTime = DateTime.UtcNow.AddMinutes(-15);
        return await _dbSet
            .Where(s => s.SlotStatus == "Locked" && s.CreatedAt < expiryTime)
            .ToListAsync(cancellationToken);
    }

    public async Task BatchUpdateSlotStatusAsync(
        IEnumerable<Guid> slotIds,
        string status,
        CancellationToken cancellationToken = default)
    {
        var slots = await _dbSet
            .Where(s => slotIds.Contains(s.SlotId))
            .ToListAsync(cancellationToken);

        foreach (var slot in slots)
        {
            slot.SlotStatus = status;
            slot.UpdatedAt = DateTime.UtcNow;
        }

        _dbSet.UpdateRange(slots);
    }
}
