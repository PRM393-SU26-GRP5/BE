using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Infrastructure.Data;

namespace CourtManager.Infrastructure;

/// <summary>
/// Application DbContext for Code First approach with Entity Framework Core.
/// Configures entities, relationships, and database mappings using Fluent API.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // DbSets for entities
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueImage> VenueImages => Set<VenueImage>();
    public DbSet<FootballField> FootballFields => Set<FootballField>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<VenueAmenity> VenueAmenities => Set<VenueAmenity>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<BookingDiscount> BookingDiscounts => Set<BookingDiscount>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<Review> Reviews => Set<Review>();

    /// <summary>
    /// Configures the model and applies all entity configurations.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<UserRole>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VenueConfiguration());
        modelBuilder.ApplyConfiguration(new FootballFieldConfiguration());
        modelBuilder.ApplyConfiguration(new VenueImageConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new BookingItemConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new ChatRoomConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

        modelBuilder.Entity<BookingDiscount>().HasKey(bd => new { bd.BookingId, bd.DiscountId });
        modelBuilder.Entity<VenueAmenity>().HasKey(va => new { va.VenueId, va.AmenityId });
        modelBuilder.Entity<NotificationRecipient>().HasKey(nr => nr.RecipientId);
    }
}
