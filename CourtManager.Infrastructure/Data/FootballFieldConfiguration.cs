using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for FootballField entity using Fluent API.
/// </summary>
public class FootballFieldConfiguration : IEntityTypeConfiguration<FootballField>
{
    public void Configure(EntityTypeBuilder<FootballField> builder)
    {
        // Primary Key
        builder.HasKey(f => f.Id);

        // Properties
        builder.Property(f => f.Id)
            .ValueGeneratedNever();

        builder.Property(f => f.OwnerId)
            .IsRequired();

        builder.Property(f => f.FieldName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.FieldType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Location)
            .HasMaxLength(300);

        builder.Property(f => f.Latitude)
            .HasPrecision(10, 7);

        builder.Property(f => f.Longitude)
            .HasPrecision(10, 7);

        builder.Property(f => f.PricePerHour)
            .HasPrecision(10, 2);

        builder.Property(f => f.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(f => f.IsActive)
            .HasDefaultValue(true);

        // Soft delete query filter
        builder.HasQueryFilter(f => !f.IsDeleted);

        // Relationships
        builder.HasOne(f => f.Owner)
            .WithMany(u => u.OwnedFields)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.FieldImages)
            .WithOne(i => i.Field)
            .HasForeignKey(i => i.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.TimeSlots)
            .WithOne(s => s.Field)
            .HasForeignKey(s => s.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Reviews)
            .WithOne(r => r.Field)
            .HasForeignKey(r => r.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("FootballFields");
    }
}
