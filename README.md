# CourtManager - Football Field Booking Management System

A production-grade, highly scalable backend API for managing football field bookings, built with **.NET 10**, **Clean Architecture**, and **CQRS (Command Query Responsibility Segregation) pattern** using **MediatR** and **EF Core Code-First**.

---

## 1. Project Overview
* **Project Name**: CourtManager
* **Business Purpose**: Streamlines the process of discovering, booking, and managing football fields, coordinating between field owners (Managers) and players (Customers/Players), and providing real-time notification/chat channels.
* **Main Problem Solved**: Eliminates manual scheduling errors, double bookings, and communication gaps between sports facility owners and players by providing atomic booking slots, transaction tracking, and interactive feedback.
* **Target Users**: 
  * **System Admins**: System health, roles, and global configurations.
  * **Court Managers**: Manage fields, upload images, set time slots, view reservations, and chat with customers.
  * **Players**: Find fields, book slots, complete transactions, rate facilities, and receive chat assistance.
* **Current Development Status**: Core database schema refactoring completed. API build compiles with 0 errors and warnings. Database successfully migrated, populated, and fully seeded with sample data in SQL Server.
* **Key Features**: 
  * Domain-driven database design mapping real-world sport centers.
  * Role-Based Access Control (RBAC) with Identity framework and JWT authorization.
  * Atomic `BookingItem` reservations bound to pre-scheduled `TimeSlot` entities.
  * Internal `ChatRoom` and `Message` communication between customers and hosts with SQL Server multiple cascade path safety configurations.
  * Review, Rating, and Notification systems.
* **Non-Goals**: Managing equipment rentals (e.g., bibs, balls), tournament bracket scheduling, or direct SMS gateway integrations (all out of scope for the current phase).

---

## 2. Business Domain
* **Domain Description**: The domain centers around sports facility logistics, primarily managing physical football fields, scheduling their operating hours through atomic blocks (`TimeSlot`), and executing transactional reservations (`Booking`).
* **Core Entities**: 
  * `User`: Extends `IdentityUser`. Core actor playing Roles of Admin, Manager, or Player.
  * `FootballField`: A physical sports facility managed by a court manager.
  * `TimeSlot`: A scheduled calendar block (e.g., 1 hour) linked to a field.
  * `Booking`: The overarching reservation record for a player.
  * `BookingItem`: Individual slot bookings under a single transaction.
  * `Payment`: Financial transaction ledger bound to a Booking.
* **Business Workflows**:
  1. **Field Scheduling**: Manager registers a `FootballField` and generates multiple available `TimeSlots`.
  2. **Booking Reservation**: Player selects multiple `TimeSlots`, creating a single `Booking` holding multiple `BookingItems`. The related `TimeSlots` status changes to `Booked` or `Blocked`.
  3. **Payment Processing**: Player completes a transaction, creating a `Payment` record and updating `BookingStatus` to `Confirmed`.
  4. **Feedback & Review**: Once a booking is completed, players rate their experience via `Reviews`.
* **Important Business Rules**:
  * No two booking items can reserve the same `TimeSlot` concurrently.
  * Booking duration must be at least 30 minutes and cannot exceed 24 hours.
  * Users can only review fields they have interacted with or booked.
  * System administrators can control all fields; managers can only edit fields they own (`OwnerId`).

---

## 3. Technology Stack
* **Languages**: C# 13 (.NET 10)
* **Frameworks**: ASP.NET Core 10.0, MediatR (v14.1.0), FluentValidation (v12.1.1)
* **Databases**: Microsoft SQL Server
* **ORMs**: Entity Framework Core 10.0 (Code-First)
* **Authentication**: ASP.NET Core Identity, JWT Bearer Tokens
* **Mappings**: AutoMapper (v16.1.1)
* **Dev Tools**: EF Core CLI tools (`dotnet-ef`), Swagger OpenAPI UI

---

## 4. Architecture Overview
* **Architecture Style**: Clean Architecture with separation of concerns.
* **Layered Structure & Dependency Flow**:
  ```
  [ APIs (Presentation) ] ──> [ Application ] ──> [ Domain ]
            │                        │
            ▼                        ▼
  [ Infrastructure (Data/Repos) ] ───┘
  ```
* **Dependency Rule**: Outer layers can depend on inner layers, but inner layers (Domain) MUST NOT depend on outer layers.
* **Design Principles**: DDD (Domain-Driven Design) sub-domains, CQRS for separating reads and writes, and SOLID principles.
* **Patterns Used**: Repository Pattern (generic and specific), Mediator (CQRS), Fluent Configuration API, Options Pattern.

---

## 5. Solution Structure

```
CourtManager/
├── CourtManager.slnx                         # Solution file
├── DATABASE_MIGRATION_GUIDE.md               # Schema migration scope & instructions
├── README.md                                 # This AI Canonical Context Document
│
├── CourtManager.Domain/                      # Core Domain Layer (Entities & Interfaces)
│   ├── Entities/
│   │   ├── User.cs                           # Custom Identity user
│   │   ├── Role.cs                           # Custom Identity role
│   │   ├── UserRole.cs                       # Custom Identity user-role junction
│   │   ├── FootballField.cs                  # Replacing legacy Court entity
│   │   ├── FieldImage.cs                     # Football field images
│   │   ├── TimeSlot.cs                       # Scheduled blocks of fields
│   │   ├── Booking.cs                        # Head transaction
│   │   ├── BookingItem.cs                    # Individual slot line item
│   │   ├── Payment.cs                        # Payment records
│   │   ├── ChatRoom.cs                       # Communication room
│   │   ├── Message.cs                        # Chat messages
│   │   ├── Notification.cs                   # User notifications
│   │   └── Review.cs                         # Field reviews
│   └── Interfaces/
│       ├── IRepository.cs                    # Generic repository
│       ├── IUserRepository.cs                # Specific user queries
│       ├── IFootballFieldRepository.cs        # Replaced ICourtRepository
│       ├── IBookingRepository.cs             # Booking verification queries
│       └── IPaymentRepository.cs             # Payment queries
│
├── CourtManager.Application/                 # CQRS Commands, Queries & DTOs
│   ├── Features/
│   │   ├── Auth/Commands/                    # Register, Login, Refresh, Logout, Forgot, Reset
│   │   └── Bookings/
│   │       ├── Commands/                     # CreateBookingCommand & Validators
│   │       └── Queries/                      # GetBookingByIdQuery
│   ├── DTOs/
│   │   ├── AuthDto.cs                        # Register, Login, Token, UserAuth DTOs
│   │   ├── UserDto.cs                        # Public user profile
│   │   ├── FootballFieldDto.cs               # Replaced CourtDto
│   │   ├── BookingDto.cs                     # Booking properties
│   │   └── PaymentDto.cs                     # Payment properties
│   ├── Mappings/
│   │   └── MappingProfile.cs                 # AutoMapper configurations
│   ├── Exceptions/
│   │   ├── NotFoundException.cs              # 404 domain handler
│   │   └── ValidationException.cs            # 400 validator handler
│   └── ApplicationServiceExtensions.cs       # AutoMapper, MediatR, Validation DI setup
│
├── CourtManager.Infrastructure/              # EF Core Context, Fluent Configs & Repos
│   ├── Data/                                 # Fluent Configurations
│   │   ├── UserConfiguration.cs
│   │   ├── FootballFieldConfiguration.cs
│   │   ├── FieldImageConfiguration.cs
│   │   ├── TimeSlotConfiguration.cs
│   │   ├── BookingConfiguration.cs
│   │   ├── BookingItemConfiguration.cs
│   │   ├── PaymentConfiguration.cs
│   │   ├── ChatRoomConfiguration.cs
│   │   ├── MessageConfiguration.cs
│   │   ├── NotificationConfiguration.cs
│   │   ├── ReviewConfiguration.cs
│   │   ├── RoleConfiguration.cs
│   │   └── UserRoleConfiguration.cs
│   ├── Migrations/                           # EF Core generated migrations
│   ├── Repositories/                         # Repository implementations
│   │   ├── Repository.cs
│   │   ├── UserRepository.cs
│   │   ├── FootballFieldRepository.cs        # Replaced CourtRepository
│   │   ├── BookingRepository.cs
│   │   └── PaymentRepository.cs
│   ├── ApplicationDbContext.cs               # Database context and seed logic
│   └── InfrastructureServiceExtensions.cs    # EF, Identity, and Repositories DI setup
│
└── CourtManager.APIs/                        # Controller & Presentation Layer
    ├── Controllers/
    │   ├── AuthController.cs                 # Authentication & profile endpoints
    │   ├── BookingsController.cs             # Booking registration and queries
    │   └── AdminController.cs                # System administrative dashboard
    ├── Middleware/
    │   └── GlobalExceptionHandlingMiddleware.cs
    ├── Configuration/
    │   └── JwtSettings.cs                    # Mapping Jwt settings options
    ├── Program.cs                            # Application startup and service bootstrapping
    └── appsettings.json                      # Development/Production connection strings
```

---

## 6. Project Responsibilities

### `CourtManager.Domain`
* **Purpose**: Houses core business logic, entities, value objects, and repository contracts.
* **Dependencies**: None (pure .NET standard / no third-party ORMs).
* **May Contain**: Entity classes, domain events, domain-specific validation, interfaces.
* **Must Not Contain**: DTOs, controllers, EF DbContext, external libraries (like AutoMapper or MediatR).

### `CourtManager.Application`
* **Purpose**: Coordinates business use cases through CQRS command and query handlers.
* **Dependencies**: `CourtManager.Domain`.
* **May Contain**: Commands, Queries, Request Handlers, Validators, DTOs, Mapping profiles.
* **Must Not Contain**: HTTP request context, database connection code, raw SQL.

### `CourtManager.Infrastructure`
* **Purpose**: Implementation of database schemas, repository queries, and identity management.
* **Dependencies**: `CourtManager.Domain` and `CourtManager.Application`.
* **May Contain**: `DbContext`, Fluent Configuration classes, repository concrete implementations, migrations.
* **Must Not Contain**: Use cases, API controller routes, UI presentation logic.

### `CourtManager.APIs`
* **Purpose**: API controllers, request routing, middleware, Swagger configurations, and environment configuration.
* **Dependencies**: `CourtManager.Application` and `CourtManager.Infrastructure`.
* **May Contain**: Controllers, Middlewares, DI wiring bootstrap (`Program.cs`), `appsettings.json`.
* **Must Not Contain**: Database Fluent Configurations or raw business calculations.

---

## 7. Layer Responsibilities

* **API/Presentation**: Captures incoming HTTP requests, validates auth headers, passes requests into MediatR pipeline, maps domain exceptions to appropriate HTTP Status codes (`400`, `401`, `403`, `404`, `500`).
* **Application**: Performs input validation via FluentValidation, handles mapping between DTOs and Entities using AutoMapper, executes transactional unit of work, and orchestrates commands and queries.
* **Domain**: Expresses entities, aggregates, relationships, unique state invariants, and contract signatures.
* **Infrastructure**: Controls database transactions, manages SQL Server indexes, implements repositories, and seeds security identity roles.

---

## 8. Domain Model

### User
* **Purpose**: Represents registered profiles (Admins, Managers, and Players).
* **Properties**: `FullName`, `Phone`, `AvatarUrl`, `LoyaltyPoints`, `IsActive`, standard `IdentityUser` fields.
* **Relationships**:
  * Owns multiple `FootballFields` (`OwnedFields`).
  * Initiates multiple `Bookings` (`Bookings`).
  * Initiates multiple `Reviews` (`Reviews`).
  * Associated with multiple `ChatRooms` as `CustomerChatRooms` or `HostChatRooms`.
  * Has multiple `Notifications` (`Notifications`).

### FootballField
* **Purpose**: Represents physical sports venues.
* **Properties**: `OwnerId`, `FieldName`, `Description`, `FieldType`, `Location`, `Latitude`, `Longitude`, `PricePerHour`, `IsActive`.
* **Relationships**:
  * Belongs to an Owner `User` (`Owner` - `DeleteBehavior.Restrict`).
  * Holds many `FieldImages` (`Images` - `DeleteBehavior.Cascade`).
  * Holds many `TimeSlots` (`Slots` - `DeleteBehavior.Cascade`).
  * Has many `Reviews` (`Reviews` - `DeleteBehavior.Restrict`).

### TimeSlot
* **Purpose**: Scheduled calendar slots of a field.
* **Properties**: `FieldId`, `StartTime`, `EndTime`, `SlotStatus` (`Available`, `Booked`, `Blocked`).
* **Relationships**:
  * Belongs to a `FootballField` (`Field` - `DeleteBehavior.Cascade`).
  * Referenced by multiple `BookingItems` (`BookingItems` - `DeleteBehavior.Restrict`).

### Booking
* **Purpose**: Reservation transaction record.
* **Properties**: `UserId`, `FieldId`, `StartTime`, `EndTime`, `TotalPrice`, `BookingStatus` (`Pending`, `Confirmed`, `Completed`, `Cancelled`), `Note`.
* **Relationships**:
  * Placed by a `User` (`User` - `DeleteBehavior.Restrict`).
  * Reserved for a `FootballField` (`Field` - `DeleteBehavior.Restrict`).
  * Contains multiple `BookingItems` (`BookingItems` - `DeleteBehavior.Cascade`).
  * Links to multiple `Payments` (`Payments` - `DeleteBehavior.Cascade`).

### BookingItem
* **Purpose**: Junction line item mapping a Booking to a booked TimeSlot.
* **Properties**: `BookingId`, `SlotId`, `Price`.
* **Relationships**:
  * Linked to a `Booking` (`Booking` - `DeleteBehavior.Cascade`).
  * Maps to a specific `TimeSlot` (`Slot` - `DeleteBehavior.Restrict`).

### Payment
* **Purpose**: Ledger record tracking cash or online payments.
* **Properties**: `BookingId`, `Amount`, `PaymentStatus` (`Pending`, `Completed`, `Failed`, `Refunded`), `PaymentMethod` (`CreditCard`, `DebitCard`, `BankTransfer`, `Cash`, `Wallet`), `PaymentType` (`Online`, `Offline`), `TransactionCode` (Unique), `PaidAt`.
* **Relationships**:
  * Reference to one `Booking` (`Booking` - `DeleteBehavior.Cascade`).

### ChatRoom
* **Purpose**: Help desk channel between host and customer.
* **Properties**: `CustomerId`, `HostId`, `CreatedAt`.
* **Relationships**:
  * Linked to customer `User` (`Customer` - `DeleteBehavior.Restrict` *SQL Server Multiple Cascade Path Safety*).
  * Linked to host `User` (`Host` - `DeleteBehavior.Restrict` *SQL Server Multiple Cascade Path Safety*).
  * Holds many `Messages` (`Messages` - `DeleteBehavior.Cascade`).

### Message
* **Purpose**: Live text communication.
* **Properties**: `RoomId`, `SenderId`, `MessageText`, `SentAt`.
* **Relationships**:
  * Belongs to `ChatRoom` (`Room` - `DeleteBehavior.Cascade`).
  * Sent by `User` (`Sender` - `DeleteBehavior.Restrict` *SQL Server Multiple Cascade Path Safety*).

---

## 9. Database Design

* **Database Engine**: Microsoft SQL Server.
* **Schema & Naming Conventions**: Tables and fields map directly to C# PascalCase properties.
* **Primary Keys**: `Guid` (C# `uniqueidentifier` in SQL Server).
* **Foreign Keys**: Configured explicitly in Fluent API using `HasForeignKey(...)`.
* **Delete Behavior (Critical to SQL Server Cascade Path Safety)**:
  * Relationships with potential cycle paths (e.g., `ChatRoom` referencing `User` twice as `Customer` and `Host`, `Message` referencing `User` as `Sender` while cascade paths also go through `ChatRoom`, `Reviews` referencing `User` and `Field`) MUST use **`DeleteBehavior.Restrict`** or **`DeleteBehavior.NoAction`**.
  * Direct 1-N compositions (e.g., `Booking` $\rightarrow$ `BookingItem`, `FootballField` $\rightarrow$ `TimeSlot`) use `DeleteBehavior.Cascade`.
* **Audit Fields**: `CreatedAt` (`datetime2`, default SQL `GETUTCDATE()`), `UpdatedAt` (`datetime2`, nullable).
* **Indexes**: 
  * Unique: `Users.Email`, `Payments.TransactionCode`, composite `Reviews(UserId, FieldId)`.
  * Performance Composite Indexes: `Bookings(FieldId, StartTime, EndTime)`, `TimeSlots(FieldId, StartTime, EndTime)`, `Messages(RoomId, SentAt)`, `Notifications(UserId, CreatedAt)`.

---

## 10. API Design Standards

* **REST Principles**: Strict adherence to HTTP Methods (`GET` for reads, `POST` for creations, `PUT` for overrides, `DELETE` for removals).
* **Route Naming**: Lowercase plural routes, e.g., `api/auth/register`, `api/bookings`, `api/bookings/{id}`.
* **DTO Patterns**: Entities must never be returned directly. Requests use command payloads; responses are returned mapped into `*Dto` classes.
* **Validation**: Request models are validated using `FluentValidation` before reaching the execution handlers.
* **Error Handling**: Captured by global middleware returning structured JSON format:
  ```json
  {
    "status": 400,
    "title": "Validation Error",
    "detail": "Field ID is required."
  }
  ```
* **Authentication**: Handled via JWT bearer authorization header: `Authorization: Bearer <token>`.

---

## 11. Coding Standards

* **Naming Conventions**: 
  * C# classes, interfaces, methods, and configurations: `PascalCase`.
  * Interface prefixes: `I` (e.g., `IUserRepository`).
  * Private fields: `_camelCase` with an underscore (e.g., `_userRepository`).
* **File Naming**: Class name matches file name exactly (e.g., `RegisterCommandHandler.cs`).
* **Nullability**: Enabled globally (`<Nullable>enable</Nullable>`). All references must declare explicitly if nullable (`?`).
* **Async Patterns**: Every IO-bound database call must be asynchronous (`async` and `await`) and accept `CancellationToken`.

---

## 12. Design Patterns Used

* **Repository Pattern**: `Repository<T>` handles default CRUD operations. Specific repositories (e.g., `BookingRepository`) implement customized complex query filters.
* **CQRS (MediatR)**: Use cases are divided into separate transactional write `Commands` (e.g., `CreateBookingCommand`) and read `Queries` (e.g., `GetBookingByIdQuery`).
* **Fluent API Configuration**: Table constraints, default SQL values, indexes, and delete cascades are kept clean and separated from domain models inside `Infrastructure/Data/*Configuration.cs` classes.

---

## 13. Dependency Injection Rules

* **Lifetimes**: 
  * DbContext: `Scoped`.
  * Repositories: `Scoped`.
  * Services (e.g., `JwtTokenService`): `Scoped`.
  * MediatR Request Handlers / Validators: Automatically scanned and registered as `Transient` / `Scoped` by framework extensions.
* **Registration Pattern**: Registered inside layer extension files:
  * `CourtManager.Application` $\rightarrow$ `ApplicationServiceExtensions.cs`
  * `CourtManager.Infrastructure` $\rightarrow$ `InfrastructureServiceExtensions.cs`

---

## 14. Configuration Management

* **Settings File**: `appsettings.json` and `appsettings.Development.json` house environment configurations.
* **JWT Token Security**: Bound strongly at startup via `JwtSettings` options mapping:
  ```json
  "JwtSettings": {
    "Secret": "your-super-secret-key-that-is-at-least-32-characters-long-for-security-purposes",
    "Issuer": "CourtManager",
    "Audience": "CourtManagerApi",
    "AccessTokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
  ```
* **Database Connection**: Bound under `ConnectionStrings.DefaultConnection`.

---

## 15. Security Architecture

* **Authentication**: Core Identity authentication. JWT Tokens contain basic identity claims:
  * `ClaimTypes.NameIdentifier` $\rightarrow$ User ID
  * `ClaimTypes.Email` $\rightarrow$ Email address
  * `ClaimTypes.Name` $\rightarrow$ FullName
  * `ClaimTypes.Role` $\rightarrow$ Assigned roles (Admin, Manager, Player)
* **Token Protection**: JWT Access Token expires in 60 minutes. Secure Refresh Token is stored hashed in database for session renewal.
* **Password Hashing**: Identity uses PBKDF2 hashing standard automatically.

---

## 16. Validation Strategy

* **Input Data Validation**: Validated using `FluentValidation` before MediatR handler executes. Rules include format patterns (Regex for phone numbers, email validation), string constraints (maxLength), and date constraints (StartTime must be in the future).
* **Business Invariant Validation**: Handled within CQRS handlers (e.g., checking if user exists, verifying field availability).
* **Database Level Constraints**: Backed up by unique indexes (TransactionCode, User emails), required properties, and string length limits.

---

## 17. Error Handling Strategy

* **Global Exception Handling**: Centralized in [GlobalExceptionHandlingMiddleware.cs](file:///d:/GitHub/prm393/CourtManager.APIs/Middleware/GlobalExceptionHandlingMiddleware.cs).
* **Custom Exception Types**:
  * `NotFoundException`: Handled as HTTP `404 Not Found`.
  * `ValidationException`: Handled as HTTP `400 Bad Request` containing detailed validation error dictionary.
  * System Exceptions: Handled as HTTP `500 Internal Server Error`.

---

## 18. Logging and Monitoring

* **Logging Framework**: Structured Microsoft Extension Logging.
* **Logging Scopes**: Logs key business actions (e.g., `"Creating booking for User: {UserId}, Field: {FieldId}"`, `"Booking created successfully with ID: {BookingId}"`).
* **Health Checks**: Custom health endpoint at `api/bookings/health` and `api/auth/health` checks API responsiveness.

---

## 19. Testing Strategy

* **Unit Tests**: Targets Domain core logic, CQRS Commands/Queries, and Fluent Validation rules.
* **Mocking Approach**: Use of mocking frameworks (e.g., `Moq` or `NSubstitute`) to isolate repository data calls during unit test execution.

---

## 20. Build and Run Instructions

### Prerequisites
* **.NET 10 SDK**
* **Local MS SQL Server** instance or LocalDB.

### Steps to Run

1. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```
2. **Build Solution**:
   ```bash
   dotnet build
   ```
3. **Execute SQL Database Schema Migration**:
   ```bash
   dotnet ef database update --project CourtManager.Infrastructure --startup-project CourtManager.APIs
   ```
4. **Run Web API Hosting**:
   ```bash
   cd CourtManager.APIs
   dotnet run
   ```
5. **Interactive Testing via Swagger**:
   Open browser at `http://localhost:5000/swagger` or `https://localhost:7001/swagger`.

---

## 21. Seeding Data (For API Testing & Development)

The database includes **10 pre-seeded accounts** spanning all core system Roles. All profiles share a uniform password hash.

* **Uniform Login Password:** `Password@123`

### Seeded Profiles Ledger

| Role | Email Account | FullName | Phone Number |
|:---|:---|:---|:---|
| **Admin** | `admin1@court.com` | System Admin1 | `0900000001` |
| **Admin** | `admin2@court.com` | System Admin2 | `0900000002` |
| **Manager** | `manager1@court.com` | Court Manager1 | `0900000003` |
| **Manager** | `manager2@court.com` | Court Manager2 | `0900000004` |
| **Manager** | `manager3@court.com` | Court Manager3 | `0900000005` |
| **Player** | `player1@court.com` | Pro Player1 | `0900000006` |
| **Player** | `player2@court.com` | Pro Player2 | `0900000007` |
| **Player** | `player3@court.com` | Casual Player3 | `0900000008` |
| **Player** | `player4@court.com` | Casual Player4 | `0900000009` |
| **Player** | `player5@court.com` | Newbie Player5 | `0900000010` |

---

## 22. Performance Considerations
* **Caching**: Suggested for public configurations or inactive master lists.
* **No-Tracking Queries**: Use `AsNoTracking()` in EF Core for read-only query lookups to improve memory and throughput performance.
* **Indexes**: Explicit composite indexes on `Bookings` and `TimeSlots` search paths prevent expensive full-table SQL scans.

---

## 23. AI Coding Instructions (CRITICAL RULES)

> [!IMPORTANT]
> Any AI Assistant modifying this codebase must adhere strictly to the following canonical directives.

* **Never Bypass Layers**: Never reference `DbContext` directly inside Controller files. Always pass actions through MediatR pipeline.
* **Delete Behavior Invariants**: When introducing new relational foreign keys, analyze the graph for multiple cascade paths. **SQL Server will reject multiple cascade paths**. Use `DeleteBehavior.Restrict` for optional/many-to-many routes or composite user associations.
* **Naming Alignment**:
  * If the business relates to sports court properties, map them to **`FootballField`** (class/entity) and **`FootballFieldDto`** (DTO). **Never introduce new structures using the legacy "Court" name**.
  * Specific repository classes must align with their entities (e.g., `IFootballFieldRepository` maps to `FootballFieldRepository`).
* **Fluent API Location**: DB constraints must always be configured inside `CourtManager.Infrastructure/Data/*Configuration.cs`. Do not pollute Entity models with validation attributes.
* **DI Registration Consistency**: When creating a new repository or service interface, register it inside the appropriate `*ServiceExtensions.cs` file immediately.
* **Input vs Business Validation**: Format verification belongs in Application Validators (`*CommandValidator.cs`). State checking (e.g., double booking, presence validations) belongs inside the `*CommandHandler.cs`.
* **Async Protocol**: All EF Core methods MUST be called asynchronously (use `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`) and accept `cancellationToken` parameter.

---

## 24. Quick Context Summary for AI Models
* **Architecture Style**: Clean Architecture (Domain $\leftarrow$ Application $\leftarrow$ Infrastructure $\leftarrow$ APIs).
* **Pattern Core**: CQRS + MediatR. Command handlers execute business mutations, Queries execute read-only mappings.
* **Database Strategy**: Code-First with EF Core 10.0, SQL Server.
* **Identity Protocol**: ASP.NET Identity mapping custom `User`, `Role`, `UserRole` models. JWT authorization mechanism.
* **Critical Entities**: `User`, `FootballField`, `TimeSlot`, `Booking`, `BookingItem`, `Payment`, `ChatRoom`, `Message`, `Notification`, `Review`.
* **Key Command Example**: `CreateBookingCommand` $\rightarrow$ validated by `CreateBookingCommandValidator` $\rightarrow$ resolved by `CreateBookingCommandHandler` $\rightarrow$ returns `BookingDto`.
