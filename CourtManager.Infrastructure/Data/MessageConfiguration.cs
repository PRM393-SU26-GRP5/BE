using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for Message entity using Fluent API.
/// </summary>
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        // Primary Key
        builder.HasKey(m => m.MessageId);

        // Properties
        builder.Property(m => m.RoomId)
            .IsRequired();

        builder.Property(m => m.SenderId)
            .IsRequired();

        builder.Property(m => m.MessageText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(m => m.SentAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Soft delete query filter
        builder.HasQueryFilter(m => !m.IsDeleted);

        // Relationships
        builder.HasOne(m => m.Room)
            .WithMany(r => r.Messages)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(m => new { m.RoomId, m.SentAt });

        // Table configuration
        builder.ToTable("Messages");
    }
}
