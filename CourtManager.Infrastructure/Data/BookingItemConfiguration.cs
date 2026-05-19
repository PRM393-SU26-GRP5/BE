using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for BookingItem entity using Fluent API.
/// </summary>
public class BookingItemConfiguration : IEntityTypeConfiguration<BookingItem>
{
    public void Configure(EntityTypeBuilder<BookingItem> builder)
    {
        // Primary Key
        builder.HasKey(bi => bi.BookingItemId);

        // Properties
        builder.Property(bi => bi.BookingId)
            .IsRequired();

        builder.Property(bi => bi.SlotId)
            .IsRequired();

        builder.Property(bi => bi.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(bi => bi.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(bi => bi.Booking)
            .WithMany(b => b.BookingItems)
            .HasForeignKey(bi => bi.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bi => bi.Slot)
            .WithMany(s => s.BookingItems)
            .HasForeignKey(bi => bi.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(bi => new { bi.BookingId, bi.SlotId });

        // Table configuration
        builder.ToTable("BookingItems");
    }
}
