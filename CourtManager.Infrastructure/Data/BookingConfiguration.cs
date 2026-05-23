using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.UserId).IsRequired();
        builder.Property(b => b.TotalPrice).HasPrecision(10, 2);
        builder.Property(b => b.Note).HasMaxLength(500);
        builder.Property(b => b.BookingStatus).IsRequired().HasConversion<string>().HasDefaultValue(CourtManager.Domain.Enums.BookingStatus.Pending).HasMaxLength(50);
        builder.Property(b => b.UpdatedAt).IsRequired(false);
        builder.Property(b => b.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.HasIndex(b => b.UserId);
        builder.HasOne(b => b.User).WithMany(u => u.Bookings).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(b => b.BookingItems).WithOne(bi => bi.Booking).HasForeignKey(bi => bi.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(b => b.Payments).WithOne(p => p.Booking).HasForeignKey(p => p.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.ToTable("Bookings");
    }
}
