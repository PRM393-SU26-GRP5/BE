using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for Payment entity using Fluent API.
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.BookingId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .HasPrecision(10, 2);

        builder.Property(p => p.PaymentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PaymentMethod)
            .HasConversion<int>();

        builder.Property(p => p.PaymentStatus)
            .IsRequired()
            .HasDefaultValue("Pending")
            .HasMaxLength(50);

        builder.Property(p => p.TransactionCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PaidAt)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(p => p.BookingId);
        builder.HasIndex(p => p.TransactionCode)
            .IsUnique();

        // Foreign Keys
        builder.HasOne(p => p.Booking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("Payments");
    }
}
