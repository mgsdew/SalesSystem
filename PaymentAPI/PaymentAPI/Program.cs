using PaymentAPI.Services;
using PaymentAPI.Services.Interfaces;
using PaymentAPI.Repositories;
using PaymentAPI.Repositories.Interfaces;
using PaymentAPI.Middleware;
using PaymentAPI.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
// Robust path resolution for different execution contexts (VS, CLI, Docker)
string GetEnvFilePath()
{
    // Get the directory where the application is running from
    var appDirectory = AppContext.BaseDirectory;

    // Navigate up to find the solution root
    // From: PaymentAPI/PaymentAPI/bin/Debug/net8.0/
    // To:   SalesSystem/
    var solutionRoot = Path.GetFullPath(Path.Combine(appDirectory, "..", "..", "..", ".."));

    // Check if we're in the solution root
    if (Directory.Exists(solutionRoot) && File.Exists(Path.Combine(solutionRoot, "SalesSystem.sln")))
    {
        return Path.Combine(solutionRoot, ".env");
    }

    // Fallback: check relative to current directory (for CLI runs)
    var currentDirEnv = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(currentDirEnv))
    {
        return currentDirEnv;
    }

    // Fallback: check parent directory
    var parentDirEnv = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
    if (File.Exists(parentDirEnv))
    {
        return parentDirEnv;
    }

    // Fallback: check grandparent directory (original logic)
    var grandparentDirEnv = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
    if (File.Exists(grandparentDirEnv))
    {
        return grandparentDirEnv;
    }

    return null; // No .env file found
}

var envFilePath = GetEnvFilePath();
if (!string.IsNullOrEmpty(envFilePath))
{
    Env.Load(envFilePath);
}

// Add services to the container
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<PaymentDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection");

    if (!string.IsNullOrEmpty(connectionString))
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        // Fallback to in-memory database for development/testing
        options.UseInMemoryDatabase("PaymentTestDB");
    }
});

// Register application services with Dependency Injection
builder.Services.AddScoped<ICardPaymentService, CardPaymentService>();
builder.Services.AddScoped<ICardPaymentRepository, CardPaymentRepository>();

// Register inter-service communication
builder.Services.AddHttpClient<IUserApiClient, UserApiClient>(client =>
{
    var userApiUrl = Environment.GetEnvironmentVariable("USERAPI_URL") ?? "http://localhost:5160";
    client.BaseAddress = new Uri(userApiUrl);
    client.DefaultRequestHeaders.Add("Authorization", "Bearer dev-token-123456");
});

builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Payment Card Validation API",
        Description = "An ASP.NET Core Web API for validating credit card numbers using the Luhn algorithm",
        Contact = new OpenApiContact
        {
            Name = "Payment API Team",
            Email = "mgsdew03@gmail.com"
        }
    });

    // Add Authorization support to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter a valid token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "Token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Enable XML comments for Swagger documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1");
        //options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Use custom authentication middleware 
// app.UseAuthTokenMiddleware();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

// Make the Program class accessible for integration tests
public partial class Program { }
