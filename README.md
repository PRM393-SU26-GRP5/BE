# CourtManager - Football Field Booking Management System

A production-grade, highly scalable backend API for managing football field bookings, built with **.NET 10**, **Clean Architecture**, and **CQRS (Command Query Responsibility Segregation) pattern** using **MediatR** and **EF Core Code-First**.

---

## 1. Project Overview
* **Business Purpose**: Streamlines the process of discovering, booking, and managing football fields, coordinating between field owners (Managers) and players (Customers/Players).
* **Key Features**: 
  * Role-Based Access Control (RBAC) with Identity framework and JWT authorization.
  * Field & TimeSlot Management for Field Owners.
  * Atomic reservations and Payment processing.
  * Real-time Internal `ChatRoom` and `Message` communication between customers and hosts.
  * Review, Rating, and Notification systems.

---

## 2. Technology Stack
* **Framework**: .NET 10 (C# 13), ASP.NET Core 10.0
* **Architecture**: Clean Architecture & CQRS (MediatR)
* **Database**: Microsoft SQL Server & Entity Framework Core 10.0 (Code-First)
* **Authentication**: ASP.NET Core Identity & JWT Bearer Tokens
* **Libraries**: AutoMapper, FluentValidation

---

## 3. Build and Run Instructions

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

## 4. Seeding Data (For API Testing & Development)

The database includes pre-seeded accounts spanning all core system Roles. 

* **Uniform Login Password:** `Password@123`

### Seeded Profiles
| Role | Email Account | FullName | Phone Number |
|:---|:---|:---|:---|
| **Admin** | `admin1@court.com` | System Admin1 | `0900000001` |
| **Manager** | `manager1@court.com` | Court Manager1 | `0900000003` |
| **Player** | `player1@court.com` | Pro Player1 | `0900000006` |

*(See `ApplicationDbContext.cs` for the full list of 10 seeded users).*

---

## 5. AI Coding Instructions (CRITICAL RULES)

> [!IMPORTANT]
> Any AI Assistant modifying this codebase must adhere strictly to the following canonical directives.

* **Never Bypass Layers**: Never reference `DbContext` directly inside Controller files. Always pass actions through MediatR pipeline.
* **Delete Behavior Invariants**: **SQL Server will reject multiple cascade paths**. Use `DeleteBehavior.Restrict` for optional/many-to-many routes or composite user associations.
* **Fluent API Location**: DB constraints must always be configured inside `CourtManager.Infrastructure/Data/*Configuration.cs`.
* **Async Protocol**: All EF Core methods MUST be called asynchronously (use `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`) and accept `cancellationToken` parameter.
