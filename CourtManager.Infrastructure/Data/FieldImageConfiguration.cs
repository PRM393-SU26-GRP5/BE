using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for FieldImage entity using Fluent API.
/// </summary>
public class FieldImageConfiguration : IEntityTypeConfiguration<FieldImage>
{
    public void Configure(EntityTypeBuilder<FieldImage> builder)
    {
        // Primary Key
        builder.HasKey(i => i.ImageId);

        // Properties
        builder.Property(i => i.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        // Soft delete query filter
        builder.HasQueryFilter(i => !i.IsDeleted);

        // Relationships
        builder.HasOne(i => i.Field)
            .WithMany(f => f.FieldImages)
            .HasForeignKey(i => i.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("FieldImages");
    }
}
