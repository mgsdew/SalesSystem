using PaymentAPI.Models.Entities;
using PaymentAPI.Repositories.Interfaces;
using System.Collections.Concurrent;

namespace PaymentAPI.Repositories;

/// <summary>
/// Card payment repository with SQL Server configuration for demonstration purposes.
/// NOTE: This implementation currently uses in-memory storage for demo/testing purposes.
/// 
/// SQL Server Configuration:
/// - Connection String: "Server=DevServer;Database=PaymentDB;User Id=sa;Password=Dev123;..."
/// - Database: PaymentDB
/// - Server: DevServer
/// 
/// In production, this would use Entity Framework Core or Dapper with the following setup:
/// 1. Install NuGet: Microsoft.EntityFrameworkCore.SqlServer
/// 2. Create DbContext with DbSet&lt;CardPayment&gt;
/// 3. Use connection string from appsettings.json
/// 4. Apply migrations to create database schema
/// </summary>
public class CardPaymentRepository : ICardPaymentRepository
{
    // Using ConcurrentDictionary for thread-safe in-memory storage (demo purposes)
    // In production: Replace with DbContext or database connection
    private static readonly ConcurrentDictionary<Guid, CardPayment> _inMemoryStorage = new();
    private readonly ILogger<CardPaymentRepository> _logger;
    // private readonly IConfiguration _configuration; // Uncomment for SQL Server
    // Connection string available at: _configuration.GetConnectionString("DefaultConnection")

    /// <summary>
    /// Initializes a new instance of the <see cref="CardPaymentRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public CardPaymentRepository(ILogger<CardPaymentRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<CardPayment> SaveAsync(CardPayment cardPayment, CancellationToken cancellationToken = default)
    {
        if (cardPayment == null)
        {
            throw new ArgumentNullException(nameof(cardPayment));
        }

        try
        {
            _logger.LogInformation("Saving card payment record with ID: {Id}", cardPayment.Id);

            // Simulate async database operation
            await Task.Run(() =>
            {
                _inMemoryStorage.AddOrUpdate(
                    cardPayment.Id,
                    cardPayment,
                    (key, existingValue) => cardPayment);
            }, cancellationToken);

            _logger.LogInformation("Card payment record saved successfully with ID: {Id}", cardPayment.Id);

            return cardPayment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving card payment record");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<CardPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving card payment record with ID: {Id}", id);

            // Simulate async database operation
            var result = await Task.Run(() =>
            {
                _inMemoryStorage.TryGetValue(id, out var cardPayment);
                return cardPayment;
            }, cancellationToken);

            if (result != null)
            {
                _logger.LogInformation("Card payment record found with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Card payment record not found with ID: {Id}", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving card payment record with ID: {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CardPayment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all card payment records");

            // Simulate async database operation
            var result = await Task.Run(() =>
            {
                return _inMemoryStorage.Values.OrderByDescending(cp => cp.ValidatedAt).ToList();
            }, cancellationToken);

            _logger.LogInformation("Retrieved {Count} card payment records", result.Count());

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all card payment records");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting card payment record with ID: {Id}", id);

            // Simulate async database operation
            var result = await Task.Run(() =>
            {
                return _inMemoryStorage.TryRemove(id, out _);
            }, cancellationToken);

            if (result)
            {
                _logger.LogInformation("Card payment record deleted successfully with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Card payment record not found for deletion with ID: {Id}", id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting card payment record with ID: {Id}", id);
            throw;
        }
    }
}
