using PaymentAPI.Models.Entities;
using PaymentAPI.Repositories.Interfaces;
using PaymentAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace PaymentAPI.Repositories;


public class CardPaymentRepository : ICardPaymentRepository
{
    private readonly PaymentDbContext _context;
    private readonly ILogger<CardPaymentRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardPaymentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public CardPaymentRepository(PaymentDbContext context, ILogger<CardPaymentRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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

            var existing = await _context.CardPayments.FindAsync(new object[] { cardPayment.Id }, cancellationToken);
            if (existing == null)
            {
                _context.CardPayments.Add(cardPayment);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(cardPayment);
            }
            await _context.SaveChangesAsync(cancellationToken);

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
    public async Task<CardPayment?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving card payment record with ID: {Id}", id);

            var cardPayment = await _context.CardPayments.FindAsync(new object[] { id }, cancellationToken);

            if (cardPayment != null)
            {
                _logger.LogInformation("Card payment record found with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Card payment record not found with ID: {Id}", id);
            }

            return cardPayment;
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

            var result = await _context.CardPayments
                .OrderByDescending(cp => cp.ValidatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} card payment records", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all card payment records");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting card payment record with ID: {Id}", id);

            var cardPayment = await _context.CardPayments.FindAsync(new object[] { id }, cancellationToken);
            if (cardPayment == null)
            {
                _logger.LogWarning("Card payment record not found for deletion with ID: {Id}", id);
                return false;
            }

            _context.CardPayments.Remove(cardPayment);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Card payment record deleted successfully with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting card payment record with ID: {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            throw new ArgumentException("Card number cannot be null or empty", nameof(cardNumber));
        }

        try
        {
            _logger.LogInformation("Deleting card payment records with card number: {CardNumber}", cardNumber);

            var cardPayments = await _context.CardPayments
                .Where(cp => cp.CardNumber == cardNumber)
                .ToListAsync(cancellationToken);

            if (!cardPayments.Any())
            {
                _logger.LogWarning("No card payment records found for deletion with card number: {CardNumber}", cardNumber);
                return false;
            }

            _context.CardPayments.RemoveRange(cardPayments);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {Count} card payment records with card number: {CardNumber}", cardPayments.Count, cardNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting card payment records with card number: {CardNumber}", cardNumber);
            throw;
        }
    }
}
