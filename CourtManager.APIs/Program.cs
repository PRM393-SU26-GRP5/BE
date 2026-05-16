using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CourtManager.Application;
using CourtManager.Infrastructure;
using CourtManager.APIs.Configuration;
using CourtManager.APIs.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURATION SECTION
// ============================================================================

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);

// ============================================================================
// DEPENDENCY INJECTION REGISTRATION
// ============================================================================

// Add Application layer services (MediatR, FluentValidation, AutoMapper)
builder.Services.AddApplicationServices();

// Add JWT Token Service with configuration
builder.Services.AddJwtTokenService(
    jwtSettings.Secret,
    jwtSettings.Issuer,
    jwtSettings.Audience,
    jwtSettings.AccessTokenExpirationInMinutes,
    jwtSettings.RefreshTokenExpirationInDays);

// Add Infrastructure layer services (DbContext, Repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Controllers
builder.Services.AddControllers();

// Add API Versioning and Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Court Manager API",
        Version = "v1.0.0",
        Description = "RESTful API for managing sports court bookings with Clean Architecture and CQRS pattern",
    });
});

// ============================================================================
// AUTHENTICATION CONFIGURATION
// ============================================================================

// Configure JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey(),
            ClockSkew = TimeSpan.Zero // No clock skew tolerance
        };

        // Configure event handlers for JWT validation
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Add custom logic here if needed after token validation
                return Task.CompletedTask;
            }
        };
    });

// Add Authorization policies
builder.Services.AddAuthorization();

// ============================================================================
// CORS CONFIGURATION
// ============================================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================================================
// LOGGING CONFIGURATION
// ============================================================================

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug();

// ============================================================================
// BUILD APPLICATION
// ============================================================================

var app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE CONFIGURATION
// ============================================================================

// Global Exception Handling Middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Swagger UI (in Development only, but can be enabled in Production if desired)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Court Manager API v1.0.0");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS Middleware
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.UseRouting();

// Map Controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () =>
    Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("Health");

// ============================================================================
// APPLICATION STARTUP
// ============================================================================

try
{
    app.Logger.LogInformation("Starting Court Manager API...");

    // Apply pending migrations automatically (optional - comment out if you prefer manual migrations)
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CourtManager.Infrastructure.ApplicationDbContext>();
        // dbContext.Database.EnsureCreated(); // Creates database if it doesn't exist
    }

    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application terminated unexpectedly.");
    throw;
}
