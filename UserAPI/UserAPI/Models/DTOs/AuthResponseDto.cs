namespace UserAPI.Models.DTOs;

/// <summary>
/// Response DTO for authentication operations.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// The secure authentication token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The authenticated user information.
    /// </summary>
    public UserResponseDto User { get; set; } = null!;
}