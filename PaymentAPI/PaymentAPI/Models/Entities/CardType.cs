namespace PaymentAPI.Models.Entities;

/// <summary>
/// Represents the type of credit card.
/// </summary>
public enum CardType
{
    /// <summary>
    /// Unknown card type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Visa card type.
    /// </summary>
    Visa,

    /// <summary>
    /// MasterCard card type.
    /// </summary>
    MasterCard,

    /// <summary>
    /// American Express card type.
    /// </summary>
    AmericanExpress,

    /// <summary>
    /// Discover card type.
    /// </summary>
    Discover
}