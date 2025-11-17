using UserAPI.Models.Entities;
using UserAPI.Repositories.Interfaces;

namespace UserAPI.Repositories;

/// <summary>
/// In-memory implementation of the user repository.
/// In production, this would be replaced with a database-backed implementation.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public UserRepository()
    {
        // Seed with some test data
        _users.Add(new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@example.com",
            FirstName = "System",
            LastName = "Administrator",
            PasswordHash = "cGFzc3dvcmQxMjNzYWx0", // base64 encoded "password123salt"
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        });

        _users.Add(new User
        {
            Id = Guid.NewGuid(),
            Username = "johndoe",
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "cGFzc3dvcmQxMjNzYWx0", // base64 encoded "password123salt"
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        });
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await Task.FromResult(_users.AsEnumerable());
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await Task.FromResult(_users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await Task.FromResult(_users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<User> AddUserAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        _users.Add(user);
        return await Task.FromResult(user);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
        {
            return await Task.FromResult(false);
        }

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.PasswordHash = user.PasswordHash;
        existingUser.Role = user.Role;
        existingUser.IsActive = user.IsActive;
        existingUser.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteUserByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user == null)
        {
            return await Task.FromResult(false);
        }

        _users.Remove(user);
        return await Task.FromResult(true);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await Task.FromResult(_users.Any(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await Task.FromResult(_users.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }
}