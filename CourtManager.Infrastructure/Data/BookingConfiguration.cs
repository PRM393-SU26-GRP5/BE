using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for Booking entity using Fluent API.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // Primary Key
        builder.HasKey(b => b.Id);

        // Properties
        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.FieldId)
            .IsRequired();

        builder.Property(b => b.StartTime)
            .IsRequired();

        builder.Property(b => b.EndTime)
            .IsRequired();

        builder.Property(b => b.TotalPrice)
            .HasPrecision(10, 2);

        builder.Property(b => b.BookingStatus)
            .IsRequired()
            .HasDefaultValue("Pending")
            .HasMaxLength(50);

        builder.Property(b => b.Note)
            .HasMaxLength(500);

        builder.Property(b => b.UpdatedAt)
            .IsRequired(false);

        builder.Property(b => b.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes for efficient querying
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.FieldId);
        builder.HasIndex(b => new { b.FieldId, b.StartTime, b.EndTime });

        // Foreign Keys
        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Field)
            .WithMany(f => f.Bookings)
            .HasForeignKey(b => b.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.BookingItems)
            .WithOne(bi => bi.Booking)
            .HasForeignKey(bi => bi.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Payments)
            .WithOne(p => p.Booking)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("Bookings");
    }
}
