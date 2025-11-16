namespace PaymentAPI.Models.Entities;

/// <summary>
/// Represents a card payment entity for database operations.
/// </summary>
public class CardPayment
{
    /// <summary>
    /// Gets or sets the unique identifier for the card payment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the card number.
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the card number is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the validation was performed.
    /// </summary>
    public DateTime ValidatedAt { get; set; }

    /// <summary>
    /// Gets or sets the card type (Visa, MasterCard, Amex, etc.).
    /// </summary>
    public CardType CardType { get; set; }
}
