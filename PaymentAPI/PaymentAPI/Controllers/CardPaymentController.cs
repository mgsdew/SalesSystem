using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Models.DTOs;
using PaymentAPI.Services.Interfaces;

namespace PaymentAPI.Controllers;

/// <summary>
/// Controller for card payment validation operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CardPaymentController : ControllerBase
{
    private readonly ICardPaymentService _cardPaymentService;
    private readonly ILogger<CardPaymentController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardPaymentController"/> class.
    /// </summary>
    /// <param name="cardPaymentService">The card payment service.</param>
    /// <param name="logger">The logger instance.</param>
    public CardPaymentController(ICardPaymentService cardPaymentService, ILogger<CardPaymentController> logger)
    {
        _cardPaymentService = cardPaymentService ?? throw new ArgumentNullException(nameof(cardPaymentService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates a credit card number using the Luhn algorithm.
    /// </summary>
    /// <param name="request">The card payment request containing the card number to validate.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A response indicating whether the card number is valid.</returns>
    /// <response code="200">Returns the validation result.</response>
    /// <response code="400">If the request is invalid or the card number format is incorrect.</response>
    /// <response code="401">If authentication token is missing or invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    /// <remarks>This endpoint requires authentication via Authorization header.</remarks>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(CardPaymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateCard(
        [FromBody] CardPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received card validation request");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for card validation request");
                return BadRequest(ModelState);
            }

            var response = await _cardPaymentService.ValidateCardAsync(request, cancellationToken);

            _logger.LogInformation("Card validation request processed successfully");

            return Ok(response);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Null argument provided in card validation request");
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = ex.Message,
                Instance = HttpContext.Request.Path
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing card validation request");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred while processing your request. Please try again later.",
                Instance = HttpContext.Request.Path
            });
        }
    }

    /// <summary>
    /// Health check endpoint to verify the API is running.
    /// </summary>
    /// <returns>A success response if the API is healthy.</returns>
    /// <response code="200">API is healthy and running.</response>
    /// <remarks>This endpoint does not require authentication.</remarks>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        _logger.LogInformation("Health check requested");
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Deletes card payment records by card number. Requires Admin role.
    /// </summary>
    /// <param name="cardNumber">The card number to delete records for.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A response indicating whether the deletion was successful.</returns>
    /// <response code="200">Records deleted successfully.</response>
    /// <response code="400">If the card number is invalid.</response>
    /// <response code="403">If user does not have Admin role.</response>
    /// <response code="404">If no records found for the card number.</response>
    /// <response code="401">If authentication token is missing or invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    /// <remarks>This endpoint requires authentication and Admin role via Authorization header.</remarks>
    [HttpDelete("delete/{cardNumber}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteByCardNumber(
        string cardNumber,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received delete request for card number: {CardNumber}", cardNumber);

            // Check if user has Admin role
            var userRole = HttpContext.Items["UserRole"]?.ToString();
            if (userRole != "Admin")
            {
                _logger.LogWarning("Unauthorized delete attempt by user with role: {Role}", userRole);
                return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "Admin role required to delete card payment records",
                    Instance = HttpContext.Request.Path
                });
            }

            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                _logger.LogWarning("Invalid card number provided for deletion");
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Detail = "Card number cannot be null or empty",
                    Instance = HttpContext.Request.Path
                });
            }

            var result = await _cardPaymentService.DeleteByCardNumberAsync(cardNumber, cancellationToken);

            if (result)
            {
                _logger.LogInformation("Successfully deleted records for card number: {CardNumber}", cardNumber);
                return Ok(new { message = "Card payment records deleted successfully", cardNumber });
            }
            else
            {
                _logger.LogWarning("No records found for card number: {CardNumber}", cardNumber);
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"No card payment records found for card number: {cardNumber}",
                    Instance = HttpContext.Request.Path
                });
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid argument provided for card deletion");
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = ex.Message,
                Instance = HttpContext.Request.Path
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting card payment records");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred while processing your request. Please try again later.",
                Instance = HttpContext.Request.Path
            });
        }
    }
}
