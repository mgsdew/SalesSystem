using UserAPI.Models.Entities;

namespace UserAPI.Repositories.Interfaces;

/// <summary>
/// Interface for user repository operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets all users from the repository.
    /// </summary>
    /// <returns>A collection of all users.</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <returns>The user if found, null otherwise.</returns>
    Task<User?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns>The user if found, null otherwise.</returns>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>The user if found, null otherwise.</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The added user with generated ID.</returns>
    Task<User> AddUserAsync(User user);

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    Task<bool> UpdateUserAsync(User user);

    /// <summary>
    /// Deletes a user from the repository by username.
    /// </summary>
    /// <param name="username">The username of the user to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    Task<bool> DeleteUserByUsernameAsync(string username);

    /// <summary>
    /// Checks if a user with the specified username already exists.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the username exists, false otherwise.</returns>
    Task<bool> UsernameExistsAsync(string username);

    /// <summary>
    /// Checks if a user with the specified email already exists.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <returns>True if the email exists, false otherwise.</returns>
    Task<bool> EmailExistsAsync(string email);
}