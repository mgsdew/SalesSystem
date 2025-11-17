using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Models.DTOs;

/// <summary>
/// Request DTO for card payment validation.
/// </summary>
public class CardPaymentRequestDto
{
    /// <summary>
    /// Gets or sets the card number to validate.
    /// Must contain only digits and be between 13-19 characters.
    /// </summary>
    [Required(ErrorMessage = "Card number is required")]
    [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Card number must contain only digits and be between 13-19 characters")]
    [StringLength(19, MinimumLength = 13, ErrorMessage = "Card number must be between 13 and 19 digits")]
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier for validation.
    /// Optional - used for inter-service communication.
    /// </summary>
    public string? UserId { get; set; }
}
