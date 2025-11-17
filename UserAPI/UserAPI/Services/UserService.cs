using UserAPI.Models.DTOs;
using UserAPI.Models.Entities;
using UserAPI.Repositories.Interfaces;
using UserAPI.Services.Interfaces;

namespace UserAPI.Services;

/// <summary>
/// Service for managing user operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(MapToResponseDto);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return user != null ? MapToResponseDto(user) : null;
    }

    public async Task<UserResponseDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        return user != null ? MapToResponseDto(user) : null;
    }

    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user != null ? MapToResponseDto(user) : null;
    }

    public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userDto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(userDto.Username))
            throw new ArgumentException("Username is required", nameof(userDto.Username));

        if (string.IsNullOrWhiteSpace(userDto.Email))
            throw new ArgumentException("Email is required", nameof(userDto.Email));

        if (string.IsNullOrWhiteSpace(userDto.FirstName))
            throw new ArgumentException("First name is required", nameof(userDto.FirstName));

        if (string.IsNullOrWhiteSpace(userDto.LastName))
            throw new ArgumentException("Last name is required", nameof(userDto.LastName));

        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(userDto.Username))
            throw new InvalidOperationException("A user with this username already exists");

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(userDto.Email))
            throw new InvalidOperationException("A user with this email already exists");

        // Create user entity
        var user = new User
        {
            Username = userDto.Username.Trim().ToLower(),
            Email = userDto.Email.Trim().ToLower(),
            FirstName = userDto.FirstName.Trim(),
            LastName = userDto.LastName.Trim(),
            PasswordHash = HashPassword(userDto.Password ?? "defaultpassword"), // In production, require password
            Role = null, // Role will be assigned later by admin
            IsActive = userDto.IsActive
        };

        var createdUser = await _userRepository.AddUserAsync(user);
        return MapToResponseDto(createdUser);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(string username, UserRequestDto userDto)
    {
        var existingUser = await _userRepository.GetUserByUsernameAsync(username);
        if (existingUser == null)
            return null;

        // Validate input
        if (string.IsNullOrWhiteSpace(userDto.Username))
            throw new ArgumentException("Username is required", nameof(userDto.Username));

        if (string.IsNullOrWhiteSpace(userDto.Email))
            throw new ArgumentException("Email is required", nameof(userDto.Email));

        if (string.IsNullOrWhiteSpace(userDto.FirstName))
            throw new ArgumentException("First name is required", nameof(userDto.FirstName));

        if (string.IsNullOrWhiteSpace(userDto.LastName))
            throw new ArgumentException("Last name is required", nameof(userDto.LastName));

        // Check if username is being changed and if it already exists
        if (!existingUser.Username.Equals(userDto.Username, StringComparison.OrdinalIgnoreCase))
        {
            if (await _userRepository.UsernameExistsAsync(userDto.Username))
                throw new InvalidOperationException("A user with this username already exists");
        }

        // Check if email is being changed and if it already exists
        if (!existingUser.Email.Equals(userDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _userRepository.EmailExistsAsync(userDto.Email))
                throw new InvalidOperationException("A user with this email already exists");
        }

        // Update user entity
        existingUser.Username = userDto.Username.Trim().ToLower();
        existingUser.Email = userDto.Email.Trim().ToLower();
        existingUser.FirstName = userDto.FirstName.Trim();
        existingUser.LastName = userDto.LastName.Trim();
        if (!string.IsNullOrWhiteSpace(userDto.Password))
        {
            existingUser.PasswordHash = HashPassword(userDto.Password);
        }
        // Note: Role is not updated here - use AssignRoleAsync for role management
        existingUser.IsActive = userDto.IsActive;

        var success = await _userRepository.UpdateUserAsync(existingUser);
        return success ? MapToResponseDto(existingUser) : null;
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        return await _userRepository.DeleteUserByUsernameAsync(username);
    }

    public async Task<UserResponseDto?> AssignRoleAsync(string targetUsername, UserRole role, string requestingUsername)
    {
        // Verify that the requesting user is an admin
        var requestingUser = await _userRepository.GetUserByUsernameAsync(requestingUsername);
        if (requestingUser == null || requestingUser.Role != UserRole.Admin)
        {
            throw new UnauthorizedAccessException("Only administrators can assign roles");
        }

        // Get the target user
        var targetUser = await _userRepository.GetUserByUsernameAsync(targetUsername);
        if (targetUser == null)
        {
            return null;
        }

        // Assign the role
        targetUser.Role = role;
        targetUser.UpdatedAt = DateTime.UtcNow;

        var success = await _userRepository.UpdateUserAsync(targetUser);
        return success ? MapToResponseDto(targetUser) : null;
    }

    public async Task<UserResponseDto?> AuthenticateUserAsync(string usernameOrEmail, string password)
    {
        // Try to find user by username first, then by email
        var user = await _userRepository.GetUserByUsernameAsync(usernameOrEmail) ??
                   await _userRepository.GetUserByEmailAsync(usernameOrEmail);

        if (user == null || !user.IsActive)
            return null;

        // In production, use proper password hashing/verification
        // For demo purposes, we're using simple comparison
        if (VerifyPassword(password, user.PasswordHash))
        {
            return MapToResponseDto(user);
        }

        return null;
    }

    private UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    private string HashPassword(string password)
    {
        // In production, use proper password hashing like BCrypt, Argon2, or PBKDF2
        // For demo purposes, using simple hash
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password + "salt"));
    }

    private bool VerifyPassword(string password, string hash)
    {
        // In production, use proper password verification
        // For demo purposes, using simple comparison
        return HashPassword(password) == hash;
    }
}