using UserAPI.Models.DTOs;
using UserAPI.Models.Entities;

namespace UserAPI.Services.Interfaces;

/// <summary>
/// Interface for user service operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <returns>A collection of user response DTOs.</returns>
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <returns>The user response DTO if found, null otherwise.</returns>
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns>The user response DTO if found, null otherwise.</returns>
    Task<UserResponseDto?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>The user response DTO if found, null otherwise.</returns>
    Task<UserResponseDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="userDto">The user creation data.</param>
    /// <returns>The created user response DTO.</returns>
    Task<UserResponseDto> CreateUserAsync(UserRequestDto userDto);

    /// <summary>
    /// Updates an existing user in the system.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <param name="userDto">The user update data.</param>
    /// <returns>The updated user response DTO if successful, null otherwise.</returns>
    Task<UserResponseDto?> UpdateUserAsync(string username, UserRequestDto userDto);

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    Task<bool> DeleteUserAsync(string username);

    /// <summary>
    /// Authenticates a user with username/email and password.
    /// </summary>
    /// <param name="usernameOrEmail">The user's username or email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>The authenticated user if successful, null otherwise.</returns>
    Task<UserResponseDto?> AuthenticateUserAsync(string usernameOrEmail, string password);

    /// <summary>
    /// Assigns a role to a user. Only administrators can perform this action.
    /// </summary>
    /// <param name="targetUsername">The username of the user to assign the role to.</param>
    /// <param name="role">The role to assign.</param>
    /// <param name="requestingUsername">The username of the admin performing the action.</param>
    /// <returns>The updated user response DTO if successful, null otherwise.</returns>
    Task<UserResponseDto?> AssignRoleAsync(string targetUsername, UserRole role, string requestingUsername);
}