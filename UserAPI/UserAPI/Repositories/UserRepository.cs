using UserAPI.Models.Entities;
using UserAPI.Repositories.Interfaces;
using UserAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace UserAPI.Repositories;

/// <summary>
/// Database-backed implementation of the user repository.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public UserRepository(UserDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all users");

            var users = await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} users", users.Count);

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all users");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID: {Id}", id);

            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                _logger.LogInformation("User found with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("User not found with ID: {Id}", id);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user with ID: {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        }

        try
        {
            _logger.LogInformation("Retrieving user with username: {Username}", username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user != null)
            {
                _logger.LogInformation("User found with username: {Username}", username);
            }
            else
            {
                _logger.LogWarning("User not found with username: {Username}", username);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user with username: {Username}", username);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        }

        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                _logger.LogInformation("User found with email: {Email}", email);
            }
            else
            {
                _logger.LogWarning("User not found with email: {Email}", email);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user with email: {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<User> AddUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {
            _logger.LogInformation("Adding new user with username: {Username}", user.Username);

            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User added successfully with ID: {Id}", user.Id);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding user with username: {Username}", user.Username);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {
            _logger.LogInformation("Updating user with ID: {Id}", user.Id);

            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                _logger.LogWarning("User not found for update with ID: {Id}", user.Id);
                return false;
            }

            // Update properties
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("User updated successfully with ID: {Id}", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID: {Id}", user.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteUserByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        }

        try
        {
            _logger.LogInformation("Deleting user with username: {Username}", username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                _logger.LogWarning("User not found for deletion with username: {Username}", username);
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User deleted successfully with username: {Username}", username);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with username: {Username}", username);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        }

        try
        {
            _logger.LogDebug("Checking if username exists: {Username}", username);

            var exists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == username.ToLower());

            _logger.LogDebug("Username {Username} exists: {Exists}", username, exists);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking if username exists: {Username}", username);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        }

        try
        {
            _logger.LogDebug("Checking if email exists: {Email}", email);

            var exists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());

            _logger.LogDebug("Email {Email} exists: {Exists}", email, exists);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking if email exists: {Email}", email);
            throw;
        }
    }
}