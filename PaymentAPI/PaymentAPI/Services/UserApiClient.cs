using System.Net.Http.Json;
using PaymentAPI.Services.Interfaces;

namespace PaymentAPI.Services;

/// <summary>
/// Service for communicating with UserAPI
/// </summary>
public class UserApiClient : IUserApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserApiClient> _logger;

    public UserApiClient(HttpClient httpClient, ILogger<UserApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Validate user exists and is active
    /// </summary>
    public async Task<bool> ValidateUserAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/User/{username}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return user?.IsActive == true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user {Username}", username);
            return false;
        }
    }

    /// <summary>
    /// Get user details for payment processing
    /// </summary>
    public async Task<UserDto?> GetUserAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/User/{username}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {Username}", username);
            return null;
        }
    }
}

/// <summary>
/// DTO for inter-service communication (local copy)
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}