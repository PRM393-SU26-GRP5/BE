using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for VenueImage entity using Fluent API.
/// </summary>
public class VenueImageConfiguration : IEntityTypeConfiguration<VenueImage>
{
    public void Configure(EntityTypeBuilder<VenueImage> builder)
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
        builder.HasOne(i => i.Venue)
            .WithMany(f => f.VenueImages)
            .HasForeignKey(i => i.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("VenueImages");
    }
}
