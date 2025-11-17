namespace PaymentAPI.Services.Interfaces;

/// <summary>
/// Interface for UserAPI communication
/// </summary>
public interface IUserApiClient
{
    /// <summary>
    /// Validate user exists and is active
    /// </summary>
    Task<bool> ValidateUserAsync(string username);

    /// <summary>
    /// Get user details for payment processing
    /// </summary>
    Task<UserDto?> GetUserAsync(string username);
}