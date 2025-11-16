using PaymentAPI.Models.DTOs;
using PaymentAPI.Models.Entities;

namespace PaymentAPI.Services.Interfaces;

/// <summary>
/// Interface for card payment validation service.
/// </summary>
public interface ICardPaymentService
{
    /// <summary>
    /// Validates a credit card number using the Luhn algorithm asynchronously.
    /// </summary>
    /// <param name="request">The card payment request containing the card number.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validation response.</returns>
    Task<CardPaymentResponseDto> ValidateCardAsync(CardPaymentRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a credit card number using the Luhn algorithm.
    /// </summary>
    /// <param name="cardNumber">The card number to validate.</param>
    /// <returns>True if the card number is valid according to the Luhn algorithm; otherwise, false.</returns>
    bool CreditCardValidator(string cardNumber);

    /// <summary>
    /// Determines the card type based on the card number prefix.
    /// </summary>
    /// <param name="cardNumber">The card number to analyze.</param>
    /// <returns>The card type (Visa, MasterCard, Amex, Discover, etc.) or Unknown if not recognized.</returns>
    CardType DetermineCardType(string cardNumber);

    /// <summary>
    /// Deletes card payment records by card number asynchronously.
    /// </summary>
    /// <param name="cardNumber">The card number to delete records for.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether any records were deleted.</returns>
    Task<bool> DeleteByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
}
