using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for TimeSlot entity using Fluent API.
/// </summary>
public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        // Primary Key
        builder.HasKey(s => s.SlotId);

        // Properties
        builder.Property(s => s.FieldId)
            .IsRequired();

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.SlotStatus)
            .IsRequired()
            .HasDefaultValue("Available")
            .HasMaxLength(50);

        builder.Property(s => s.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Soft delete query filter
        builder.HasQueryFilter(s => !s.IsDeleted);

        // Relationships
        builder.HasOne(s => s.Field)
            .WithMany(f => f.TimeSlots)
            .HasForeignKey(s => s.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.BookingItems)
            .WithOne(bi => bi.Slot)
            .HasForeignKey(bi => bi.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => new { s.FieldId, s.StartTime, s.EndTime });

        // Table configuration
        builder.ToTable("TimeSlots");
    }
}
