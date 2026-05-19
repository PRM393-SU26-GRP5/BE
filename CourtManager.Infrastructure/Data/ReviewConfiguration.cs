using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for Review entity using Fluent API.
/// </summary>
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // Primary Key
        builder.HasKey(r => r.ReviewId);

        // Properties
        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.FieldId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Field)
            .WithMany(f => f.Reviews)
            .HasForeignKey(r => r.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes - unique review per user per field
        builder.HasIndex(r => new { r.UserId, r.FieldId })
            .IsUnique();

        // Table configuration
        builder.ToTable("Reviews");
    }
}
