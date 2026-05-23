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

        // Seed initial data (optional)
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds initial data into the database.
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Hardcoded password hash for "Password@123" to avoid EF Core dynamic model change warnings
        var defaultPasswordHash = "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==";

        // Role GUIDs from RoleConfiguration
        var adminRoleId = new Guid("10000000-0000-0000-0000-000000000001");
        var managerRoleId = new Guid("10000000-0000-0000-0000-000000000002");
        var playerRoleId = new Guid("10000000-0000-0000-0000-000000000003");

        var users = new List<User>();
        var userRoles = new List<UserRole>();

        var accountData = new[]
        {
            new { Id = new Guid("20000000-0000-0000-0000-000000000001"), Role = adminRoleId, FullName = "System Admin1", Email = "admin1@court.com", Phone = "0900000001" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000002"), Role = adminRoleId, FullName = "System Admin2", Email = "admin2@court.com", Phone = "0900000002" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000003"), Role = managerRoleId, FullName = "Court Manager1", Email = "manager1@court.com", Phone = "0900000003" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000004"), Role = managerRoleId, FullName = "Court Manager2", Email = "manager2@court.com", Phone = "0900000004" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000005"), Role = managerRoleId, FullName = "Court Manager3", Email = "manager3@court.com", Phone = "0900000005" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000006"), Role = playerRoleId, FullName = "Pro Player1", Email = "player1@court.com", Phone = "0900000006" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000007"), Role = playerRoleId, FullName = "Pro Player2", Email = "player2@court.com", Phone = "0900000007" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000008"), Role = playerRoleId, FullName = "Casual Player3", Email = "player3@court.com", Phone = "0900000008" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000009"), Role = playerRoleId, FullName = "Casual Player4", Email = "player4@court.com", Phone = "0900000009" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000010"), Role = playerRoleId, FullName = "Newbie Player5", Email = "player5@court.com", Phone = "0900000010" }
        };

        foreach (var data in accountData)
        {
            users.Add(new User
            {
                Id = data.Id,
                FullName = data.FullName,
                Phone = data.Phone,
                UserName = data.Email,
                NormalizedUserName = data.Email.ToUpper(),
                Email = data.Email,
                NormalizedEmail = data.Email.ToUpper(),
                PhoneNumber = data.Phone,
                PasswordHash = defaultPasswordHash,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                ConcurrencyStamp = data.Id.ToString(),
                SecurityStamp = data.Id.ToString()
            });

            userRoles.Add(new UserRole
            {
                UserId = data.Id,
                RoleId = data.Role,
                AssignedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }

        modelBuilder.Entity<User>().HasData(users);
        modelBuilder.Entity<UserRole>().HasData(userRoles);
    }
}
