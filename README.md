# CourtManager - RESTful API Boilerplate

A comprehensive RESTful backend API for managing sports court bookings, built with **.NET 10**, **Clean Architecture**, and **CQRS pattern** (using **MediatR**).

## 📋 Project Overview

This project demonstrates enterprise-level architecture patterns and best practices for building scalable, maintainable .NET APIs:

- **Clean Architecture**: Separation of concerns with clear dependency flow
- **CQRS Pattern**: Command Query Responsibility Segregation using MediatR
- **Entity Framework Core**: Code First approach with Fluent API configurations
- **FluentValidation**: Comprehensive input validation
- **JWT Authentication**: Secure token-based authentication
- **Global Exception Handling**: Centralized error management
- **AutoMapper**: Object-to-object mapping
- **Swagger/OpenAPI**: Interactive API documentation

## 🏗️ Project Structure

```
CourtManager/
├── CourtManager.Domain/                    # Domain Layer (Entities & Interfaces)
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Court.cs
│   │   ├── Booking.cs
│   │   └── Payment.cs
│   └── Interfaces/
│       ├── IRepository.cs
│       ├── IUserRepository.cs
│       ├── ICourtRepository.cs
│       ├── IBookingRepository.cs
│       └── IPaymentRepository.cs
│
├── CourtManager.Application/               # Application Layer (CQRS & Business Logic)
│   ├── Features/
│   │   └── Bookings/
│   │       ├── Commands/
│   │       │   ├── CreateBookingCommand.cs
│   │       │   ├── CreateBookingCommandValidator.cs
│   │       │   └── CreateBookingCommandHandler.cs
│   │       └── Queries/
│   │           ├── GetBookingByIdQuery.cs
│   │           └── GetBookingByIdQueryHandler.cs
│   ├── DTOs/
│   │   ├── UserDto.cs
│   │   ├── CourtDto.cs
│   │   ├── BookingDto.cs
│   │   └── PaymentDto.cs
│   ├── Mappings/
│   │   └── MappingProfile.cs
│   ├── Exceptions/
│   │   ├── NotFoundException.cs
│   │   └── ValidationException.cs
│   └── ApplicationServiceExtensions.cs
│
├── CourtManager.Infrastructure/            # Infrastructure Layer (Data Access)
│   ├── Data/
│   │   ├── UserConfiguration.cs
│   │   ├── CourtConfiguration.cs
│   │   ├── BookingConfiguration.cs
│   │   ├── PaymentConfiguration.cs
│   │   └── [Migrations]/
│   ├── Repositories/
│   │   ├── Repository.cs
│   │   ├── CourtRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── BookingRepository.cs
│   │   └── PaymentRepository.cs
│   ├── ApplicationDbContext.cs
│   └── InfrastructureServiceExtensions.cs
│
└── CourtManager.APIs/                      # APIs Layer (Presentation)
    ├── Controllers/
    │   └── BookingsController.cs
    ├── Middleware/
    │   └── GlobalExceptionHandlingMiddleware.cs
    ├── Configuration/
    │   └── JwtSettings.cs
    ├── Program.cs
    ├── appsettings.json
    └── Properties/
        └── launchSettings.json
```

## 🚀 Getting Started

### Prerequisites

- **.NET 10 SDK** or later
- **SQL Server** (LocalDB or full instance)
- **Visual Studio 2022** or **VS Code** with C# extensions

### Installation

1. **Clone or extract the project**:
   ```bash
   cd d:\GitHub\prm393
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

## 💾 Database Setup & Migrations

### Using Entity Framework Core Migrations

This project uses **Code First approach** with EF Core migrations. Follow these steps to set up the database:

#### Step 1: Create Initial Migration

Navigate to the Infrastructure project directory and create the initial migration:

```bash
# From the solution root
cd CourtManager.Infrastructure
dotnet ef migrations add InitialCreate -s ../CourtManager.APIs
```

**Expected output:**
```
Build started...
Build succeeded.
An entity type 'Booking' cannot configure the foreign key because it has no properties marked with [PrimaryKey] or specified in OnModelCreating.
...
To undo this action, use 'ef migrations remove'
```

This creates a `Migrations` folder with timestamp-prefixed migration files.

#### Step 2: Update Database

Apply the migration to create the database schema:

```bash
# From the solution root (or Infrastructure project)
dotnet ef database update -s CourtManager.APIs
```

**What this command does:**
- Creates the database (if it doesn't exist)
- Creates all tables: Users, Courts, Bookings, Payments
- Creates indexes and relationships
- Seeds initial data (Users and Courts)

#### Step 3: Verify Database Creation

Check that the database was created by:
- **SQL Server Object Explorer** (in Visual Studio)
- **SSMS** (SQL Server Management Studio)
- Or query: `SELECT name FROM sys.databases WHERE name LIKE 'CourtManager%'`

### Alternative: EnsureCreated (Development Only)

If you don't want to use migrations, `Program.cs` includes:
```csharp
dbContext.Database.EnsureCreated();
```

This creates the database and schema automatically on first run (development only).

### Subsequent Migrations

When you make model changes:

```bash
# Create new migration
dotnet ef migrations add DescriptiveNameForYourChanges -s CourtManager.APIs

# Apply migration to database
dotnet ef database update -s CourtManager.APIs
```

## 🏃 Running the Application

### Option 1: Using dotnet CLI

```bash
# From the solution root
cd CourtManager.APIs
dotnet run
```

### Option 2: Using Visual Studio

1. Set `CourtManager.APIs` as the startup project
2. Press `F5` or click **Run**

### Option 3: Using VS Code

```bash
cd CourtManager.APIs
dotnet run --launch-profile https
```

**Application will be available at:**
- **Swagger UI**: `https://localhost:7000` (or `http://localhost:5125` for HTTP)
- **API Base URL**: `https://localhost:7000/api`
- **Health Check**: `https://localhost:7000/api/health`

## 📚 API Endpoints

### Public Endpoints (No Authentication Required)

```
GET  /api/health                    - Health check endpoint
GET  /swagger                        - Swagger UI documentation
```

### Protected Endpoints (JWT Authentication Required)

#### Bookings

```
POST   /api/bookings                - Create a new booking
GET    /api/bookings/{id}           - Get booking by ID
GET    /api/bookings/health         - Booking health check
```

## 🔐 Authentication

### JWT Token Configuration

JWT settings are configured in `appsettings.json`:

```json
"JwtSettings": {
    "Secret": "your-super-secret-key-that-is-at-least-32-characters-long",
    "Issuer": "CourtManager",
    "Audience": "CourtManagerApi",
    "ExpirationInMinutes": 60
}
```

### Generating JWT Tokens

To test protected endpoints, you need a valid JWT token. Example:

```csharp
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
var token = new JwtSecurityToken(
    issuer: jwtSettings.Issuer,
    audience: jwtSettings.Audience,
    expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
    signingCredentials: creds);
```

### Using Tokens in API Calls

Add the token to request headers:

```http
Authorization: Bearer <your_jwt_token>
```

## 📝 CQRS Pattern Example

### CreateBooking Flow

**1. Command** ([CreateBookingCommand.cs](CourtManager.Application/Features/Bookings/Commands/CreateBookingCommand.cs)):
```csharp
public class CreateBookingCommand : IRequest<BookingDto>
{
    public Guid UserId { get; set; }
    public Guid CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
```

**2. Validator** ([CreateBookingCommandValidator.cs](CourtManager.Application/Features/Bookings/Commands/CreateBookingCommandValidator.cs)):
- Validates user and court IDs
- Ensures start time is in the future
- Validates booking duration (30 min - 24 hours)

**3. Handler** ([CreateBookingCommandHandler.cs](CourtManager.Application/Features/Bookings/Commands/CreateBookingCommandHandler.cs)):
- Verifies user exists
- Verifies court exists
- Checks court availability
- Calculates total amount
- Creates and saves booking

**4. Controller** ([BookingsController.cs](CourtManager.APIs/Controllers/BookingsController.cs)):
```csharp
[HttpPost]
public async Task<ActionResult<BookingDto>> CreateBooking(
    CreateBookingCommand command, 
    CancellationToken cancellationToken)
{
    return CreatedAtAction(nameof(GetBookingById), 
        new { id = result.Id }, result);
}
```

## 🔄 Dependency Injection

Services are registered in `Program.cs`:

```csharp
// Application layer (MediatR, FluentValidation, AutoMapper)
builder.Services.AddApplicationServices();

// Infrastructure layer (DbContext, Repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(...);
```

## 📋 Entity Relationships

### Database Schema

```
Users (1) ──────────┐
                    │
                    ├──> Bookings (Many) ──────┐
                    │                           │
                    └────────────────────────────┤────> Payments (1)
                                                │
                                    Courts (1) ─┴───┐
                                                    │
                                              (1 to Many)
```

### Fluent API Configuration

All entity relationships are configured in:
- [UserConfiguration.cs](CourtManager.Infrastructure/Data/UserConfiguration.cs)
- [CourtConfiguration.cs](CourtManager.Infrastructure/Data/CourtConfiguration.cs)
- [BookingConfiguration.cs](CourtManager.Infrastructure/Data/BookingConfiguration.cs)
- [PaymentConfiguration.cs](CourtManager.Infrastructure/Data/PaymentConfiguration.cs)

## 🧪 Testing the API

### Using Swagger UI

1. Navigate to `https://localhost:7000`
2. Click "Authorize" button (top-right)
3. Enter: `Bearer <your_jwt_token>`
4. Test endpoints interactively

### Using cURL

```bash
# Create booking
curl -X POST https://localhost:7000/api/bookings \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "guid-here",
    "courtId": "guid-here",
    "startTime": "2025-06-01T10:00:00Z",
    "endTime": "2025-06-01T12:00:00Z"
  }'

# Get booking
curl -X GET https://localhost:7000/api/bookings/{id} \
  -H "Authorization: Bearer <token>"
```

### Using Postman

1. Create a new POST request to `https://localhost:7000/api/bookings`
2. Go to **Authorization** tab → Type: **Bearer Token** → Paste JWT
3. **Body** (JSON):
   ```json
   {
     "userId": "00000000-0000-0000-0000-000000000001",
     "courtId": "00000000-0000-0000-0000-000000000001",
     "startTime": "2025-06-01T10:00:00Z",
     "endTime": "2025-06-01T12:00:00Z"
   }
   ```
4. Send request

## 🛠️ Extending the Application

### Adding a New Feature

1. **Create Command** in `Application/Features/YourFeature/Commands/`
2. **Create Handler** implementing `IRequestHandler<YourCommand, TResponse>`
3. **Create Validator** extending `AbstractValidator<YourCommand>`
4. **Register in MediatR** (automatic via `AddApplicationServices()`)
5. **Create Controller** with endpoint
6. **Test via Swagger**

### Adding a New Entity

1. Create Entity in `Domain/Entities/`
2. Add DbSet in `ApplicationDbContext`
3. Create Configuration in `Infrastructure/Data/`
4. Create Repository Interface in `Domain/Interfaces/`
5. Create Repository Implementation in `Infrastructure/Repositories/`
6. Create Migration:
   ```bash
   dotnet ef migrations add Add{EntityName} -s CourtManager.APIs
   dotnet ef database update -s CourtManager.APIs
   ```

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CourtManagerDb;..."
  },
  "JwtSettings": {
    "Secret": "...",
    "Issuer": "CourtManager",
    "Audience": "CourtManagerApi",
    "ExpirationInMinutes": 60
  }
}
```

### Environment-Specific Settings

- `appsettings.Development.json` - Development settings
- `appsettings.Production.json` - Production settings (create as needed)

## 📦 NuGet Packages

- **MediatR** (14.1.0) - CQRS pattern implementation
- **FluentValidation** (12.1.1) - Input validation
- **AutoMapper** (16.1.1) - Object mapping
- **Entity Framework Core** (10.x) - ORM
- **Swashbuckle.AspNetCore** (10.1.7) - Swagger/OpenAPI
- **JWT Bearer** (10.0.x) - Authentication

## 🐛 Troubleshooting

### Migration Issues

**Error**: `The project 'X' doesn't compile with 'Y'`

**Solution**:
```bash
dotnet clean
dotnet build
dotnet ef migrations add YourMigration -s CourtManager.APIs
```

### Database Connection Issues

**Error**: `Cannot open database "CourtManagerDb"`

**Solution**:
1. Ensure SQL Server is running
2. Check `DefaultConnection` string in `appsettings.json`
3. For LocalDB: Ensure `(localdb)\mssqllocaldb` is available

### JWT Token Issues

**Error**: `401 Unauthorized`

**Solution**:
1. Ensure token is in correct format: `Authorization: Bearer <token>`
2. Verify token hasn't expired
3. Check JWT secret matches between token generation and validation

## 📚 Architecture Overview

### Clean Architecture Layers

```
┌─────────────────────────────────────────────┐
│         APIs (Presentation)                 │
│  - Controllers                              │
│  - Middleware                               │
│  - Configuration                            │
└────────────────┬────────────────────────────┘
                 │
┌─────────────────▼────────────────────────────┐
│     Application (Business Logic)             │
│  - Commands/Queries (CQRS)                   │
│  - Handlers                                  │
│  - Validators                                │
│  - DTOs                                      │
│  - Mappings                                  │
└────────────────┬────────────────────────────┘
                 │
┌─────────────────▼────────────────────────────┐
│       Domain (Core Business)                 │
│  - Entities                                  │
│  - Repository Interfaces                     │
│  - Enums                                     │
│  - Value Objects                             │
└────────────────┬────────────────────────────┘
                 │
┌─────────────────▼────────────────────────────┐
│    Infrastructure (Data Access)              │
│  - DbContext                                 │
│  - Entity Configurations                     │
│  - Repository Implementations                │
│  - Migrations                                │
└─────────────────────────────────────────────┘
```

## 📄 License

This project is provided as a boilerplate template for learning and development purposes.

## 💡 Best Practices Implemented

- ✅ **SOLID Principles**
- ✅ **Dependency Injection**
- ✅ **Repository Pattern**
- ✅ **CQRS Pattern**
- ✅ **Fluent Validation**
- ✅ **Async/Await**
- ✅ **Exception Handling**
- ✅ **Logging**
- ✅ **Security (JWT)**
- ✅ **API Documentation (Swagger)**
- ✅ **Code-First Migrations**
- ✅ **AutoMapper**

## 🤝 Support

For questions or issues, refer to:
- Official .NET Documentation: https://learn.microsoft.com/dotnet/
- MediatR: https://github.com/jbogard/MediatR
- Entity Framework Core: https://learn.microsoft.com/ef/core/
