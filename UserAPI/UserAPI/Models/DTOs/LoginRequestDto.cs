namespace UserAPI.Models.DTOs;

/// <summary>
/// DTO for login requests.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// User's username or email address.
    /// </summary>
    public string UsernameOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}