using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentAPI.Models.DTOs;
using PaymentAPI.Repositories.Interfaces;
using PaymentAPI.Services;
using Xunit;

namespace PaymentAPI.Tests.Services;

public class CardPaymentServiceTests
{
    private readonly Mock<ICardPaymentRepository> _mockRepository;
    private readonly Mock<ILogger<CardPaymentService>> _mockLogger;
    private readonly CardPaymentService _service;

    public CardPaymentServiceTests()
    {
        _mockRepository = new Mock<ICardPaymentRepository>();
        _mockLogger = new Mock<ILogger<CardPaymentService>>();
        _service = new CardPaymentService(_mockRepository.Object, _mockLogger.Object);
    }

    #region CreditCardValidator Tests

    [Theory]
    [InlineData("4532015112830366", true)]  // Valid Visa
    [InlineData("5425233430109903", true)]  // Valid MasterCard
    [InlineData("374245455400126", true)]   // Valid American Express
    [InlineData("6011111111111117", true)]  // Valid Discover
    [InlineData("4532015112830367", false)] // Invalid Visa (wrong checksum)
    [InlineData("1234567890123456", false)] // Invalid card number
    [InlineData("0000000000000000", true)]  // Edge case: all zeros (passes Luhn)
    public void CreditCardValidator_Should_ReturnCorrectValidationResult(string cardNumber, bool expectedResult)
    {
        // Act
        var result = _service.CreditCardValidator(cardNumber);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CreditCardValidator_Should_ReturnFalse_WhenCardNumberIsNullOrEmpty(string cardNumber)
    {
        // Act
        var result = _service.CreditCardValidator(cardNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("123")]           // Too short
    [InlineData("12345678901234567890")] // Too long
    public void CreditCardValidator_Should_ReturnFalse_WhenCardNumberLengthIsInvalid(string cardNumber)
    {
        // Act
        var result = _service.CreditCardValidator(cardNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("4532-0151-1283-0366")] // Valid Visa with dashes
    [InlineData("4532 0151 1283 0366")] // Valid Visa with spaces
    public void CreditCardValidator_Should_HandleCardNumberWithSpacesAndDashes(string cardNumber)
    {
        // Act
        var result = _service.CreditCardValidator(cardNumber);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CreditCardValidator_Should_ReturnFalse_WhenCardNumberContainsLetters()
    {
        // Arrange
        var cardNumber = "453201511283036A";

        // Act
        var result = _service.CreditCardValidator(cardNumber);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region DetermineCardType Tests

    [Theory]
    [InlineData("4532015112830366", "Visa")]
    [InlineData("4111111111111111", "Visa")]
    [InlineData("5425233430109903", "MasterCard")]
    [InlineData("5555555555554444", "MasterCard")]
    [InlineData("2221000000000009", "MasterCard")] // New MasterCard range
    [InlineData("374245455400126", "American Express")]
    [InlineData("371449635398431", "American Express")]
    [InlineData("6011111111111117", "Discover")]
    [InlineData("6500000000000002", "Discover")]
    [InlineData("9999999999999999", "Unknown")]
    public void DetermineCardType_Should_ReturnCorrectCardType(string cardNumber, string expectedCardType)
    {
        // Act
        var result = _service.DetermineCardType(cardNumber);

        // Assert
        result.Should().Be(expectedCardType);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void DetermineCardType_Should_ReturnUnknown_WhenCardNumberIsNullOrEmpty(string cardNumber)
    {
        // Act
        var result = _service.DetermineCardType(cardNumber);

        // Assert
        result.Should().Be("Unknown");
    }

    #endregion

    #region ValidateCardAsync Tests

    [Fact]
    public async Task ValidateCardAsync_Should_ReturnValidResponse_WhenCardIsValid()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "4532015112830366"
        };

        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<PaymentAPI.Models.Entities.CardPayment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentAPI.Models.Entities.CardPayment cp, CancellationToken ct) => cp);

        // Act
        var result = await _service.ValidateCardAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.CardType.Should().Be("Visa");
        result.MaskedCardNumber.Should().Be("************0366");
        result.Message.Should().Be("Card number is valid");
        result.ValidatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<PaymentAPI.Models.Entities.CardPayment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCardAsync_Should_ReturnInvalidResponse_WhenCardIsInvalid()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "4532015112830367" // Invalid checksum
        };

        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<PaymentAPI.Models.Entities.CardPayment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentAPI.Models.Entities.CardPayment cp, CancellationToken ct) => cp);

        // Act
        var result = await _service.ValidateCardAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Card number is invalid");

        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<PaymentAPI.Models.Entities.CardPayment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCardAsync_Should_ThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ValidateCardAsync(null));
    }

    [Fact]
    public async Task ValidateCardAsync_Should_SaveValidationResultToRepository()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "5425233430109903"
        };

        PaymentAPI.Models.Entities.CardPayment savedCardPayment = null;

        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<PaymentAPI.Models.Entities.CardPayment>(), It.IsAny<CancellationToken>()))
            .Callback<PaymentAPI.Models.Entities.CardPayment, CancellationToken>((cp, ct) => savedCardPayment = cp)
            .ReturnsAsync((PaymentAPI.Models.Entities.CardPayment cp, CancellationToken ct) => cp);

        // Act
        await _service.ValidateCardAsync(request);

        // Assert
        savedCardPayment.Should().NotBeNull();
        savedCardPayment.CardNumber.Should().Be(request.CardNumber);
        savedCardPayment.IsValid.Should().BeTrue();
        savedCardPayment.CardType.Should().Be("MasterCard");
        savedCardPayment.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task ValidateCardAsync_Should_RespectCancellationToken()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "4532015112830366"
        };

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<PaymentAPI.Models.Entities.CardPayment>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => _service.ValidateCardAsync(request, cts.Token));
    }

    #endregion

    #region Luhn Algorithm Edge Cases

    [Theory]
    [InlineData("4532015112830366")] // Valid 16-digit card
    [InlineData("6011111111111117")] // Valid 16-digit card
    [InlineData("378282246310005")] // Valid 15-digit Amex
    public void CreditCardValidator_Should_ValidateDifferentCardLengths(string cardNumber)
    {
        // Act
        var result = _service.CreditCardValidator(cardNumber);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
