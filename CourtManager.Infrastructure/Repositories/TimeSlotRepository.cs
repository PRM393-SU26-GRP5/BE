using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using CourtManager.Domain.Enums;

namespace CourtManager.Infrastructure.Repositories;

public class TimeSlotRepository : Repository<TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid fieldId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Kind == DateTimeKind.Utc
            ? date.Date
            : DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var nextDay = startOfDay.AddDays(1);

        return await _dbSet
            .Include(s => s.Field)
            .Where(s => s.FieldId == fieldId
                && s.StartTime >= startOfDay
                && s.StartTime < nextDay
                && s.StartTime >= DateTime.UtcNow
                && (s.SlotStatus == SlotStatus.Available || (s.SlotStatus == SlotStatus.Locked && s.LockedUntil.HasValue && s.LockedUntil.Value <= DateTime.UtcNow)))
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetSlotsByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Field)
            .Where(s => s.FieldId == fieldId)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateSlotStatusAsync(Guid slotId, string status, CancellationToken cancellationToken = default)
    {
        var slot = await _dbSet.FirstOrDefaultAsync(s => s.SlotId == slotId, cancellationToken);
        if (slot == null)
        {
            return;
        }

        if (Enum.TryParse<SlotStatus>(status, true, out var parsedStatus))
        {
            slot.SlotStatus = parsedStatus;
            slot.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SlotStatus == SlotStatus.Locked)
            .OrderBy(s => s.LockedUntil)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SlotStatus == SlotStatus.Locked && s.LockedUntil.HasValue && s.LockedUntil.Value <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task BatchUpdateSlotStatusAsync(IEnumerable<Guid> slotIds, string status, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<SlotStatus>(status, true, out var parsedStatus))
        {
            return;
        }

        var ids = slotIds.ToList();
        var slots = await _dbSet.Where(s => ids.Contains(s.SlotId)).ToListAsync(cancellationToken);
        foreach (var slot in slots)
        {
            slot.SlotStatus = parsedStatus;
            slot.UpdatedAt = DateTime.UtcNow;
        }
    }

    public override async Task<TimeSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Field)
                .ThenInclude(f => f!.Venue)
            .FirstOrDefaultAsync(s => s.SlotId == id, cancellationToken);
    }
}
