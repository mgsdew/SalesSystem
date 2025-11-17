using PaymentAPI.Models.DTOs;
using PaymentAPI.Services.Interfaces;
using PaymentAPI.Repositories.Interfaces;
using PaymentAPI.Models.Entities;

namespace PaymentAPI.Services;

/// <summary>
/// Service for card payment validation using the Luhn algorithm.
/// </summary>
public class CardPaymentService : ICardPaymentService
{
    private readonly ICardPaymentRepository _repository;
    private readonly ILogger<CardPaymentService> _logger;
    private readonly IUserApiClient _userApiClient;
    private readonly IMessagePublisher _messagePublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardPaymentService"/> class.
    /// </summary>
    /// <param name="repository">The card payment repository.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userApiClient">Client for UserAPI communication.</param>
    /// <param name="messagePublisher">Publisher for message events.</param>
    public CardPaymentService(
        ICardPaymentRepository repository,
        ILogger<CardPaymentService> logger,
        IUserApiClient userApiClient,
        IMessagePublisher messagePublisher)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userApiClient = userApiClient ?? throw new ArgumentNullException(nameof(userApiClient));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
    }

    /// <inheritdoc/>
    public async Task<CardPaymentResponseDto> ValidateCardAsync(CardPaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Validating card number with length: {Length}", request.CardNumber?.Length ?? 0);

        try
        {
            var isValid = CreditCardValidator(request.CardNumber);
            var cardType = DetermineCardType(request.CardNumber);
            var maskedCardNumber = MaskCardNumber(request.CardNumber);

            var response = new CardPaymentResponseDto
            {
                IsValid = isValid,
                MaskedCardNumber = maskedCardNumber,
                CardType = cardType,
                Message = isValid ? "Card number is valid" : "Card number is invalid",
                ValidatedAt = DateTime.UtcNow
            };

            // Only save valid card validation results to repository
            if (isValid)
            {
                var cardPaymentEntity = new CardPayment
                {
                    Id = Guid.NewGuid().ToString(), // Convert Guid to string for database compatibility
                    CardNumber = request.CardNumber, // In production, this should be encrypted
                    IsValid = isValid,
                    CardType = cardType,
                    ValidatedAt = response.ValidatedAt
                };

                await _repository.SaveAsync(cardPaymentEntity, cancellationToken);
                _logger.LogInformation("Valid card validation saved to database. CardType: {CardType}", cardType);

                // Publish event for other services to consume
                var paymentEvent = new PaymentValidatedEvent
                {
                    CardId = cardPaymentEntity.Id,
                    CardNumber = maskedCardNumber,
                    IsValid = true,
                    ValidatedAt = response.ValidatedAt,
                    UserId = request.UserId ?? string.Empty // Assuming UserId is added to request
                };
                _messagePublisher.PublishPaymentValidated(paymentEvent);
            }
            else
            {
                _logger.LogInformation("Invalid card validation - not saved to database. CardType: {CardType}", cardType);
            }

            // Example: Validate user exists (if UserId provided)
            if (!string.IsNullOrEmpty(request.UserId))
            {
                var userExists = await _userApiClient.ValidateUserAsync(request.UserId);
                if (!userExists)
                {
                    response.Message = "Card is valid but user not found";
                    response.IsValid = false;
                }
            }

            _logger.LogInformation("Card validation completed. IsValid: {IsValid}, CardType: {CardType}", isValid, cardType);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while validating card number");
            throw;
        }
    }

    /// <inheritdoc/>
    public bool CreditCardValidator(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return false;
        }

        // Remove any spaces or dashes
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        // Check if the card number contains only digits
        if (!cardNumber.All(char.IsDigit))
        {
            return false;
        }

        // Check if the card number length is valid (13-19 digits)
        if (cardNumber.Length < 13 || cardNumber.Length > 19)
        {
            return false;
        }

        // Luhn Algorithm Implementation
        int sum = 0;
        bool alternate = false;

        // Process digits from right to left
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = cardNumber[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9; // Same as summing the digits (e.g., 16 -> 1+6 = 7)
                }
            }

            sum += digit;
            alternate = !alternate;
        }

        // The card number is valid if the sum is divisible by 10
        return sum % 10 == 0;
    }

    /// <inheritdoc/>
    public CardType DetermineCardType(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return CardType.Unknown;
        }

        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        // Visa: starts with 4
        if (cardNumber.StartsWith("4"))
        {
            return CardType.Visa;
        }

        // MasterCard: starts with 51-55 or 2221-2720
        if (cardNumber.Length >= 2)
        {
            var firstTwo = cardNumber.Substring(0, 2);
            if (int.TryParse(firstTwo, out int firstTwoDigits))
            {
                if (firstTwoDigits >= 51 && firstTwoDigits <= 55)
                {
                    return CardType.MasterCard;
                }
            }

            if (cardNumber.Length >= 4)
            {
                var firstFour = cardNumber.Substring(0, 4);
                if (int.TryParse(firstFour, out int firstFourDigits))
                {
                    if (firstFourDigits >= 2221 && firstFourDigits <= 2720)
                    {
                        return CardType.MasterCard;
                    }
                }
            }
        }

        // American Express: starts with 34 or 37
        if (cardNumber.StartsWith("34") || cardNumber.StartsWith("37"))
        {
            return CardType.AmericanExpress;
        }

        // Discover: starts with 6011, 622126-622925, 644-649, or 65
        if (cardNumber.StartsWith("6011") || cardNumber.StartsWith("65"))
        {
            return CardType.Discover;
        }

        if (cardNumber.Length >= 6)
        {
            var firstSix = cardNumber.Substring(0, 6);
            if (int.TryParse(firstSix, out int firstSixDigits))
            {
                if (firstSixDigits >= 622126 && firstSixDigits <= 622925)
                {
                    return CardType.Discover;
                }
            }
        }

        if (cardNumber.Length >= 3)
        {
            var firstThree = cardNumber.Substring(0, 3);
            if (int.TryParse(firstThree, out int firstThreeDigits))
            {
                if (firstThreeDigits >= 644 && firstThreeDigits <= 649)
                {
                    return CardType.Discover;
                }
            }
        }

        return CardType.Unknown;
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
            _logger.LogInformation("Deleting card payment records for card number: {CardNumber}", cardNumber);

            var result = await _repository.DeleteByCardNumberAsync(cardNumber, cancellationToken);

            if (result)
            {
                _logger.LogInformation("Successfully deleted card payment records for card number: {CardNumber}", cardNumber);
            }
            else
            {
                _logger.LogWarning("No card payment records found to delete for card number: {CardNumber}", cardNumber);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting card payment records for card number: {CardNumber}", cardNumber);
            throw;
        }
    }

    /// <summary>
    /// Masks the card number showing only the last 4 digits.
    /// </summary>
    /// <param name="cardNumber">The card number to mask.</param>
    /// <returns>The masked card number.</returns>
    private string MaskCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return string.Empty;
        }

        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        if (cardNumber.Length <= 4)
        {
            return new string('*', cardNumber.Length);
        }

        var lastFour = cardNumber.Substring(cardNumber.Length - 4);
        var masked = new string('*', cardNumber.Length - 4) + lastFour;

        return masked;
    }
}
