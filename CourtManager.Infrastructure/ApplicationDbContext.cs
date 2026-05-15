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
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();

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
        modelBuilder.ApplyConfiguration(new CourtConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

        // Seed initial data (optional)
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds initial data into the database.
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Users - Use fixed GUIDs
        var userId1 = new Guid("20000000-0000-0000-0000-000000000001");
        var userId2 = new Guid("20000000-0000-0000-0000-000000000002");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = userId1,
                FirstName = "John",
                LastName = "Doe",
                UserName = "john.doe@example.com",
                NormalizedUserName = "JOHN.DOE@EXAMPLE.COM",
                Email = "john.doe@example.com",
                NormalizedEmail = "JOHN.DOE@EXAMPLE.COM",
                PhoneNumber = "+1234567890",
                PasswordHash = string.Empty,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                ConcurrencyStamp = "20000000-0000-0000-0000-000000000001",
                SecurityStamp = "20000000-0000-0000-0000-000000000001"
            },
            new User
            {
                Id = userId2,
                FirstName = "Jane",
                LastName = "Smith",
                UserName = "jane.smith@example.com",
                NormalizedUserName = "JANE.SMITH@EXAMPLE.COM",
                Email = "jane.smith@example.com",
                NormalizedEmail = "JANE.SMITH@EXAMPLE.COM",
                PhoneNumber = "+0987654321",
                PasswordHash = string.Empty,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                ConcurrencyStamp = "20000000-0000-0000-0000-000000000002",
                SecurityStamp = "20000000-0000-0000-0000-000000000002"
            }
        );

        // Seed Courts - Use fixed GUIDs
        var courtId1 = new Guid("30000000-0000-0000-0000-000000000001");
        var courtId2 = new Guid("30000000-0000-0000-0000-000000000002");

        modelBuilder.Entity<Court>().HasData(
            new Court
            {
                Id = courtId1,
                Name = "Court 1 - Basketball",
                Description = "Professional basketball court",
                Location = "Downtown Sports Complex",
                PricePerHour = 50.00m,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsAvailable = true
            },
            new Court
            {
                Id = courtId2,
                Name = "Court 2 - Tennis",
                Description = "Outdoor tennis court",
                Location = "Central Park",
                PricePerHour = 40.00m,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsAvailable = true
            }
        );
    }
}
