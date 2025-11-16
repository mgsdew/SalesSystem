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

// Load environment variables from parent .env file if it exists
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

// Add services to the container
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseInMemoryDatabase("PaymentTestDB"));

// Register application services with Dependency Injection
builder.Services.AddScoped<ICardPaymentService, CardPaymentService>();
builder.Services.AddScoped<ICardPaymentRepository, CardPaymentRepository>();

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
            Email = "support@payment-api.com"
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
    // app.UseSwaggerUI(options =>
    // {
    //     //options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1");
    //    options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    // });
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
