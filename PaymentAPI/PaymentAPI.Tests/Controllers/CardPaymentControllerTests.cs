using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentAPI.Controllers;
using PaymentAPI.Models.DTOs;
using PaymentAPI.Models.Entities;
using PaymentAPI.Services.Interfaces;
using Xunit;

namespace PaymentAPI.Tests.Controllers;

public class CardPaymentControllerTests
{
    private readonly Mock<ICardPaymentService> _mockService;
    private readonly Mock<ILogger<CardPaymentController>> _mockLogger;
    private readonly CardPaymentController _controller;

    public CardPaymentControllerTests()
    {
        _mockService = new Mock<ICardPaymentService>();
        _mockLogger = new Mock<ILogger<CardPaymentController>>();
        _controller = new CardPaymentController(_mockService.Object, _mockLogger.Object);
        
        // Set up HttpContext for ProblemDetails
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task ValidateCard_Should_ReturnOk_WhenValidationSucceeds()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "4532015112830366"
        };

        var expectedResponse = new CardPaymentResponseDto
        {
            IsValid = true,
            CardType = CardType.Visa,
            MaskedCardNumber = "************0366",
            Message = "Card number is valid",
            ValidatedAt = DateTime.UtcNow
        };

        _mockService.Setup(s => s.ValidateCardAsync(It.IsAny<CardPaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateCard(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);

        _mockService.Verify(s => s.ValidateCardAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCard_Should_ReturnOk_WhenCardIsInvalid()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "1234567890123456"
        };

        var expectedResponse = new CardPaymentResponseDto
        {
            IsValid = false,
            CardType = CardType.Unknown,
            MaskedCardNumber = "************3456",
            Message = "Card number is invalid",
            ValidatedAt = DateTime.UtcNow
        };

        _mockService.Setup(s => s.ValidateCardAsync(It.IsAny<CardPaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateCard(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as CardPaymentResponseDto;
        response!.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateCard_Should_ReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "invalid"
        };

        _controller.ModelState.AddModelError("CardNumber", "Card number must contain only digits and be between 13-19 characters");

        // Act
        var result = await _controller.ValidateCard(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        _mockService.Verify(s => s.ValidateCardAsync(It.IsAny<CardPaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidateCard_Should_ReturnBadRequest_WhenArgumentNullExceptionIsThrown()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "4532015112830366"
        };

        _mockService.Setup(s => s.ValidateCardAsync(It.IsAny<CardPaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException("request"));

        // Act
        var result = await _controller.ValidateCard(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ProblemDetails>();
        
        var problemDetails = badRequestResult.Value as ProblemDetails;
        problemDetails!.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task ValidateCard_Should_ReturnInternalServerError_WhenUnexpectedExceptionIsThrown()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "4532015112830366"
        };

        _mockService.Setup(s => s.ValidateCardAsync(It.IsAny<CardPaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.ValidateCard(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        
        var problemDetails = objectResult.Value as ProblemDetails;
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task ValidateCard_Should_PassCancellationToken()
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = "4532015112830366"
        };

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var expectedResponse = new CardPaymentResponseDto
        {
            IsValid = true,
            CardType = CardType.Visa,
            MaskedCardNumber = "************0366",
            Message = "Card number is valid",
            ValidatedAt = DateTime.UtcNow
        };

        _mockService.Setup(s => s.ValidateCardAsync(It.IsAny<CardPaymentRequestDto>(), cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateCard(request, cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockService.Verify(s => s.ValidateCardAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public void HealthCheck_Should_ReturnOk()
    {
        // Act
        var result = _controller.HealthCheck();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Theory]
    [InlineData("4532015112830366")] // Visa
    [InlineData("5425233430109903")] // MasterCard
    [InlineData("374245455400126")]  // Amex
    public async Task ValidateCard_Should_HandleDifferentCardTypes(string cardNumber)
    {
        // Arrange
        var request = new CardPaymentRequestDto
        {
            CardNumber = cardNumber
        };

        var expectedResponse = new CardPaymentResponseDto
        {
            IsValid = true,
            Message = "Card number is valid",
            ValidatedAt = DateTime.UtcNow
        };

        _mockService.Setup(s => s.ValidateCardAsync(It.IsAny<CardPaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateCard(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
