namespace PaymentAPI.Models.DTOs;

/// <summary>
/// Response DTO for card payment validation.
/// </summary>
public class CardPaymentResponseDto
{
    /// <summary>
    /// Gets or sets a value indicating whether the card number is valid according to the Luhn algorithm.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the masked card number (showing only last 4 digits for security).
    /// </summary>
    public string MaskedCardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card type (Visa, MasterCard, Amex, Discover, etc.).
    /// </summary>
    public string? CardType { get; set; }

    /// <summary>
    /// Gets or sets the validation message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the validation was performed.
    /// </summary>
    public DateTime ValidatedAt { get; set; }
}
